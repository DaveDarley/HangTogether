using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HangTogether.ServerManager;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HangTogether
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DisplayPotentialFriends : ContentPage
    {
        public ObservableCollection<UserProfile> _Profiles = new ObservableCollection<UserProfile>();

        public bool isMenuOpen = false;
        public DisplayPotentialFriends(User user)
        {
            InitializeComponent();
            CardBinding();
            BindingContext = this;
            
            // rentrer le menu en bas de l'ecran
            InvisibleMenu();
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
        
        // Test:
        public void CardBinding()
        {
            _Profiles.Add(new UserProfile()
                {
                    FirstName = "Dave", LastName = "Dave",Age="18",
                    Anecdote = "Je m'appelle dave, je vis en californie, j'aime aller a la plage, j'etudies en informatique a UdEm",
                    Titre = "Dave Darley Joseph, 18"
                    
                }
            );
            _Profiles.Add(new UserProfile()
                {
                    FirstName = "Dave", LastName = "Dave",Age="18",
                    Anecdote = "Je m'appelle dave, je vis en californie, j'aime aller a la plage, j'etudies en informatique a UdEm" +
                               "j'aime programmer en java, c. T'aimes programmer, viens on se connecte",
                    Titre = "Dave Darley Joseph, 18"
                }
            );
            _Profiles.Add(new UserProfile()
                {
                    FirstName = "Dave", LastName = "Dave",Age="18",
                    Anecdote = "Je m'appelle dave, je vis en californie, j'aime aller a la plage, j'etudies en informatique a UdEm",
                    Titre = "Dave Darley Joseph, 18"
                }
            );
            _Profiles.Add(new UserProfile()
                {
                    FirstName = "Dave", LastName = "Dave",Age="18",
                    Anecdote = "Je m'appelle dave, je vis en californie, j'aime aller a la plage, j'etudies en informatique a UdEm",
                    Titre = "Dave Darley Joseph, 18"
                }
            );
            _Profiles.Add(new UserProfile()
                {
                    FirstName = "Dave", LastName = "Dave",Age="18",
                    Anecdote = "Je m'appelle dave, je vis en californie, j'aime aller a la plage, j'etudies en informatique a UdEm",
                    Titre = "Dave Darley Joseph, 18"
                }
            );
            _Profiles.Add(new UserProfile()
                {
                    FirstName = "Dave", LastName = "Dave",Age="18",
                    Anecdote = "Je m'appelle dave, je vis en californie, j'aime aller a la plage, j'etudies en informatique a UdEm",
                    Titre = "Dave Darley Joseph, 18"
                }
            );
            _Profiles.Add(new UserProfile()
                {
                    FirstName = "Dave", LastName = "Dave",Age="18",
                    Anecdote = "Je m'appelle dave, je vis en californie, j'aime aller a la plage, j'etudies en informatique a UdEm",
                    Titre = "Dave Darley Joseph, 18"
                }
            );
            _Profiles.Add(new UserProfile()
                {
                    FirstName = "Dave", LastName = "Dave",Age="18",
                    Anecdote = "Je m'appelle dave, je vis en californie, j'aime aller a la plage, j'etudies en informatique a UdEm",
                    Titre = "Dave Darley Joseph, 18"
                }
            );
        }

        public ObservableCollection<UserProfile> Profile
        {
            get => _Profiles;
            set
            {
                _Profiles = value;
            }


        }

        public class UserProfile
        {
            
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Age { get; set; }
            public string Anecdote { get; set; }

            public string Titre { get; set; }
        }
    }
}