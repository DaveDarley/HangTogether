using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HangTogether.ServerManager;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HangTogether
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Contacts : ContentPage
    {
        private User userGoingThroughHisContacts;
        public Contacts(User userConsultingContacts)
        {
            InitializeComponent();
            this.userGoingThroughHisContacts = userConsultingContacts;
            getListUserInContactWithMe(userGoingThroughHisContacts);
        }

        public async void getListUserInContactWithMe(User userLookingInContacts)
        {
            DataBaseMessagesManager dataBaseMessagesManager = new DataBaseMessagesManager();
            List<User> usersInContactWithMe =
                await dataBaseMessagesManager.getUserInContactsWithMe(userLookingInContacts);
            displayAllContacts(usersInContactWithMe);
        }

        /*
         * Fonction qui prend en parametre une liste de User avec lequel un user
         * en particulier est en contact et affiche les infos des differents users
         * sur l'ecran
         */
        public async void displayAllContacts(List<User> usersInContactWithMe)
        {
            var layout = this.containerContacts;
            foreach (var user in usersInContactWithMe)
            {
                var nbNouveauxMessages = await getNumberOfNewMessages(user, userGoingThroughHisContacts);
                StackLayout stack = new StackLayout()
                {
                    Orientation = StackOrientation.Vertical,
                    BackgroundColor = Color.White,
                    Margin = new Thickness(0),
                    Padding = new Thickness(0)
                };

                Label labelNom = new Label()
                {
                    Text = user.nom + " " + user.prenom,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalTextAlignment = TextAlignment.Center,
                    TextColor = Color.Black
                };

                Label labelNouveauMessage = new Label()
                {
                    Text = "Nouveau(x) Message(s): "+nbNouveauxMessages,
                    TextColor = Color.LawnGreen,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalTextAlignment = TextAlignment.Center
                };

                BoxView monSeparateurTop = new BoxView()
                {
                    Color = Color.Black, VerticalOptions = LayoutOptions.FillAndExpand, HeightRequest = 1
                };
                
                BoxView monSeparateurBottom = new BoxView()
                {
                    Color = Color.Black, VerticalOptions = LayoutOptions.FillAndExpand, HeightRequest = 1
                };
                
                stack.Children.Add(monSeparateurTop);
                stack.Children.Add(labelNom); stack.Children.Add(labelNouveauMessage);
                stack.Children.Add(monSeparateurBottom);
                
                // TapEvent on frame :
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += (s, e) => {
                    // handle the tap
                    Navigation.PushAsync(new DisplayMessages(userGoingThroughHisContacts,user));
                };
                stack.GestureRecognizers.Add(tapGestureRecognizer);

                layout.Children.Add(stack);
            }
        }

        
        public async Task<int> getNumberOfNewMessages(User userSendingMessage, User userGoingThroughContacts)
        {
            int nbNouveauxMessages = 0;
            DataBaseMessagesManager dataBaseMessagesManager = new DataBaseMessagesManager();
            List<Message> nouveauxMessagesNonLu = await dataBaseMessagesManager.GetAllMessages("Nouveaux Messages");
            foreach (var message in nouveauxMessagesNonLu)
            {
                if (message.fromEmail == userSendingMessage.email && message.toEmail == userGoingThroughContacts.email)
                {
                    nbNouveauxMessages++;
                }
            }

            return nbNouveauxMessages;
        }


    }
}