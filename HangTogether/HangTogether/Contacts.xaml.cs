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
            //await this.DisplayToastAsync("l: "+usersInContactWithMe.Count,5000);

            displayAllContacts(usersInContactWithMe);
        }

        public async void displayAllContacts(List<User> usersInContactWithMe)
        {
            var layout = this.containerContacts;
            foreach (var user in usersInContactWithMe)
            {
                StackLayout stack = new StackLayout()
                {
                    Orientation = StackOrientation.Vertical,
                    BackgroundColor = Color.DarkGray,
                    Margin = new Thickness(0)
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
                    Text = "Nouveau(x) Message(s): "+0,
                    TextColor = Color.LawnGreen,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalTextAlignment = TextAlignment.Center
                };
                
                stack.Children.Add(labelNom); stack.Children.Add(labelNouveauMessage);
                
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

        // public async void onTapContacts(Object s, EventArgs e,User userSendingMessages, User userReceivingMessages)
        // {
        //     await Navigation.PushAsync(new DisplayMessages(userSendingMessages,userReceivingMessages));
        // }

    }
}