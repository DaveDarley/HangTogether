using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using HangTogether.ServerManager;

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
            DataBaseManager dataBaseManager = new DataBaseManager();
            List<User> userInContactWithMe = new List<User>();
            
            Query allMessagesQuery = firebaseClient.Collection("Messages");
            QuerySnapshot allMessagesQuerySnapshot = await allMessagesQuery.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in allMessagesQuerySnapshot.Documents)
            {
                Message message = documentSnapshot.ConvertTo<Message>();
                if (message.fromEmail == user.email)
                {
                   userInContactWithMe.Add(await dataBaseManager.getUser(message.toEmail)); 
                }
                if (message.toEmail == user.email)
                {
                    userInContactWithMe.Add(await dataBaseManager.getUser(message.fromEmail)); 
                }
            }
            return userInContactWithMe;
        }

        public async Task<int> getNumberOfNewMessages(User userSendingMessages, User userGoingThroughContacts)
        {
            int nbNouveauxMessages = 0;
            Query allMessagesQuery = firebaseClient.Collection("Messages");
            QuerySnapshot allMessagesQuerySnapshot = await allMessagesQuery.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in allMessagesQuerySnapshot.Documents)
            {
                Message message = documentSnapshot.ConvertTo<Message>();
                if (message.fromEmail == userSendingMessages.email && message.toEmail == userGoingThroughContacts.email && message.lu == "n")
                {
                    nbNouveauxMessages++;
                }
            }

            return nbNouveauxMessages;
        }




    }
}