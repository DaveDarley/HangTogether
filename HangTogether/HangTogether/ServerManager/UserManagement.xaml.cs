using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HangTogether.ServerManager
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserManagement : ContentPage
    {
       // FirebaseClient monDB = new FirebaseClient("https://anodate-ca8b9-default-rtdb.firebaseio.com/");
        public UserManagement()
        {

            InitializeComponent();
        }

        public async void addUserToDB(string Nom, string Prenom, string mail, string mdp)
        {
           // FirebaseClient monDB = new FirebaseClient("https://anodate-ca8b9-default-rtdb.firebaseio.com/");

           // SignUpUser lala = new SignUpUser();
           // lala.getEmail().IsVisible = true;
           
            await DisplayAlert("Alert", "FuckYou", "OK");
           // Application.Current.MainPage = new NavigationPage(new ChooseAndModifyInterests());

            //var mesUsers =  getAllUsers();
            //var isEmailInUse = isEmailAlreadyInUse(mail, mesUsers);
            //
            //await DisplayAlert ("Alert", "You have been alerted: "+isEmailInUse, "OK");
            //
            // Console.WriteLine(isEmailInUse);
            //
            // if (!isEmailInUse)
            // {
            //     SignUpUser lala = new SignUpUser();
            //     lala.getEmail().IsVisible = false; 
            //     await monDB
            //         .Child("Users")
            //         .PostAsync(new User() {nom = Nom, prenom = Prenom, email = mail, mdp = mdp});
            //     Application.Current.MainPage = new NavigationPage(new ChooseAndModifyInterests());
            // }
            // else
            // {
            //     SignUpUser lala = new SignUpUser();
            //     lala.getEmail().Text = "Email deja en utilisation";
            //     lala.getEmail().IsVisible = true;
            // }
        }

        public /*async Task<List<User>>*/ ObservableCollection<User> getAllUsers()
        {
             FirebaseClient monDB = new FirebaseClient("https://anodate-ca8b9-default-rtdb.firebaseio.com/");

          /* return (await monDB
                .Child("Users")
                .OnceAsync<User>()).Select(item => new User
            {
                email = item.Object.email
            }).ToList();*/
          var users = monDB
              .Child("Users")
              .AsObservable<User>()
              .AsObservableCollection();
          return users;
        }

        public bool isEmailAlreadyInUse(string email, ObservableCollection<User> mesUsers)
        {
            foreach (var user in mesUsers)
            {
                if (String.Equals(user.email, email))
                {
                    return true;
                }
            }
            return false;
        }
        
        
        
        
    }
}