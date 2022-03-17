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
        }

        public void CardBinding()
        {
            _Profiles.Add(new UserProfile(FirestName = "Dave", LastName = ));
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
        }
    }
}