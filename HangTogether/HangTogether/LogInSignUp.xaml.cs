using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.Converters;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HangTogether
{
   // [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LogInSignUp : ContentPage
    {
        public LogInSignUp()
        {
            InitializeComponent();
        }
        
        // command pattern 3911??
       // public ICommand ForgotPasswordCommand => new Command(OnForgotPassword);
        
        /*
         * Validation des infos entrees par le user
         * Email et mdp
         */
        async void ValidateInfosUser(object sender, EventArgs args)
        {
            if (IsUserCorrect())
            {
                // m'envoie a la page recherche loisirs
                Application.Current.MainPage = new NavigationPage(new ChooseAndModifyInterests()) ;
            }
        }

        public bool IsUserCorrect()
        {

             var emailUser = this.emailUser.Text;
             var mdpUser = this.mdpUser.Text;
 
             if ( String.IsNullOrEmpty(emailUser) || String.IsNullOrEmpty(mdpUser))
             {
                 if (String.IsNullOrEmpty(emailUser))
                 {
                     var mailError = this.mailError;
                     mailError.IsVisible = true;
                 }
                 else
                 {
                     var mailError = this.mailError;
                     mailError.IsVisible = false;
                 }

                 if (String.IsNullOrEmpty(mdpUser))
                 {
                     var mdpError = this.mdpError;
                     mdpError.IsVisible = true;
                 }
                 else
                 {
                     var mdpError = this.mdpError;
                     mdpError.IsVisible = true;
                 }

                 return false;
             }
             // Validate par base de données
             else
             {
                 return true;
             }


        }
        
        /*
         * Gestion du click sur mon label: Mdp oublié
         * On envoie l'user sur la page de
         * recouvrement de MDP;
         * Mais Avant faut demander au user l'email sur lequel il veut
         * recevoir 
         */
        async void OnTapForgetPassword(object sender, EventArgs args)
        {
            // S'assurer que l'email est bien valide aussi (BD)
            // Si pas validde on reste dans la meme page et on ft un petit toast
            string emailUser = await DisplayPromptAsync("Recover Password", "What's your email?");
            if (String.IsNullOrEmpty(emailUser))
            {
            }
            else
            {
                await Navigation.PushAsync(new ForgottenPassword());
            }

            
            
        }
        
        async void OnTapSignUp(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new SignUpUser());
            
        }




    }
}