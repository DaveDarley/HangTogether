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
    public partial class SignUpUser : ContentPage
    {
        public SignUpUser()
        {
            InitializeComponent();
        }

        async void onClickSignUp(Object o, EventArgs e)
        {
            if (validateInfosUser())
            {
                Application.Current.MainPage = new ChooseAndModifyInterests();
            }
        }

        
        // A verifier si l'email n'est pas deja en cours d'utilisation
        // dans la BD et s'il a rentre tous les infos requises
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
                var nomError = this.nomError;
                nomError.IsVisible = true;
                isInfosValid = false;
            }
            else
            {
                var nomError = this.nomError;
                nomError.IsVisible = false;
            }

            if (String.IsNullOrEmpty(prenomUser))
            {
                var prenomError = this.prenomError;
                prenomError.IsVisible = true;
                isInfosValid = false;
            }
            else
            {
                var prenomError = this.prenomError;
                prenomError.IsVisible = false;
            }
            
            if (String.IsNullOrEmpty(emailUser))
            {
                var emailError = this.emailError;
                emailError.IsVisible = true;
                isInfosValid = false;
            }
            else
            {
                var emailError = this.emailError;
                emailError.IsVisible = false;
            }
            
            if (String.IsNullOrEmpty(mdpUser))
            {
                var mdpError = this.mdpError;
                mdpError.IsVisible = true;
                isInfosValid = false;
            }
            else
            {
                var mdpError = this.mdpError;
                mdpError.IsVisible = false;
            }
            
            dob.SetValue (DatePicker.MaximumDateProperty, DateTime.Now);
            

            // Faut verifier si email pas deja en utilisation ici
            return isInfosValid;
        }
    }
}