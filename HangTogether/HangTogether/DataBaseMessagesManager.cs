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
                .Child("Conversations")
                .PostAsync(conversation);
        }
        
        /*
         * Fonction qui s'occupe de mettre a jour les messages entre
         * 2 utilisateurs (lorsque l'un d'entre les 2 envoie un nouveau message)
        */
        public async Task UpdateConversation(Message message)
        {
            await firebaseClient.Child("Conversations").Child(message.Key)
                .PutAsync(message); 
        }

        /*
         * Fonction qui retoune les conversations (s'il existe) entre 2 utilisateurs
         */
        public Message getConversation(string emailFirstUser, string emailSndUser, List<Message> conversations)
        {
            Message conversationBetweenTwoUsers = new Message("","","","","");
            foreach (var conversation in conversations)
            {
                if ((conversation.fromEmail == emailFirstUser && conversation.toEmail == emailSndUser)
                    ||(conversation.fromEmail == emailSndUser && conversation.toEmail == emailFirstUser) )
                {
                    conversationBetweenTwoUsers = conversation;
                }
            }
            return conversationBetweenTwoUsers;
        }
        
        /*
         * Fonction qui retourne toutes les conversations de ma base de donnée
        */
        public async Task<List<Message>> GetAllConversations()
        {
            return (await firebaseClient
                .Child("Conversations")
                .OnceAsync<Message>()).Select(item => new Message(
                    item.Object.fromEmail,item.Object.toEmail,item.Object.firstMessage,item.Object.sndMessage,item.Key)).ToList();
        }
        
        
    }
}