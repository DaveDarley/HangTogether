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
    /*
     * Classe qui s'occupe de de verifier que l'email entre par le user lors de
     * l'etape de sign up mais aussi lors de l'etape de recouvrement de mot de passe
     *
     * Etape sign in: Seul moyen de verifier que celui qui est entrain de creer le compte avec l'email
     * a bien acces a l'email
     */
    public class VerificationEmail
    {

        /*
         * Fonction qui s'occupe de verifier que l'email entré par le user lors du sign-up appartient bien
         * au user en lui envoyant un code de verification a cet email que le user doit rentrer pour prouver
         * que c'est bien lui
         */
        public static string verifEmail(string emailUser)
        {
            string verifCode = generateVerifCodeRandom();
                    
            //https://www.c-sharpcorner.com/article/xamarin-forms-send-email-using-smtp2/
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                message.From = new MailAddress("hangtogether.app@gmail.com");
                message.To.Add(new MailAddress(emailUser));
                message.Subject = "Vérification Email";
                message.Body = "Votre de code de verification est: "+ verifCode;
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com"; //for gmail host
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("hangtogether.app@gmail.com", "boxafkachcvptyrl");
                smtp.Send(message);
                
            }
            catch (Exception ex)
            {
                return ("Erreur lors de l'envoie du code de verification");
            }
            return verifCode;
        }
        
        /*
         * Cette fonction genere de maniere ALEATOIRE
         * une suite de 10 lettres
         * Src: https://www.softwaretestinghelp.com/csharp-random-number/
         */
        public static string generateVerifCodeRandom()
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

    }
}