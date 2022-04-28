using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using HangTogether.ServerManager;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HangTogether
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Contacts : ContentPage
    {
        private User userGoingThroughHisContacts;
        
                private bool _isRefreshing;  
                private Command _refreshViewCommand;  
                
        public Contacts(User userConsultingContacts)
        {
            InitializeComponent();
            this.userGoingThroughHisContacts = userConsultingContacts;
            getListUserInContactWithMe(userGoingThroughHisContacts);
        }
        
        public bool IsRefreshing  
        {  
            get => _isRefreshing; 
        }  
  
        public Command RefreshViewCommand  
        {  
            get  
            {  
                return _refreshViewCommand ?? (_refreshViewCommand = new Command(() =>  
                {  
                    this.getListUserInContactWithMe(userGoingThroughHisContacts);  
                }));  
            }  
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
                StackLayout finalStack = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    Margin = new Thickness(15,10,15,10),
                    Padding = new Thickness(0)
                };

                AvatarView avatar = new AvatarView()
                {
                    Text = user.nom.Substring(0,1),
                    Color = CreateColor(),
                    TextColor = Color.White,
                    Padding = 0,
                    FontAttributes = FontAttributes.Bold,
                    HeightRequest = 20,
                    WidthRequest = 20,
                    VerticalOptions = LayoutOptions.Center
                };
                
                StackLayout stack = new StackLayout()
                {
                    Orientation = StackOrientation.Vertical,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                };

                Label labelNom = new Label()
                {
                    Text = user.nom + " " + user.prenom,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalTextAlignment = TextAlignment.Center,
                    TextColor = Color.Black,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 23
                };

                Label labelNouveauMessage = new Label()
                {
                    Text = "Nouveau(x) Message(s): "+nbNouveauxMessages,
                    TextColor = Color.Gray,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalTextAlignment = TextAlignment.Center
                };

                if (avatar.Color == Color.White)
                {
                    avatar.TextColor = Color.Black;
                }

                BoxView monSeparateurBottom = new BoxView()
                {
                    Color = Color.Gray, VerticalOptions = LayoutOptions.FillAndExpand, HeightRequest = 1
                };
                stack.Children.Add(labelNom); stack.Children.Add(labelNouveauMessage);
                stack.Children.Add(monSeparateurBottom);
                
                finalStack.Children.Add(avatar);
                finalStack.Children.Add(stack);
                
                
                // TapEvent on frame :
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += (s, e) => {
                    // handle the tap
                    test(user);
                     // Navigation.PushAsync(new DisplayMessages(userGoingThroughHisContacts,user));
                     //Navigation.RemovePage(Navigation.NavigationStack[1]);
                };
                stack.GestureRecognizers.Add(tapGestureRecognizer);

                layout.Children.Add(finalStack);
            }
        }

        public async void test(User user)
        {
            await Navigation.PushAsync(new DisplayMessages(userGoingThroughHisContacts,user));

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
        
        
        /*
         * Generateur de random couleur pour mes avatars
         */
        private static Color CreateColor()
        {
            var rand = new Random();
            Color c = Color.FromRgb(rand.Next(256), rand.Next(256), rand.Next(256));
            return c;
        }
        
                /*
         * Fonction qui s'occupe de l'apparition du menu
         * sur l'ecran
         * SRC: http://xamaringuyshow.com/2020/06/21/xamarin-forms-bottom-slider/
         */

        async void OnTapMenu(Object o, EventArgs e)
        {
           var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
           if (frameMenu.HeightRequest == 0)
           {
               Action<double> callback = input => frameMenu.HeightRequest = input;
               double startHeight = 0;
               double endHeight = /*mainDisplayInfo.Height*/Application.Current.MainPage.Height/3;
               uint rate = 32;
               uint length = 500;
               Easing easing = Easing.CubicOut;
               frameMenu.Animate("anim", callback, startHeight, endHeight, rate, length, easing);
               return;
           }
           else
           {
               Action<double> callback = input => frameMenu.HeightRequest = input;
               double startHeight = /*mainDisplayInfo.Height*/ Application.Current.MainPage.Height/3;
               double endiendHeight = 0;
               uint rate = 32;
               uint length = 500;
               Easing easing = Easing.SinOut;
               frameMenu.Animate("anim", callback, startHeight, endiendHeight, rate, length, easing);
           }
           
        }
        
        

        
        
         
         /*
          * Dans ces 4 prochaines fonctions , je gere lorsque le
          * user clique sur un element du menu
          */
          void OnTapFindFriends(object o, EventArgs e)
         {
             ProfilUser.GestionClickMenu("pote", userGoingThroughHisContacts);
         }
           void OnTapChooseInterests(object o, EventArgs e)
         {
             ProfilUser.GestionClickMenu("loisirs",userGoingThroughHisContacts);
         }
           void OnTapViewMessages(object o, EventArgs e)
         {
             ProfilUser.GestionClickMenu("messages",userGoingThroughHisContacts);
         }
         async void OnTapDeactivateAccount(object o, EventArgs e)
         {
             bool desactiverCompte = await DisplayAlert ("Desactivation Compte", "Etes vous sur de vouloir desactiver votre compte", "Oui", "Non");
             if (desactiverCompte)
             {
                 ProfilUser.GestionClickMenu("delete",userGoingThroughHisContacts);
             }
         }


    }
}