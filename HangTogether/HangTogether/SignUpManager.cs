using System;
using System.Threading.Tasks;
using Firebase.Database;
using Xamarin.Forms;

namespace HangTogether
{
    public class SignUpManager 
    {
        


        public async void  addNewuserToDB(string nom, string prenom, DatePicker dob, string email, string mdp)
        {
            Console.Write("Je suis entre ici");
            // FirebaseClient firebaseClient = new FirebaseClient("https://anodate-ca8b9-default-rtdb.firebaseio.com/");
            // // Si ressource Users existe pas encore??
            // await firebaseClient  
            //     .Child("Users")  
            //     .PostAsync((new Users() { nom = nom, prenom = prenom , dob = dob, email = email, mdp = mdp}).ToString());
        }

        public bool isEmailInUsage(string email)
        {
            // parcours lists users et verifier si email deja en cours d'utilisation
            return true;
        }

     
        

    }
}