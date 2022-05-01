using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HangTogether.ServerManager;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HangTogether
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ForgottenPassword : ContentPage
    {
        private RecoverPasswordUser utilisateurQuiModifieSonMdp;
        public ForgottenPassword(RecoverPasswordUser userToRecoverPassword)
        {
            InitializeComponent();
            this.utilisateurQuiModifieSonMdp = userToRecoverPassword;
        }
        
        /*
         * Fonction qui valide le format des informations entrees par le
         * user (S'assure champ code de verif pas vide mais aussi mdp a au moins 8 caracteres)
         */
        public bool verifRecoveryPassword()
        {

            var codeVerif = this.verifCode.Text;
            var nouveauMdpUser = this.nouveauMdp.Text;
            bool didUserEnterInfos = true;
 

            // User rentre pas code verif et clique sign In
            if (String.IsNullOrEmpty(codeVerif))
            {
                this.codeVerifError.IsVisible = true;
                this.codeVerifError.Text = "Veuillez entrer le code de verification qui vous a ete fourni par courriel";
                didUserEnterInfos = false;
            }
            else // pas vide ou null alors on va valider par la base de donnée
            {
                this.codeVerifError.IsVisible = false;
            }

            if (String.IsNullOrEmpty(nouveauMdpUser))
            {
                this.nouveauMdpError.IsVisible = true;
                didUserEnterInfos = false;
            }
            else 
            {
                if (this.nouveauMdp.Text.Length < 8)
                {
                    this.nouveauMdpError.Text = "Votre mot de passe doit contenir au moins 8 caracteres";
                    this.nouveauMdpError.IsVisible = true;
                    didUserEnterInfos = false;
                }
                else
                {
                    this.nouveauMdpError.IsVisible = false;
                }
            }
            return didUserEnterInfos;
        }

        
        /*
         * Fonction qui valide que le code de verification entre par le user
         * est bien qui lui a ete envoye par courriel
         */
        async void signInRecoverPassword(Object s, EventArgs e)
        {
            if (verifRecoveryPassword())
            {
                DataBaseManager dataBaseManager = new DataBaseManager();
                var verifCodeEnterByUser = this.verifCode.Text;
                var verifCodeSendToUser = utilisateurQuiModifieSonMdp.verifCode;
                var emailUserRecoveringPassword = utilisateurQuiModifieSonMdp.email;
                User toUpdate = await dataBaseManager.getUser(emailUserRecoveringPassword);
                
                
                // C'est bien un user valide qui change son mdp
                if (verifCodeSendToUser == verifCodeEnterByUser)
                {
                    
                    Byte[] saltToEncryptMdp = SecureMdp.getSaltForEncryption();
                    string saltToEncryptMdpToSaveInDB = SecureMdp.byteArraySaltToString(saltToEncryptMdp);
                    string hashedMdp = SecureMdp.encryptPassword(this.nouveauMdp.Text, saltToEncryptMdp);
                    
                    // change mdp de ce user dans l'autre table
                    toUpdate.mdp = hashedMdp;
                    toUpdate.saltToEncryptMdp = saltToEncryptMdpToSaveInDB;
                    await dataBaseManager.UpdateUser(toUpdate);
                    Application.Current.MainPage = new NavigationPage(new ChooseAndModifyInterests(toUpdate));
                }
                else
                {
                    this.codeVerifError.Text = "Le code de verification rentré est incorrect";
                    this.codeVerifError.IsVisible = true;
                }
                
            }
        }

        async void goSignUp(Object s, EventArgs e)
        {
            
            await Navigation.PushAsync(new SignUpUser());
            // Si une personne passe de mdp oublie a sign up, on lui empeche
            // de revenir a la page mdp oublie, de preference il reviendra 
            // a la page sign in
            Navigation.RemovePage(Navigation.NavigationStack[1]);
        }
        
    }
}