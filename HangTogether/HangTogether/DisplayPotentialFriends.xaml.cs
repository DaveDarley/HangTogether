using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HangTogether
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DisplayPotentialFriends : ContentPage
    {
        public ObservableCollection<UserProfile> _Profiles = new ObservableCollection<UserProfile>();
        public DisplayPotentialFriends()
        {
            InitializeComponent();
            CardBinding();
            BindingContext = this;
        }

        async void OnTapMenu(Object o, EventArgs e)
        {
        }

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