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
         * champs email, mdp sont du bon format et ensuite on verifie ces informations par
         * rapport a la BD
         */
        async void ValidateInfosUser(object sender, EventArgs args)
        {
            if (IsUserCorrect())
            {
                DataBaseManager dataBaseManager = new DataBaseManager();
                var allUser = await dataBaseManager.GetAllUsers();
               // var hashedMdpEnterByUser = SecureMdp.encryptPassword(this.mdpUser.Text);
                if (dataBaseManager.isUserValid(this.emailUser.Text,this.mdpUser.Text, allUser))
                {
                    var user = dataBaseManager.getUser(allUser, this.emailUser.Text);
                    Application.Current.MainPage = new NavigationPage(new ChooseAndModifyInterests(user));
                }
                else
                {
                    if (!dataBaseManager.isEmailAlreadyInUsage(this.emailUser.Text, allUser))
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
         * laissés par le user
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
         * Sinon peut etre un peit Toast?? Et oui
         */
        async void OnTapForgetPassword(object sender, EventArgs args)
        {
            string emailUser = await DisplayPromptAsync("Recover Password", "What's your email?");
            if (!String.IsNullOrEmpty(emailUser))
            {
                DataBaseManager dataBaseManager = new DataBaseManager();
                var allUser = await dataBaseManager.GetAllUsers();
                bool isUserValid = dataBaseManager.isEmailAlreadyInUsage(emailUser, allUser);
                if (isUserValid)
                {
                    string verifCode = generateVerifCodeRandom();
                    
                    //https://www.c-sharpcorner.com/article/xamarin-forms-send-email-using-smtp2/
                    try
                    {
                        MailMessage message = new MailMessage();
                        SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                        message.From = new MailAddress("hangtogether.app@gmail.com");
                        message.To.Add(new MailAddress(emailUser));
                        message.Subject = "Recouvrement mot de passe App:HangTogether";
                        message.Body = "Votre de code de verification est: "+ verifCode;
                        smtp.Port = 587;
                        smtp.Host = "smtp.gmail.com"; //for gmail host
                        smtp.EnableSsl = true;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new System.Net.NetworkCredential("hangtogether.app@gmail.com", "boxafkachcvptyrl");
                        smtp.Send(message);

                        RecoverPasswordUser userPasswordRecover = new RecoverPasswordUser(emailUser, verifCode);
                       // dataBaseManager.adduserPasswordRecovery(userPasswordRecover);
                        await Navigation.PushAsync(new ForgottenPassword(userPasswordRecover));
                    }
                    catch (Exception ex)
                    {
                        DisplayAlert("Erreur lors de l'envoie du code de verification", ex.Message, "OK");
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

        /*
         * Cette fonction genere de maniere ALEATOIRE
         * une suite de 10 lettres
         * Src: https://www.softwaretestinghelp.com/csharp-random-number/
         */
        public string generateVerifCodeRandom()
        {
            Random ran = new Random();
             
            String b = "abcdefghijklmnopqrstuvwxyz";
            
            int length = 10;
             
            String randomCodeVerification = "";
             
            for(int i =0; i<length; i++)
            {
                int a = ran.Next(26);
                randomCodeVerification = randomCodeVerification + b.ElementAt(a);
            }

            return randomCodeVerification;
        }
        
        
        async void OnTapSignUp(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new SignUpUser());
            
        }
        

        
    }
}