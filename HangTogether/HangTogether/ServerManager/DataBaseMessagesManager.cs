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
            await firebaseClient.Child("Messages")
                .PostAsync(conversation);
        }

        /*
         * Fonction qui s'occupe de mettre a jour un message dans ma BD
         * particulierement mettre a jour le champ lu
         */
        public async void updateConversation(Message conversation)
        {
            await firebaseClient.Child("Messages")
                .Child(conversation.Key).PutAsync(conversation);
        }

        /*
         * Fonction qui recupere tous les messages entre 2 users;
         * Pour tous les messages destines a l'user qui a appelé cette fonction (firstUser)
         * on met l'attribut lu des messages a "y"
         */
        public async Task<List<Message>> getAllMessages(User firstUser, User sndUser)
        {
            List<Message> messagesUser = new List<Message>();
            var messages = (await firebaseClient.Child("Messages")
                .OnceAsync<Message>()).AsEnumerable().Where(message =>
                message.Object.toEmail == firstUser.email && message.Object.fromEmail == sndUser.email
            || message.Object.toEmail == sndUser.email && message.Object.toEmail == firstUser.email);
            foreach (var messageObject in messages)
            {
                Message message = messageObject.Object;
                message.Key = messageObject.Key;
                if (message.toEmail == firstUser.email && message.fromEmail == sndUser.email)
                {
                    message.lu = "y";
                }
                messagesUser.Add(message);
                updateConversation(message);
            }

            return messagesUser;
        }
        
        public async Task<List<Message>> getNonReadMessages(User firstUser, User sndUser)
        {
            List<Message> messagesUser = new List<Message>();
            var messages = (await firebaseClient.Child("Messages")
                .OnceAsync<Message>()).AsEnumerable().Where(message =>
                message.Object.toEmail == firstUser.email && message.Object.fromEmail == sndUser.email &&
                message.Object.lu == "n");
            foreach (var messageObject in messages)
            {
                Message message = messageObject.Object;
                message.Key = messageObject.Key;
                message.lu = "y";
                messagesUser.Add(message);
                updateConversation(message);
            }

            return messagesUser;
        }



       
        

        /*
         * Fonction qui recupere pour un user quelcquonque
         * tous les users avec lesquels il est deja rentre en contact(i.e ecrit ou recu un message)
         */
        public async Task<List<User>> getUserInContactsWithMe(User user)
        {
            DataBaseManager dataBaseManager = new DataBaseManager();
            List<User> userInContactWithMe = new List<User>();

            var allConvosWithMe = (await firebaseClient.Child("Messages").OnceAsync<Message>())
                .AsQueryable().Where(message => message.Object.fromEmail == user.email ||
                                                message.Object.toEmail == user.email).ToList();
            foreach (var messageObject in allConvosWithMe)
            {
                if (messageObject.Object.fromEmail == user.email)
                {
                    userInContactWithMe.Add(await dataBaseManager.getUser(messageObject.Object.toEmail));
                }
                else
                {
                    userInContactWithMe.Add(await dataBaseManager.getUser(messageObject.Object.fromEmail));
                }
            }
            return userInContactWithMe;
        }

        public async Task<int> getNumberOfNewMessages(User userSendingMessages, User userGoingThroughContacts)
        {
            int nbNouveauxMessages = 0;
            nbNouveauxMessages = (await firebaseClient.Child("Messages")
                .OnceAsync<Message>()).AsEnumerable().Where(message =>
                message.Object.fromEmail == userSendingMessages.email && message.Object.toEmail ==
                                                                      userGoingThroughContacts.email
                                                                      && message.Object.lu == "n").Count();

            return nbNouveauxMessages;
        }




    }
}