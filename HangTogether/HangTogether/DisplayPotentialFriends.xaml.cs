using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HangTogether.ServerManager;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;
using Xamarin.Forms.Xaml;

namespace HangTogether
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DisplayPotentialFriends : ContentPage
    {
        public ObservableCollection<DisplayUser> _userToDisplayOnCard = new ObservableCollection<DisplayUser>();
        private User userLookingForNewFriends;

        public DisplayPotentialFriends(User user)
        {
            InitializeComponent();
            BindingContext = this;
            userLookingForNewFriends = user;
            CardBinding();
        }

        
        /*
         * Fonction qui retourne les loisirs du user qui recherche
         * des nouveaux amis sous forme de tableau de string 
         */
        // public string[] getInterestUserLookingForNewFriends()
        // {
        //     return userLookingForNewFriends.loisirs.Contains(',')
        //         ? userLookingForNewFriends.loisirs.Split(',')
        //         : new []{userLookingForNewFriends.loisirs};
        // }

        /*
         * Qd on clique sur Envoyez un message;
         * je cherche quel user est actuelement sur l'ecran(CarouselView.CurrentItem)
         * et a partir de la je trouve le User en question 
         */
        async void OnTapSendMessage(Object o, EventArgs e)
        {
            DisplayUser userInDisplay = TheCarousel.CurrentItem as DisplayUser;
            User userToSendMessage = userInDisplay.user;
            
            await Navigation.PushAsync(new DisplayMessages(this.userLookingForNewFriends,userToSendMessage));
        }

        
        /*
         * Fonction qui s'occupe de l'apparition du menu
         * sur l'ecran
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
                this.stackBgMenu.IsEnabled = false;
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
                this.stackBgMenu.IsEnabled = true;
            }
        }
        
        
        /*
         * Dans ces 4 prochaines fonctions , je gere lorsque le
         * user clique sur un element du menu
         */
         void OnTapFindFriends(object o, EventArgs e)
        {
            ProfilUser.GestionClickMenu("pote", userLookingForNewFriends);
        }
         void OnTapChooseInterests(object o, EventArgs e)
        {
            ProfilUser.GestionClickMenu("loisirs", userLookingForNewFriends);
        }
         void OnTapViewMessages(object o, EventArgs e)
        {
          ProfilUser.GestionClickMenu("messages",userLookingForNewFriends);
        }
        async void OnTapDeactivateAccount(object o, EventArgs e)
        {
            bool desactiverCompte = await DisplayAlert ("Desactivation Compte", "Etes vous sur de vouloir desactiver votre compte", "Oui", "Non");
            if (desactiverCompte)
            {
                ProfilUser.GestionClickMenu("delete",userLookingForNewFriends);
            }
        }
        
        /*
         * Fonction qui s'occupe de baisser le menu lorsque le user clique n'importe ou
         * sur l'ecran:
         * Si GridLoisirs.IsEnabled = false c-a-d mon menu est sur l'ecran alors je descends
         * le menu est j'active les interactions avec les users
         */
        async void OnTapBg(Object o, EventArgs e)
        {
            if (!this.stackBgMenu.IsEnabled)
            {
                DisplayAlert("test", "g clique sur l'ecran pr baisser le menu", "ok");
                Action<double> callback = input => frameMenu.HeightRequest = input;
                double startHeight = Application.Current.MainPage.Height/3;
                double endiendHeight = 0;
                uint rate = 32;
                uint length = 500;
                Easing easing = Easing.SinOut;
                frameMenu.Animate("anim", callback, startHeight, endiendHeight, rate, length, easing);
                this.stackBgMenu.IsEnabled = true;
            }
        }
        



        /*
         *Fonction qui recupere tous les users de ma DB et pour chaque user
         * on parcours sa liste de loisirs. Si ce user a des loisirs en communs
         * avec l'user qui recherche de nouveaux potes (user du constructeur de la classe)
         * On enregistre ce user mais aussi les loisirs qu'il a en commun avec l'utilisateur
         * qui recherche de nouveaux potes.
         * Lorsqu'on compare les loisirs on transforme les string en LOWERCASE
         * Si aucun utilisateur n'a pas de loisirs en commun avec le user qui recherche de nouveau pote
         * on lui affiche une alert et on lui renvoie a la page "ChooseAndModifyInterests()"
         */
        public async void CardBinding()
        {
            DataBaseManager dataBaseManager = new DataBaseManager();
            List<User>userWithSharedInterests = await dataBaseManager.getUserWithSharedInterests(userLookingForNewFriends);
            
            List<Loisir> interestsUserLookingForNewFriends = await dataBaseManager.getInterestsUser(userLookingForNewFriends);
            List<string> interestUserSearchingNewFriends = new List<string>();
            foreach (var loisir in interestsUserLookingForNewFriends)
            {
                interestUserSearchingNewFriends.Add(loisir.nom);
            }

            if (userWithSharedInterests.Count > 0)
            {
                foreach (var user in userWithSharedInterests)
                {
                    List<string> loisirsEnCommun = new List<string>();
                    string titre = user.nom + " " + user.prenom;
                    string anecdotes = user.anecdotes;

                    List<Loisir> loisirsUserWithSharedInterests = await dataBaseManager.getInterestsUser(user);
                    List<string> interestUserPossibleFriend = new List<string>();
                    foreach (var loisir in loisirsUserWithSharedInterests)
                    {
                        interestUserPossibleFriend.Add(loisir.nom);
                    }
                    
                    foreach (var loisir in interestUserPossibleFriend)
                    {
                        if (interestUserSearchingNewFriends.Contains(loisir))
                        {
                            loisirsEnCommun.Add(loisir);
                        }
                    }
                    DisplayUser userToDisplayOnCard = new DisplayUser(user,titre, loisirsEnCommun, anecdotes);
                    _userToDisplayOnCard.Add(userToDisplayOnCard);
                }
            }
            else
            {
                await DisplayAlert("Recherche nouveau pote",
                    "Aucun utilisateur a les memes interets que toi, Essaie d'elargir ta liste d'interets", "OK");

                await Application.Current.MainPage.Navigation.PopAsync();
                
                /*  await Navigation.PushAsync(new ChooseAndModifyInterests(userLookingForNewFriends));
                  // on enleve page de DisplayPotentialFriends de la liste de NAVIGATIONPAGE
                  Navigation.RemovePage(Navigation.NavigationStack[1]); */
            }



        }
       
        
        public ObservableCollection<DisplayUser> Profile
        {
            get => _userToDisplayOnCard;
            set
            {
                _userToDisplayOnCard = value;
            }
        }
        public class DisplayUser
        {
            public DisplayUser(User user,string titre, List<string>sharedInterests,string anecdotes)
            {
                this.titre = titre;
                this.sharedInterests = sharedInterests;
                this.anecdotes = anecdotes;
                this.user = user;
            }
            public string titre { get; set; }
            public List<string> sharedInterests { get; set; }
            public string anecdotes { get; set; }

            public User user { get; set; }

        }



    }
}