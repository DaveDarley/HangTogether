using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using HangTogether.ServerManager;

namespace HangTogether
{
    public class DataBaseMessagesManager
    { 
        FirebaseClient firebaseClient;
        public DataBaseMessagesManager()
        {
            firebaseClient = new FirebaseClient("https://anodate-ca8b9-default-rtdb.firebaseio.com/");
        }

        //Fonctiion qui ajoute un nouveau echange entre 2 utilisateurs dans 
        //la BD
        public async void addNewConversation(Message conversation)
        {
            await firebaseClient
                .Child("Messages")
                .PostAsync(conversation);
        }

        /*
         * Idee: Soit user A ecrit a User B, on ajoute les nouveaux messages
         * Dans une nouvelle Table; qd User B recupere ce message, on l'efface de
         * ma table de nouveaux Messages. Comme ca pas besoin de de parcourir ma liste entier
         * de message pour trouver le nouveau message
        */
        public async void addNonReadMessages(Message nouveauMessage)
        {
            await firebaseClient
                .Child("Nouveaux Messages")
                .PostAsync(nouveauMessage);
        }

        /*
         * Qd user B lit le nouveau Message qui lui etait destine, on efface le nouveau
         * message de la liste de Nouveaux Messages
         */
        public async Task deleteMessageFromNonReadMEssages(Message message)
        {
            await firebaseClient.Child("Nouveaux Messages").Child(message.Key).DeleteAsync(); 
        }
        
        // A faire: Ameliorer les 2 fonctions qui suivent:
        
        /*
         * Fonction qui retourne tous les messages de la table qui lui est passé en parametre
         * 2 tables possibles: Messages ou Nouveaux Messages
         */
        public async Task<List<Message>> GetAllMessages(string nomTable)
        {
            return (await firebaseClient
                .Child(nomTable)
                .OnceAsync<Message>()).Select(item => new Message(
                    item.Object.fromEmail, item.Object.toEmail, item.Object.message, item.Key ,item.Object.timeStamp)).ToList();
        }
       
        /*
         * Fonction qui retourne tous les conversations entre 2 users en particulier.
         */
        public List<Message> getConvos(User from, User to, List<Message>allMessages)
        {
            List<Message> convos = new List<Message>();
            foreach (var message in allMessages)
            {
                if (message.fromEmail == from.email && message.toEmail == to.email
                    || message.fromEmail == to.email && message.toEmail == from.email)
                {
                    convos.Add(message);
                }
            }
            return convos;
        }
        
        /*
         * Fonction qui retourne tous les conversations d'un user VERS un autre.
         * Qd je cherche dans ma table "Nouveaux Messages" si je suis le user A
         * je veux pas recuperer mes propres messages mais plutot les nouveaux messages
         * que B m'a envoye ;
         * Pas besoin de reparcourir ma liste total de messages et de les reafficher
         */
        public List<Message> getConvosFromOneUserToAnother(User from, User to, List<Message>allMessages)
        {
            List<Message> convos = new List<Message>();
            foreach (var message in allMessages)
            {
                if (message.toEmail == to.email && message.fromEmail == from.email)
                {
                    convos.Add(message);
                }
            }
            return convos;
        }

        /*
         * Fonction qui recupere pour un user quelcquonque
         * tous les users avec lesquels il est deja rentre en contact(i.e ecrit ou recu un message)
         */
        public async Task<List<User>> getUserInContactsWithMe(User user)
        {
            List<string> emailUserInContactWithMe = new List<string>();
            List<Message> allMessages = await GetAllMessages("Messages");
            DataBaseManager dataBaseManager = new DataBaseManager();
            List<User> allUsers = await dataBaseManager.GetAllUsers();
            List<User> usersInContactWithMe = new List<User>();
            
            foreach (var messages in allMessages)
            {
                var emailToAdd = (messages.fromEmail == user.email)
                    ? messages.toEmail
                    : (messages.toEmail == user.email)
                        ? messages.fromEmail
                        : "";

                if ((!string.IsNullOrEmpty(emailToAdd)) && (!emailUserInContactWithMe.Contains(emailToAdd)))
                {
                    emailUserInContactWithMe.Add(emailToAdd);
                }
            }

            foreach (var User in allUsers)
            {
                if (emailUserInContactWithMe.Contains(User.email))
                {
                    usersInContactWithMe.Add(User);
                }
            }

            return usersInContactWithMe;
        }




    }
}