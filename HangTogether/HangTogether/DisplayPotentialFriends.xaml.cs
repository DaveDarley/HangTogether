using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HangTogether.ServerManager;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;
using Xamarin.Forms.Xaml;

namespace HangTogether
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DisplayPotentialFriends : ContentPage
    {
        public List<User> userWithSharedinterests = new List<User>();
        public ObservableCollection<DisplayUser> _userToDisplayOnCard = new ObservableCollection<DisplayUser>();
        private User userLookingForNewFriends;
        
        
        public bool isMenuOpen = false;
        
        public DisplayPotentialFriends(User user)
        {
            InitializeComponent();
            BindingContext = this;
            userLookingForNewFriends = user;
            InitializePage();
            CardBinding();
            // rentrer le menu en bas de l'ecran
            InvisibleMenu();
        }
        
        
        public async void InitializePage()
        {
            DataBaseManager dataBaseManager = new DataBaseManager();
            var allUsers = await dataBaseManager.GetAllUsers();
            userWithSharedinterests = dataBaseManager.getUserWithSharedInterests(userLookingForNewFriends, allUsers);
            //await DisplayAlert("Alert","size user with shared interests " + userWithSharedinterests.Count,"accept");
        }
        
        /*
         * Fonction qui retourne les loisirs du user qui recherche
         * des nouveaux amis sous forme de tableau de string 
         */
        public string[] getInterestUserLookingForNewFriends()
        {
            return userLookingForNewFriends.loisirs.Contains(',')
                ? userLookingForNewFriends.loisirs.Split(',')
                : new []{userLookingForNewFriends.loisirs};
        }



        public void InvisibleMenu()
        {
            this.frameMenu.TranslationY +=   (this.frameMenu.HeightRequest + 50);
        }

        /*
         * Fonction qui s'occupe de l'apparition du menu
         * sur l'ecran
         */
        async void OnTapMenu(Object o, EventArgs e)
        {
            if (isMenuOpen)
            {
                await this.frameMenu.TranslateTo(0, this.frameMenu.TranslationY + this.frameMenu.HeightRequest,
                    1000);
                isMenuOpen = false;
            }
            else
            {
                await this.frameMenu.TranslateTo(0, this.frameMenu.TranslationY - this.frameMenu.HeightRequest,
                    1000);
                isMenuOpen = true;
            }
        }
        
        /*
         * Dans ces 4 prochaines fonctions , je gere lorsque le
         * user clique sur un element du menu
         */
         void OnTapFindFriends(object o, EventArgs e)
        {
          //  ProfilUser.GestionClickMenu("pote");
        }
         void OnTapChooseInterests(object o, EventArgs e)
        {
         //   ProfilUser.GestionClickMenu("loisirs");
        }
         void OnTapViewMessages(object o, EventArgs e)
        {
          //  ProfilUser.GestionClickMenu("messages");
        }
        async void OnTapDeactivateAccount(object o, EventArgs e)
        {
            bool desactiverCompte = await DisplayAlert ("Desactivation Compte", "Etes vous sur de vouloir desactiver votre compte", "Oui", "Non");
            if (desactiverCompte)
            {
                // Supprmimer user de la base de données
                Application.Current.MainPage = new NavigationPage(new LogInSignUp());
            }
        }
        
        public List<Frame> CreateLoisirsFrame(string [] loisirs)
        {
            List<Frame> frameADessinerSurlayout = new List<Frame>();
            for (int i = 0; i < loisirs.Length; i++)
            {
                Frame frame = new Frame()
                {
                    BackgroundColor = Color.White,
                    CornerRadius = 30,
                    HasShadow = true,
                    IsVisible = true,
                    Margin = new Thickness(0,4,0,4),
                    Content = new Label()
                    {
                        Text = loisirs[i],
                        TextColor = Color.Black,
                        HorizontalOptions = LayoutOptions.StartAndExpand,
                    }
                };
                frameADessinerSurlayout.Add(frame);
            }
            return frameADessinerSurlayout;
        }
        
        
        /*
         * Fonction qui prend la liste prends une liste de loisirs en string,
         * cree pour chaque loisirs une Frame et ajoute ce frame dans le champ
         * Loisirs en communs de mon "CarouselView"
         */
        
       /* public void addLoisirsToCommonInterests(List<Frame>commonInterests)
        {
            ObservableCollection<View> views = TheCarousel.VisibleViews;
            PancakeView monPancake = views[0] as PancakeView;
            Grid monGrid = monPancake.Children as Grid;
            var LayoutInterest = (FlexLayout) monGrid.Children.Where(c => Grid.GetRow(c) == 2 && Grid.GetColumn(c)==0);
            
            foreach (var interests in commonInterests)
            {
                LayoutInterest.Children.Add(interests);
            }
        }*/
       

        /*
         * Fonction qui s'occupe de mettre les informations
         * qui seront affichées sur chaque carte
         */
        public async void CardBinding()
        {
            string[] interestsUserLookingForNewFriends = getInterestUserLookingForNewFriends();
            List<User> usersWithSameInterests = userWithSharedinterests;
            await DisplayAlert("Alert","size user with shared interests " + usersWithSameInterests.Count,"accept");

            for (int i = 0; i<usersWithSameInterests.Count; i++)
            {
                // List<string> loisirsEnCommun = new List<string>();
                // string titre = user.nom + " " + user.prenom;
                // string anecdotes = user.anecdotes;
                //
                List<Frame> test = new List<Frame>();
            
               /* string[] loisirsUsers = user.loisirs.Split(',');
                foreach (var loisir in loisirsUsers)
                {
                    if (Array.Exists(interestsUserLookingForNewFriends, x => x == loisir))
                    {
                        loisirsEnCommun.Add(loisir);
                    }
                }
                List<Frame> frameOfInterestsInCommon = CreateLoisirsFrame(loisirsEnCommun.ToArray());*/
                DisplayUser userToDisplayOnCard = new DisplayUser("titre"+i, test, "anecdotes");
                _userToDisplayOnCard.Add(userToDisplayOnCard);
                
            }

            // List<Frame> test = new List<Frame>();
            // DisplayUser dis = new DisplayUser("hmm 14",test,"eeee");
            // _userToDisplayOnCard.Add(dis);

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
            public DisplayUser(string titre, List<Frame>sharedInterests,string anecdotes)
            {
                this.titre = titre;
                this.sharedInterests = sharedInterests;
                this.anecdotes = anecdotes;
            }
            public string titre { get; set; }
            public List<Frame> sharedInterests { get; set; }
            public string anecdotes { get; set; }

        }



    }
}