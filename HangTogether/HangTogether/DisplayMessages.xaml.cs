using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HangTogether.ServerManager;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;
using Xamarin.Forms.Xaml;

/*
 * Idee principale sur le fonctionnement:
 * Soit userFrom le user qui envoie le message et userTo celui qui recoit le message
 * Si qd userFrom communique avec userTo et que userTo en ligne , je sauvegarde les messages dans ma table "Nouveaux messages"
 * et je les affiche sur l'ecran de userTo
 * Si userTo pas en ligne alors je sauvegarde les messages dans ma liste total de messages et ensuite qd il retourne en ligne
 * je parcours cette liste total de messages et je lui affiche tous ces conversations entre userFrom et lui
 */

namespace HangTogether
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DisplayMessages : ContentPage
    {
        public User userFrom;
        public  User userTo;
        private string nomUserTo;
        private DataBaseMessagesManager dataBaseMessagesManager;
        public string Title { get; set; }
        
        
        public DisplayMessages(User userSendingMessage, User userReceivingMessages)
        {
            InitializeComponent();
            
            dataBaseMessagesManager = new DataBaseMessagesManager();
            userFrom = userSendingMessage;
            userTo = userReceivingMessages;
            nomUserTo = userTo.nom + " " + userTo.prenom;
            Title = nomUserTo;
            this.BindingContext = this;
            whenUserConnected(); //au debut on recupere ts les messages entre les 2 users dans ma DB(s'il y en a)
            
        }

        public async void getNewMessages()
        {
            var page1=App.Current.MainPage.Navigation.NavigationStack.LastOrDefault().GetType().Name; 
            
            if (page1 != "DisplayMessages")
            {
                return;
            }

            await Task.Delay(5000);
            DataBaseMessagesManager dataBaseMessagesManager = new DataBaseMessagesManager();
            List<Message> messagesToMe = await dataBaseMessagesManager.getNonReadMessages(userFrom, userTo);
            
            if (messagesToMe.Count() != 0)
            {
                displayAllConvos(messagesToMe);
            }
            
            getNewMessages();
            
        }


        /*
         * Fonction qui s'occupe d'aller recuperer tous les ANCIENS messages entre 2 users pour les afficher
         * qd le user se connecte (monte sur la page de messages)
         */
        public async void whenUserConnected()
        {
            DataBaseMessagesManager dataBaseMessagesManager = new DataBaseMessagesManager();
            List<Message> listMessagesTotal = await dataBaseMessagesManager.getAllMessages(userFrom,userTo);
            displayAllConvos(listMessagesTotal);
            getNewMessages();
        }
        
        /*
         * Qd utilisateur clique sur send, on envoie le message dans la BD et on
         * l'affiche sur l'ecran de l'emetteur du message
         */
        private void  OnSendMessage(Object sender, EventArgs e)
        {
            DataBaseMessagesManager dataBaseMessagesManager = new DataBaseMessagesManager();
            var textToSend = this.message.Text;
            if (!(string.IsNullOrEmpty(textToSend))) // pas envoyer des messages vide
            {
                var dateTime = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
                string cleMessage = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond + "";
                Message message = new Message(userFrom.email, userTo.email, textToSend, cleMessage, dateTime);
                dataBaseMessagesManager.addNewConversation(message);
            
                // Je dois dessiner le message sur l'ecran de l'emetteur du message:
                List<Message> messageToDisplay = new List<Message>();
                messageToDisplay.Add(message);
                displayAllConvos(messageToDisplay);
                this.message.Text = String.Empty;
            }
        }

        /*
         * Cette fonction prend une liste de message en parametre et dessine les differents
         * messages sur l'ecran du user. Dependemment de l'emetteur et le recepteur le message
         * se dessine de facon differente sur l'ecran (A gauche ou A droite).
         */
        public async void displayAllConvos(List<Message> messageToDisplayOnScreen)
        {
            var layoutUser = containerMessages; // layout sur lequel on dessine les messages
            
            if (messageToDisplayOnScreen.Count > 0)
            {
                Frame lastFrameToScrollTo = new Frame();
                foreach (var message in messageToDisplayOnScreen)
                {
                    var stackLayout = new StackLayout()
                    {
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        Orientation = StackOrientation.Vertical,
                        Padding = new Thickness(0,0,0,0)
                    };
                    var labelText = new Label()
                    {
                        Text = message.message,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        TextColor = Color.Black,
                        Padding = new Thickness(0, 0, 0, 0),
                        Margin = new Thickness(0, 0, 0, 0),
                        FontSize = 20
                    };
                    var labelDatetime = new Label()
                    {
                        Text = message.timeStamp,
                        HorizontalOptions = LayoutOptions.EndAndExpand,
                        TextColor = Color.Black
                    };
                    stackLayout.Children.Add(labelText);
                    stackLayout.Children.Add(labelDatetime);
                    Frame frameMessage = new Frame()
                    {
                        HasShadow = false,
                        CornerRadius = 30,
                        Padding = new Thickness(30,5,30,5)
                    };
                    frameMessage.Content = stackLayout;
                    if (message.fromEmail == userFrom.email)
                    {
                        frameMessage.BackgroundColor = Color.LightPink;
                        frameMessage.HorizontalOptions = LayoutOptions.EndAndExpand;
                        frameMessage.Margin = new Thickness(40, 3, 5, 3);
                        
                    }
                    else
                    {
                        frameMessage.BackgroundColor = Color.LightGray;
                        frameMessage.HorizontalOptions = LayoutOptions.StartAndExpand;
                        frameMessage.Margin = new Thickness(5, 3, 40, 3);
                    }
                    
                    layoutUser.Children.Add(frameMessage);
                    lastFrameToScrollTo = frameMessage;
                }
                await this.scrollMessages.ScrollToAsync(lastFrameToScrollTo,ScrollToPosition.MakeVisible,true);


            }
        }
    }
}