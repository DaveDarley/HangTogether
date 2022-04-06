using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using HangTogether.ServerManager;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HangTogether
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignUpUser : ContentPage
    {
        public SignUpUser()
        {
            InitializeComponent();
        }
        
        /*
         * Fonction qui s'assure que l'email rentre par le user
         * lors de son enregistrement n'est pas deja en cours d'utilisation
         * Si infos correct, on envoie le user a la page de recherche/modification loisirs
         */
        async void onClickSignUp(Object o, EventArgs e)
        {
            if (validateInfosUser())
            {
                DataBaseManager dataBaseManager = new DataBaseManager();
                var allUser = await dataBaseManager.GetAllUsers();
                if (dataBaseManager.isEmailAlreadyInUsage(this.email.Text, allUser))
                {
                    this.emailError.Text = "Il existe deja un compte avec ce courriel";
                    this.emailError.IsVisible = true;
                }
                else
                {
                    Byte[] saltToEncryptMdp = SecureMdp.getSaltForEncryption();
                    string saltToEncryptMdpToSaveInDB = SecureMdp.byteArraySaltToString(saltToEncryptMdp);
                    string hashedMdp = SecureMdp.encryptPassword(this.mdp.Text, saltToEncryptMdp);
                    User user = new User(this.nom.Text, this.prenom.Text, this.email.Text, hashedMdp,"","","",saltToEncryptMdpToSaveInDB);
                    
                    await dataBaseManager.AddUser(user);
                    Application.Current.MainPage = new NavigationPage(new ChooseAndModifyInterests(user));
                }
            }
        }
        

        
        /*
         * Fonction qui verifie si les infos rentrées par le user
         * lors de son enregistrement sont dans le bon FORMAT
         */
        public bool validateInfosUser()
        {
            
            var nomUser = this.nom.Text;
            var prenomUser = this.prenom.Text;
            var emailUser = this.email.Text;
            var mdpUser = this.mdp.Text;
            var dob = this.startDatePicker;
            var isInfosValid = true;
 

            if (String.IsNullOrEmpty(nomUser))
            {
                this.nomError.IsVisible = true;
                isInfosValid = false;
            }
            else
            {
                this.nomError.IsVisible = false;
            }

            if (String.IsNullOrEmpty(prenomUser))
            {
                this.prenomError.IsVisible = true;
                isInfosValid = false;
            }
            else
            {
                this.prenomError.IsVisible = false;
            }

            
            if (!IsValidEmail(emailUser))
            {
                this.emailError.Text = "Le format de votre courriel est invalide";
                this.emailError.IsVisible = true;
                isInfosValid = false;
            }
            else
            {
                this.emailError.IsVisible = false;
            }


            if (String.IsNullOrEmpty(mdpUser))
            {
                this.mdpError.IsVisible = true;
                isInfosValid = false;
            }
            else
            {
                if (mdpUser.Length < 8)
                {
                    this.mdpError.Text = "Mot de passe doit contenir plus de 8 caracteres";
                    this.mdpError.IsVisible = true;
                    isInfosValid = false;
                }
                else
                {
                    this.mdpError.IsVisible = false;
                }
            }
            
            dob.SetValue (DatePicker.MaximumDateProperty, DateTime.Now);

            return isInfosValid;
        }

        
        // Src:https://docs.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format
        // public bool validateEmailFormat(string email)
        // {
        //     if (string.IsNullOrWhiteSpace(email))
        //         return false;
        //
        //     try
        //     {
        //         // Normalize the domain
        //         email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
        //             RegexOptions.None, TimeSpan.FromMilliseconds(200));
        //
        //         // Examines the domain part of the email and normalizes it.
        //         string DomainMapper(Match match)
        //         {
        //             // Use IdnMapping class to convert Unicode domain names.
        //             var idn = new IdnMapping();
        //
        //             // Pull out and process domain name (throws ArgumentException on invalid)
        //             string domainName = idn.GetAscii(match.Groups[2].Value);
        //
        //             return match.Groups[1].Value + domainName;
        //         }
        //     }
        //     catch (RegexMatchTimeoutException e)
        //     {
        //         return false;
        //     }
        //     catch (ArgumentException e)
        //     {
        //         return false;
        //     }
        //
        //     try
        //     {
        //         return Regex.IsMatch(email,
        //             @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        //             RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        //     }
        //     catch (RegexMatchTimeoutException)
        //     {
        //         return false;
        //     }
        // }
        
        // Src: https://stackoverflow.com/questions/1365407/c-sharp-code-to-validate-email-address
        bool IsValidEmail(string email)
        {
            if (String.IsNullOrEmpty(email))
            {
                return false;
            }

            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith(".")) {
                return false; // suggested by @TK-421
            }
            try {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch {
                return false;
            }
        }





    }
}