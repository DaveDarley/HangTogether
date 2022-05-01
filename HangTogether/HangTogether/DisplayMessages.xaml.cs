using System;
using System.Collections.Generic;
using Google.Cloud.Firestore;
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
            listenOnNewMessages();
        }

        /*
         * A chaque nouveau message ajouté a ma table "Messages", ce listener sera lancé
         * et donc pour le user recepteur du message on met l'attribut lu = "y";
         * Vu que le champ lu du message a changé, on le met aussi a jour dans la BD
         */
        public async void listenOnNewMessages()
        {
            FirestoreDb db = FirestoreDb.Create("https://anodate-ca8b9-default-rtdb.firebaseio.com/");
            CollectionReference citiesRef = db.Collection("Messages");
            Query query = db.Collection("Messages");

            FirestoreChangeListener listener = query.Listen(snapshot =>
            {
                List<Message> messagesRecu = new List<Message>();
                foreach (DocumentSnapshot documentSnapshot in snapshot.Documents)
                {
                    Message message = documentSnapshot.ConvertTo<Message>();
                    if (message.toEmail == userFrom.email && message.fromEmail == userTo.email)
                    {
                        message.lu = "y";
                        dataBaseMessagesManager.updateConversation(message);
                        messagesRecu.Add(message);
                        displayAllConvos(messagesRecu);
                    }
                }
            });
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
        }
        
        /*
         * Qd utilisateur clique sur send, on envoie le message dans la BD et on
         * l'affiche sur l'ecran de l'emetteur du message
         */
        private void  OnSendMessage(Object sender, EventArgs e)
        {
            DataBaseMessagesManager dataBaseMessagesManager = new DataBaseMessagesManager();
            var textToSend = this.message.Text;
            var dateTime = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
            Message message = new Message(userFrom.email, userTo.email, textToSend, "", dateTime,"n");
            dataBaseMessagesManager.addNewConversation(message);
            
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
                    // put scrollview at the bottom the last message not working quite well
                }
                await this.scrollMessages.ScrollToAsync(lastFrameToScrollTo,ScrollToPosition.MakeVisible,true);

               // await this.scrollMessages.ScrollToAsync(0,scrollMessages.Content.Height,true);

            }
        }

        
        /*
         * Function qui gere lorsque le user clique sur la fleche de retour
         * PK: Je veux update (si necessaire) les infos de cette page
         * lorsque user retourne sur la page.
         *
         * Prob: Fonction juste sur android
         */
        
        // protected override bool OnBackButtonPressed()
        // {
        //     Device.BeginInvokeOnMainThread(async () =>
        //     {
        //         //aTimer.Stop();
        //         updateStatusUser("n");
        //         DisplayAlert("Status user",
        //             "J'ai quitte messages , alors mon status doit etre: " + userFrom.isUserReadMessage, "ok");
        //         Application.Current.MainPage = new NavigationPage(new Contacts(userFrom));
        //     });
        //
        //     return true;
        // }
        
        //https://stackoverflow.com/questions/57662491/detect-back-arrow-press-of-the-navigationpage-in-xamarin-forms
        // protected override void OnDisappearing()
        // {
        //     base.OnDisappearing();
        //     updateStatusUser("n");
        //     Application.Current.MainPage = new NavigationPage(new Contacts(userFrom));
        // }

        
    }
}