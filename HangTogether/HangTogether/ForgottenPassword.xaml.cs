using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HangTogether
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ForgottenPassword : ContentPage
    {
        public ForgottenPassword()
        {
            InitializeComponent();
        }
        
        /*
         * Fonction qui valide les infos rentrees par le user lors
         * du recouvrement de son mot de passe
         */
        public bool verifRecoveryPassword()
        {

            var codeVerif = this.verifCode.Text;
            var nouveauMdpUser = this.nouveauMdp.Text;
 
            if ( String.IsNullOrEmpty(codeVerif) || String.IsNullOrEmpty(nouveauMdpUser))
            {
                // User rentre pas code verif et clique sign In
                if (String.IsNullOrEmpty(codeVerif))
                {
                    var CodeVerifError = this.codeVerifError;
                    CodeVerifError.IsVisible = true;
                    CodeVerifError.Text = "Veuillez entrer le code de verification qui vous a ete fourni par courriel";
                }
                else // pas vide ou null alors on va valider par la base de donnée
                {
                    var CodeVerifError = this.codeVerifError;
                    CodeVerifError.IsVisible = false;
                }

                if (String.IsNullOrEmpty(nouveauMdpUser))
                {
                    var mdpError = this.nouveauMdpError;
                    mdpError.IsVisible = true;
                }
                else // pas vide ou null alors on va valider par la base de donnée
                {
                    var mdpError = this.nouveauMdpError;
                    mdpError.IsVisible = false; 
                }

                return false;
            }
            // Validate par base de données
            else
            {
                // Verifier que c'est le bon code de recouvrement 
                // et que Mdp n'est pas reutilisé
                return true;
            }


        }

        async void signInRecoverPassword(Object s, EventArgs e)
        {
            if (verifRecoveryPassword())
            {
                // Une fois user login je change le mainPage a la page "RechercheLosisrs"
                // Une fois connecte il doit pas pouvoir retourner a la page RecoverPassword
                Application.Current.MainPage = new NavigationPage(new ChooseAndModifyInterests());
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