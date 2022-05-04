using System;
using System.Collections.Generic;
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
                var trimEmail = this.email.Text.Trim();
                User user = await dataBaseManager.getUser(trimEmail);
                if (!(user is null))
                {
                    this.emailError.Text = "Il existe deja un compte avec ce courriel";
                    this.emailError.IsVisible = true;
                }
                else
                {
                    // Deuxieme Verfication user:
                    string codeVerifUserToCheck = VerificationEmail.verifEmail(trimEmail);
                    string codeVerifUser = await DisplayPromptAsync("Vérification Email", "Veuillez entrer le code recu par courriel");
                    if (!String.IsNullOrEmpty(codeVerifUser) && codeVerifUserToCheck == codeVerifUser) // user correct
                    {
                        // Encryption du mot de passe du user pour ne pas l'enregistrer tel quel dans la BD
                        Byte[] saltToEncryptMdp = SecureMdp.getSaltForEncryption();
                        string saltToEncryptMdpToSaveInDB = SecureMdp.byteArraySaltToString(saltToEncryptMdp);
                        string hashedMdp = SecureMdp.encryptPassword(this.mdp.Text, saltToEncryptMdp);
                        
                        User userCree = new User(this.nom.Text, this.prenom.Text, trimEmail, hashedMdp,"",saltToEncryptMdpToSaveInDB,"");
                        await dataBaseManager.AddUser(userCree);
                        
                        // Pk: Qd on ajoute le user dans la bd son champs id est vide
                        User nouveauUser = await dataBaseManager.getUser(userCree.email);
                        Application.Current.MainPage = new NavigationPage(new ChooseAndModifyInterests(nouveauUser));
                    
                    }
                    else
                    {
                        await DisplayAlert ("Vérification courriel", "Erreur lors de la vérification de votre courriel, veuillez réessayer", "OK");
                    }

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
            string codeVerifUser;
 

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