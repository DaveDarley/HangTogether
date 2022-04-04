using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Database;
using HangTogether.ServerManager;

using System.Linq;
using Firebase.Database.Query;

namespace HangTogether
{
    /*
     * On passe le user en parametre au constructeur de la classe
     * et pour ce user on cheche tous les autres users qui sont en contact
     * avec lui . On pourra donc creer une LISTE DE CHATS
     */
    public class ChatHub
    {
        private User user;
        private FirebaseClient firebaseClient;
        public ChatHub()
        {
            firebaseClient = new FirebaseClient("https://anodate-ca8b9-default-rtdb.firebaseio.com/");
        }


        /*
         * Fonction qui recupere tous les users avec lequel "le user en question"
         * est rentré en contact
         */
        public async Task<List<User>> getUserInContactsWithMe()
        {
            var user = this.user;
            List<User> usersInCommunicationWithMe = new List<User>(); 
            DataBaseManager dataBaseManager = new DataBaseManager();
            var listMessages = from userMessages in (await firebaseClient
                    .Child("Messages")
                    .OnceAsync<Message>())
                where (userMessages.Object.fromEmail == user.email
                       || userMessages.Object.toEmail == user.email)
                select new Message(
                     userMessages.Object.fromEmail,
                     userMessages.Object.toEmail,
                     userMessages.Object.message,
                     userMessages.Key,
                     userMessages.Object.timeStamp);
        

            foreach (var item in listMessages)
            {
                if (item.fromEmail == user.email)
                {
                    usersInCommunicationWithMe.Add(await dataBaseManager.getUserParticularUser(item.toEmail));
                }
                if (item.toEmail == user.email)
                {
                    usersInCommunicationWithMe.Add(await dataBaseManager.getUserParticularUser(item.fromEmail));
                }
            }

            return usersInCommunicationWithMe;
        }


       
    

    }
}