using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using Google.Cloud.Firestore;
using HangTogether.ServerManager;
using Realms;

namespace HangTogether
{
    public class DataBaseMessagesManager
    { 
        FirestoreDb firebaseClient;
        public DataBaseMessagesManager()
        {
            firebaseClient = FirestoreDb.Create("https://anodate-ca8b9-default-rtdb.firebaseio.com/");
        }

        //Fonctiion qui ajoute un nouveau echange entre 2 utilisateurs dans 
        //la BD
        public async void addNewConversation(Message conversation)
        {
            DocumentReference messageRef = firebaseClient.Collection("Messages").Document();
            conversation.Key = messageRef.Id;
            await messageRef.SetAsync(conversation);
        }

        /*
         * Fonction qui s'occupe de mettre a jour un message dans ma BD
         * particulierement mettre a jour le champ lu
         */
        public async void updateConversation(Message conversation)
        {
            DocumentReference messagesRef = firebaseClient.Collection("Messages").Document(conversation.Key);
            await messagesRef.SetAsync(conversation);
        }

        /*
         * Fonction qui recupere tous les messages entre 2 users;
         * Pour tous les messages destines a l'user qui a appelé cette fonction (firstUser)
         * on met l'attribut lu des messages a "y"
         */
        public async Task<List<Message>> getAllMessages(User firstUser, User sndUser)
        {
            List<Message> messages = new List<Message>();
            Query allMessagesQuery = firebaseClient.Collection("Messages");
            QuerySnapshot allMessagesQuerySnapshot = await allMessagesQuery.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in allMessagesQuerySnapshot.Documents)
            {
                Message message = documentSnapshot.ConvertTo<Message>();
                if (message.fromEmail == firstUser.email && message.toEmail == sndUser.email 
                    || message.fromEmail == sndUser.email && message.toEmail == firstUser.email)
                {
                    if (message.fromEmail == sndUser.email && message.toEmail == firstUser.email)
                    {
                        message.lu = "y";
                        updateConversation(message);
                    }
                    messages.Add(message);
                }
            }

            return messages;
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