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
        private  System.Timers.Timer aTimer; // initialiser a chaque instance de cette classe
        
        public DisplayMessages(User userSendingMessage, User userReceivingMessages)
        {
            InitializeComponent();
            
            userFrom = userSendingMessage;
            userTo = userReceivingMessages;
            nomUserTo = userTo.nom + " " + userTo.prenom;
            
            whenUserConnected(); //au debut on recupere ts les messages entre les 2 users dans ma DB(s'il y en a)
            SetTimer();
        }

        /*
         * Fonction qui s'occupe d'aller recuperer tous les messages entre 2 users pour les afficher.
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
        private  void SetTimer()
        {
            // Create a timer with a one second interval.
            aTimer = new System.Timers.Timer(1000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += wait_Tick;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }
        
        /*
         * User A ecrit a User B ; le message de A est stocke dans ma table "Nouveaux Messages" de ma
         * DB ; Et ensuite on affiche le message
         */
        private  async void  wait_Tick(Object sender, ElapsedEventArgs e)
        {
            DataBaseMessagesManager dataBaseMessagesManager = new DataBaseMessagesManager();
            List<Message> nouveauxMessages = await dataBaseMessagesManager.GetAllMessages("Nouveaux Messages");
            List<Message> nouveauxMessagesBetweenUserSendingAndUserReceiving =
                dataBaseMessagesManager.getConvosFromOneUserToAnother(userTo, userFrom, nouveauxMessages);
            displayAllConvos(nouveauxMessagesBetweenUserSendingAndUserReceiving);
            
            // Vu que User A a recupere les nouveaux messages de B vers A , alors on
            // efface ts les messages de B a A de ma table "Nouveaux Messages"
            if (nouveauxMessages.Count > 0)
            {
                foreach (var message in nouveauxMessages)
                {
                    await dataBaseMessagesManager.deleteMessageFromNonReadMEssages(message);
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
            Message message = new Message(userFrom.email, userTo.email, textToSend, "", "");
            dataBaseMessagesManager.addNewConversation(message);
            dataBaseMessagesManager.addNonReadMessages(message);
            
            // Je dois dessiner le message sur l'ecran de l'emetteur du message:
            List<Message> messageToDisplay = new List<Message>();
            messageToDisplay.Add(message);
            displayAllConvos(messageToDisplay);
        }

        /*
         * Cette fonction prend une liste de message en parametre et dessine les differents
         * messages sur l'ecran du user. Dependemment de l'emetteur et le recepteur le message
         * se dessine de facon differente sur l'ecran.
         */
        public void displayAllConvos(List<Message> messageToDisplayOnScreen)
        {
            var layoutUser = containerMessages; // layout sur lequel on dessine les messages
            
            if (messageToDisplayOnScreen.Count > 0)
            {
                foreach (var message in messageToDisplayOnScreen)
                {
                    Frame frameMessage = new Frame()
                    {
                        HasShadow = false,
                        Content = new Label()
                        {
                           Text = message.message,
                           HorizontalOptions = LayoutOptions.FillAndExpand,
                           VerticalOptions = LayoutOptions.FillAndExpand,
                           TextColor = Color.Black
                        }
                    };
                    if (message.fromEmail == userFrom.email)
                    {
                        frameMessage.BackgroundColor = Color.LightPink;
                        frameMessage.HorizontalOptions = LayoutOptions.EndAndExpand;
                        frameMessage.Margin = new Thickness(40, 5, 5, 5);
                    }
                    else
                    {
                        frameMessage.BackgroundColor = Color.LightGray;
                        frameMessage.HorizontalOptions = LayoutOptions.StartAndExpand;
                        frameMessage.Margin = new Thickness(5, 5, 40, 5);
                    }
                    layoutUser.Children.Add(frameMessage);

                }
                
            }
        }

        
    }
}