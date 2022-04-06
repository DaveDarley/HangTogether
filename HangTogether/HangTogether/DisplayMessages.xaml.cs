using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using HangTogether.ServerManager;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;
using Xamarin.Forms.Xaml;

namespace HangTogether
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DisplayMessages : ContentPage
    {
        private User userFrom;
        private User userTo;
        private string nomUserTo;

        public string Title { get; set; }

        private  System.Timers.Timer aTimer; // initialiser a chaque instance de cette classe
        
        
        public DisplayMessages(User userSendingMessage, User userReceivingMessages)
        {
            InitializeComponent();
           // BindingContext = this;
           userFrom = userSendingMessage;
            userTo = userReceivingMessages;
            nomUserTo = userTo.nom + " " + userTo.prenom;
            Title = nomUserTo;
            this.BindingContext = this;
           // this.nomRecepteurMessage.SetBinding(Label.TextProperty, nomUserTo);
            whenUserConnected(); //au debut on recupere ts les messages entre les 2 users dans ma DB(s'il y en a)
           // SetTimer();
           wait_Tick();
        }
        
        //public string nomUserTo { get; }

        /*
         * Fonction qui s'occupe d'aller recuperer tous les ANCIENS messages entre 2 users pour les afficher.
         */
        public async void whenUserConnected()
        {
            DataBaseMessagesManager dataBaseMessagesManager = new DataBaseMessagesManager();
            List<Message> listMessagesTotal = await dataBaseMessagesManager.GetAllMessages("Messages");
            List<Message> messagesBetweenUserSendingAndUserReceiving =
                dataBaseMessagesManager.getConvos(userFrom, userTo, listMessagesTotal);
            displayAllConvos(messagesBetweenUserSendingAndUserReceiving);

        }

        /*
         * Chaque une seconde on verifie si n'y a pas eu de nouveaux
         * messages ajoutés dans ma Table : Nouveaux Messages
         */
      /*  private  void SetTimer()
        {
            // Create a timer with a one second interval.
            aTimer = new System.Timers.Timer(5000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += wait_Tick;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }*/
        
        /*
         * User A ecrit a User B ; le message de A est stocke dans ma table "Nouveaux Messages" de ma
         * DB ; Et ensuite on affiche le message
         */
        private  async void  wait_Tick(/*Object sender, ElapsedEventArgs e*/)
        {
            while(true){
                DataBaseMessagesManager dataBaseMessagesManager = new DataBaseMessagesManager();
                List<Message> nouveauxMessages = await dataBaseMessagesManager.GetAllMessages("Nouveaux Messages");
                
                // recupereation des nouveaux messages entre A et B; pas besoin de parcourir tous les messages pour afficher
                // le dernier message; la on recupere les derniers messages de A vers B directement.
                List<Message> nouveauxMessagesBetweenUserSendingAndUserReceiving =
                    dataBaseMessagesManager.getConvosFromOneUserToAnother(userTo, userFrom, nouveauxMessages);
                
                
                // Vu que User A a recupere les nouveaux messages de B vers A , alors on
                // efface ts les messages de B a A de ma table "Nouveaux Messages"
                if (nouveauxMessagesBetweenUserSendingAndUserReceiving.Count > 0)
                {
                    displayAllConvos(nouveauxMessagesBetweenUserSendingAndUserReceiving);
                    foreach (var message in nouveauxMessagesBetweenUserSendingAndUserReceiving)
                    {
                        await dataBaseMessagesManager.deleteMessageFromNonReadMEssages(message);
                    }
                }
            }

        }
        
        /*
         * Qd utilisateur clique sur send, on envoie le message dans la BD.
         * Le message est mis dans ma liste de message total mais aussi dans ma liste
         * de nouveaux messages jusqu'a ce que le user receptrice le lise.
         */
        private void  OnSendMessage(Object sender, EventArgs e)
        {
            DataBaseMessagesManager dataBaseMessagesManager = new DataBaseMessagesManager();
            var textToSend = this.message.Text;
            var dateTime = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
            Message message = new Message(userFrom.email, userTo.email, textToSend, "", dateTime);
            dataBaseMessagesManager.addNewConversation(message);
            dataBaseMessagesManager.addNonReadMessages(message);
            
            // Je dois dessiner le message sur l'ecran de l'emetteur du message:
            List<Message> messageToDisplay = new List<Message>();
            messageToDisplay.Add(message);
            displayAllConvos(messageToDisplay);
            this.message.Text = String.Empty;
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
                    
                    // put scrollview at the bottom the last message not working quite well
                    await this.scrollMessages.ScrollToAsync(frameMessage,ScrollToPosition.MakeVisible,true);

                }
               // await this.scrollMessages.ScrollToAsync(0,scrollMessages.Content.Height,true);

            }
        }

        
    }
}