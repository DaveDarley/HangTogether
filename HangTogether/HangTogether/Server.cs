using System;
using System.Collections.Generic;
using Firebase.Database;
using HangTogether.ServerManager;

namespace HangTogether
{
    /*
     * Classe qui s'assure de la communication entre la DB et le Serveur
     * Pour maintenir SYNCHRONIZATION entre les differents clients
     */
    
    // Au debut de l'application on instancie un serveur et on le connecte
    // a ma DB
    public class Server
    {
        private FirebaseClient firebase;
        /*
         * Idee:
         * Qd client A envoie message a Client B; On stocke le message
         * dans forClientB et on le met aussi dans ma DB;
         * A chaque seconde client B demande a Server si il n'y a pas de nouveau
         * message pour moi et si forClientB non vide le server lui retourne
         * la liste de message qui lui est destine.
         */
        private List<Message> messagesEnAttenteDelivery;
        
        
        public Server(List<Message>messageWaitingToDeliverBetweeTwoUsers)
        {
            this.messagesEnAttenteDelivery = messageWaitingToDeliverBetweeTwoUsers;
            connectToDB();
        }

        public void connectToDB()
        {
            firebase = new FirebaseClient("https://anodate-ca8b9-default-rtdb.firebaseio.com/");
        }

        
        public async void sendMessagesToDB(Message message)
        {
            DataBaseMessagesManager dataBaseMessagesManager = new DataBaseMessagesManager();
            dataBaseMessagesManager.addNewConversation(message);
            
            messagesEnAttenteDelivery.Add(message);
            
            
        }

        /*
         * Qd 2 user connectes, cette fonction sera appelé a chaque seconde
         * pour verifier si il n'y a pas de messages en attentes pour lui
         * de la part d'un user en particulier
         */
        public List<Message> isThereNewMessages(User toUser, User fromUser)
        {
            List<Message> toUserfromUser = new List<Message>();
            if (messagesEnAttenteDelivery.Count > 0)
            {
                for (int i = 0; i<messagesEnAttenteDelivery.Count; i++)
                {
                    Message message = messagesEnAttenteDelivery[i];
                    if (message.fromEmail == fromUser.email && message.toEmail == toUser.email)
                    {
                        toUserfromUser.Add(message);
                        messagesEnAttenteDelivery.RemoveAt(i);
                    }
                }
            }
            return toUserfromUser;
        }
        

    }
}