using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using HangTogether.ServerManager;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.UI.Views.Options;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HangTogether
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LogInSignUp : ContentPage
    {
        public LogInSignUp()
        {
            InitializeComponent();
        }
        
        /*
         *Fonction qui s'occupe de valider l'email et le mot de passe du user
         * lors du Log In;
         * Tout d'abord elle appelle une fonction qui verifie que les infos mis dans les
         * champs email, mdp sont du bon Format et ensuite on verifie ces informations par
         * rapport a la BD
         */
        async void ValidateInfosUser(object sender, EventArgs args)
        {
            if (IsUserCorrect())
            {
                DataBaseManager dataBaseManager = new DataBaseManager();
                var trimEmail = this.emailUser.Text.Trim();
                User user = await dataBaseManager.getUser(trimEmail);
                var isUserValid = dataBaseManager.isUserValid(user, this.mdpUser.Text);
                
                if (isUserValid)
                {
                    Application.Current.MainPage = new NavigationPage(new ChooseAndModifyInterests(user));
                }
                else
                {
                    if (user is null)
                    {
                        this.mailError.Text = "Il n'existe aucun compte associe a ce courriel";
                        this.mailError.IsVisible = true;
                    }
                    this.mdpError.Text = "Mot de passe invalide";
                    this.mdpError.IsVisible = true;
                }
            }
        }
        
        /*
         * Cette fonction valide les infos entrees par le user a l'etape de
         * sign in; Cette fonction verifie juste que les champs ne sont pas
         * laissés vide par le user
         */
        public bool IsUserCorrect()
        {
        
             var emailUser = this.emailUser.Text;
             var mdpUser = this.mdpUser.Text;
             var isUserExist = true;
 
             if (String.IsNullOrEmpty(emailUser))
             {
                 this.mailError.IsVisible = true;
                 isUserExist = false;
             }
             else
             {
                 this.mailError.IsVisible = false;
             }
    
             if (String.IsNullOrEmpty(mdpUser))
             {
                 this.mdpError.IsVisible = true;
                 isUserExist = false;
             }
             else
             {
                 this.mdpError.IsVisible = false;
             }

             return isUserExist;
        }
        
        
        /*
         * Lors du recouvrement du mdp d'un user, on verifie si l'email entré par le
         * user pour le recouvrement est un email valide (i.e existe dans la BD)
         * Si oui on lui envoie un email a cette adresse;
         * Sinon peut etre un petit Toast?? Et oui
         */
        async void OnTapForgetPassword(object sender, EventArgs args)
        {
            string email = await DisplayPromptAsync("Recouvrement de Mot de passe", "Quel est votre courriel?");
            if (!String.IsNullOrEmpty(email))
            {
                string emailUser = email.Trim();
                DataBaseManager dataBaseManager = new DataBaseManager();
                var user = await dataBaseManager.getUser(emailUser);
                if (!(user is null))
                {
                    string verifCode = VerificationEmail.verifEmail(emailUser);
                    if (!String.IsNullOrEmpty(verifCode) && verifCode != "Erreur lors de l'envoie du code de verification")
                    {
                        RecoverPasswordUser userPasswordRecover = new RecoverPasswordUser(emailUser, verifCode);
                        // dataBaseManager.adduserPasswordRecovery(userPasswordRecover);
                        await Navigation.PushAsync(new ForgottenPassword(userPasswordRecover));
                    }
                    else
                    {
                        await DisplayAlert("Code de vérification recouvrement mot de passe", "Erreur lors de l'envoie du code de vérification, veuillez réessayer", "OK");
                    }
                }
                else
                { // Toast courriel de recouvrement de mot de passe invalide
                    var messageOptions = new MessageOptions
                    {
                        Message = "Courriel de verification invalide",
                        Foreground = Color.White,
                        Font = Font.SystemFontOfSize(16),
                        Padding = new Thickness(20)
                    };

                    var options = new ToastOptions
                    {
                        MessageOptions = messageOptions,
                        CornerRadius = new Thickness(20, 20, 20, 20),
                        BackgroundColor = Color.DarkGray
                    };
                    // Toast email entre pour recouvrement mot de passe pas valide
                    await this.DisplayToastAsync(options);
                }
            }
            
        }

        
        
        async void OnTapSignUp(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new SignUpUser());
        }
        

        
    }
}