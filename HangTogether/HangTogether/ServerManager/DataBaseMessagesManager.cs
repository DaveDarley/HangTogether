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

        //Fonction qui ajoute un nouveau echange entre 2 utilisateurs dans 
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
         * on met l'attribut lu des messages a "y".
         * Cette fonction represente l'historique de conversations entre 2 users.
         */
        public async Task<List<Message>> getAllMessages(User firstUser, User sndUser)
        {
            List<Message> messagesUser = new List<Message>();
            var messages = (await firebaseClient.Child("Messages")
                .OnceAsync<Message>()).AsEnumerable().Where(message =>
                message.Object.toEmail == firstUser.email && message.Object.fromEmail == sndUser.email
            || message.Object.toEmail == sndUser.email && message.Object.fromEmail == firstUser.email);
            foreach (var messageObject in messages)
            {
                Message message = messageObject.Object;
                message.Key = messageObject.Key;
                if (message.toEmail == firstUser.email && message.fromEmail == sndUser.email)
                {
                    message.lu = "y";
                    messagesUser.Add(message);
                    updateConversation(message);
                }

                if (message.toEmail == sndUser.email && message.fromEmail == firstUser.email)
                {
                    messagesUser.Add(message);
                    updateConversation(message);
                }
            }

            return messagesUser;
        }
        
        /*
         * Fonction qui recupere tous les messages non lu destinés a l'utilisateur passe au premier
         * parametre;
         * Lorsque recuperé, on met l'attribut "lu" de ces messages a y
         */
        public async Task<List<Message>> getNonReadMessages(User firstUser, User sndUser)
        {
            List<Message> messagesUser = new List<Message>();
            var messages = (await firebaseClient.Child("Messages")
                .OnceAsync<Message>()).AsEnumerable().Where(message =>
                message.Object.toEmail == firstUser.email && message.Object.fromEmail == sndUser.email).ToList();

            if (messages.Count() != 0)
            {
                var nonReadMessagesObject = messages.AsEnumerable().Where(message =>
                    message.Object.lu == "n").ToList();
                if (nonReadMessagesObject.Count() != 0)
                {
                    foreach (var messageObject in nonReadMessagesObject)
                    {
                        Message message = messageObject.Object;
                        message.Key = messageObject.Key;
                        message.lu = "y";
                        messagesUser.Add(message);
                        updateConversation(message);
                    }
                }
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
            List<string> emailUserInContactWithMe = new List<string>();
            List<User> userInContactWithMe = new List<User>();

            var allConvosWithMe = (await firebaseClient.Child("Messages").OnceAsync<Message>())
                .AsEnumerable().Where(message => message.Object.fromEmail == user.email ||
                                                message.Object.toEmail == user.email).ToList();
            foreach (var messageObject in allConvosWithMe)
            {
                
                if (messageObject.Object.fromEmail == user.email)
                {
                    string emailPossibleUserInContactWithMe = messageObject.Object.toEmail;
                    if (!( emailUserInContactWithMe.Contains(emailPossibleUserInContactWithMe)))
                    {
                        emailUserInContactWithMe.Add(emailPossibleUserInContactWithMe);
                    }
                }
                if (messageObject.Object.toEmail == user.email)
                {
                    string emailPossibleUserInContactWithMe = messageObject.Object.fromEmail;
                    if (!( emailUserInContactWithMe.Contains(emailPossibleUserInContactWithMe)))
                    {
                        emailUserInContactWithMe.Add(emailPossibleUserInContactWithMe);
                    }
                }
            }

            foreach (var userEmail in emailUserInContactWithMe)
            {
                userInContactWithMe.Add(await dataBaseManager.getUser(userEmail));
            }
            return userInContactWithMe;
        }

        
        public async Task<int> getNumberOfNewMessages(User userSendingMessages, User userGoingThroughContacts)
        {
            int nbNouveauxMessages = 0;
            var nonReadMessagesToME = (await firebaseClient.Child("Messages")
                .OnceAsync<Message>()).AsEnumerable().Where(message =>
                message.Object.fromEmail == userSendingMessages.email && message.Object.toEmail ==
                                                                      userGoingThroughContacts.email
                                                                      && message.Object.lu == "n");

            if (nonReadMessagesToME.Count() != 0)
            {
                nbNouveauxMessages += nonReadMessagesToME.Count();
            }

            return nbNouveauxMessages;
        }




    }
}