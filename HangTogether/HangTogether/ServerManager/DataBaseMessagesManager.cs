using System;
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

        /*
         * Fonction qui ajoute un nouveau message, a la table non lu du recepteur
        */
        public async void addNewConversation(Message conversation)
        {
            DataBaseManager dataBaseManager = new DataBaseManager();
            User userFromMessage = await dataBaseManager.getUser(conversation.fromEmail);
            User userToMessage = await dataBaseManager.getUser(conversation.toEmail);
            await firebaseClient.Child("Messages")
                .Child(userFromMessage.id).Child(userToMessage.id).Child("nonlu")
                .Child(conversation.Key).PutAsync(conversation);
        }

        
        /*
         * Qd un message est envoye de A a B, le message est considere comme
         * non lu par B, une fois le message lu par B, il faut l'enlever du champ
         * non lu de B et le mettre dans le champ lu de B
         */
        public async void addMessagesToReadOnUserRead(Message conversation)
        {
            DataBaseManager dataBaseManager = new DataBaseManager();
            User userFromMessage = await dataBaseManager.getUser(conversation.fromEmail);
            User userToMessage = await dataBaseManager.getUser(conversation.toEmail);
            
            await firebaseClient.Child("Messages")
                .Child(userFromMessage.id).Child(userToMessage.id).Child("lu")
                .Child(conversation.Key).PutAsync(conversation);
        }

        public async void deleteMessagesFromNonReadMessagesTableOnRead(Message message)
        {
            DataBaseManager dataBaseManager = new DataBaseManager();
            User userFromMessage = await dataBaseManager.getUser(message.fromEmail);
            User userToMessage = await dataBaseManager.getUser(message.toEmail);
            
            await firebaseClient.Child("Messages").Child(userFromMessage.id)
                .Child(userToMessage.id).Child("nonlu")
                .Child(message.Key).DeleteAsync();
        }
        

        /*
         * Fonction qui recupere tous les messages entre 2 users;
         * Pour tous les messages destines a l'user qui a appelé cette fonction (firstUser)
         * on met l'attribut lu des messages a "y".
         * Cette fonction represente l'historique de conversations entre 2 users.
         */
        public async Task<List<Message>> getAllMessages(User userFrom, User userTo)
        {
            List<Message> messagesUser = new List<Message>();
            
            // Recuperation messages lu que userFrom a envoye a userTo
            var readMessagesFromUserFromToUserTo =
                (await firebaseClient.Child("Messages").Child(userFrom.id).Child(userTo.id).Child("lu").OnceAsync<Message>())
                .AsEnumerable().ToList();
            if (readMessagesFromUserFromToUserTo.Count() != 0)
            {
                foreach (var message in readMessagesFromUserFromToUserTo)
                {
                    Message monMessage = message.Object;
                    messagesUser.Add(monMessage);
                }
            }
            
            // Recuperation des messages non lu que userFrom a envoye a userTo
            var NonReadMessagesFromUserFromToUserTo =
                (await firebaseClient.Child("Messages").Child(userFrom.id).Child(userTo.id).Child("nonlu").OnceAsync<Message>())
                .AsEnumerable().ToList();
            if (NonReadMessagesFromUserFromToUserTo.Count() != 0)
            {
                foreach (var message in NonReadMessagesFromUserFromToUserTo)
                {
                    Message monMessage = message.Object;
                    messagesUser.Add(monMessage);
                }
            }
            
            // recuperation des messages lu que userFrom a recu de userTo
            var readMessagesFromUserToToUserFrom =
                (await firebaseClient.Child("Messages").Child(userTo.id).Child(userFrom.id).Child("lu").OnceAsync<Message>())
                .AsEnumerable().ToList();
            if (readMessagesFromUserToToUserFrom.Count() != 0)
            {
                foreach (var message in readMessagesFromUserToToUserFrom)
                {
                    Message monMessage = message.Object;
                    messagesUser.Add(monMessage);
                }
            }
            
            // recuperation des messages non lu que userFrom a recu de userTo
            var NonreadMessagesFromUserToToUserFrom =
                (await firebaseClient.Child("Messages").Child(userTo.id).Child(userFrom.id).Child("nonlu").OnceAsync<Message>())
                .AsEnumerable().ToList();
            if (NonreadMessagesFromUserToToUserFrom.Count() != 0)
            {
                foreach (var message in NonreadMessagesFromUserToToUserFrom)
                {
                    Message monMessage = message.Object;
                    
                    addMessagesToReadOnUserRead(monMessage);
                    deleteMessagesFromNonReadMessagesTableOnRead(monMessage);
                    
                    messagesUser.Add(monMessage);
                }
            }

            // vu qu'on a ft 4 requetes pour recuperer tous les messages entre user, on est pas sur de l'ordre d'arrivee
            // alors on ordonne les message par timestamp
            // Src: https://www.codegrepper.com/code-examples/csharp/sort+long+list+like+short+list+c%23+
            messagesUser = messagesUser.OrderBy(o=>o.Key).ToList();

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
            var messages = (await firebaseClient.Child("Messages").Child(sndUser.id).Child(firstUser.id).Child("nonlu").OrderByKey()
                .OnceAsync<Message>()).ToList();

            if (messages.Count() != 0)
            {
                foreach (var messageObject in messages)
                {
                    addMessagesToReadOnUserRead(messageObject.Object);
                    deleteMessagesFromNonReadMessagesTableOnRead(messageObject.Object);
                    messagesUser.Add(messageObject.Object);
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
            List<User> userInContactsWithMe = new List<User>();
            List<string> idUserInContactWithMe = new List<string>();
            

            // ce sera couteux au long terme
            var allUsers = (await firebaseClient.Child("Users").OrderByKey().OnceAsync<User>()).AsEnumerable().ToList();
            List<string> idAllUsers = new List<string>();

            foreach (var firebaseObjectUser in allUsers)
            {
                if (firebaseObjectUser.Object.id != user.id)
                {
                    idAllUsers.Add(firebaseObjectUser.Object.id);
                }
            }

            foreach (var iduser in idAllUsers)
            {
                var possibleContact1 =
                    (await firebaseClient.Child("Messages").Child(user.id).Child(iduser).Child("nonlu").OrderByKey().OnceAsync<Message>()).AsEnumerable().ToList();
                
                var possibleContacts2 =
                    (await firebaseClient.Child("Messages").Child(user.id).Child(iduser).Child("lu").OrderByKey().OnceAsync<Message>()).AsEnumerable().ToList();
                
                var possibleContacts3 =
                    (await firebaseClient.Child("Messages").Child(iduser).Child(user.id).Child("lu").OrderByKey().OnceAsync<Message>()).AsEnumerable().ToList();
                
                var possibleContacts4 =
                    (await firebaseClient.Child("Messages").Child(iduser).Child(user.id).Child("nonlu").OrderByKey().OnceAsync<Message>()).AsEnumerable().ToList();

                if (possibleContact1.Count()!= 0 || possibleContacts2.Count() != 0 || possibleContacts3.Count() != 0 || possibleContacts4.Count() != 0)
                {
                    if (!(idUserInContactWithMe.Contains(iduser)))
                    {
                        idUserInContactWithMe.Add(iduser);
                    }
                }
            }
            
            foreach (var idUser in idUserInContactWithMe)
            {
               userInContactsWithMe.Add(await dataBaseManager.getUserById(idUser)); 
            }
            return userInContactsWithMe;
        }

        
        public async Task<int> getNumberOfNewMessages(User userSendingMessages, User userGoingThroughContacts)
        {
            int nbNouveauxMessages = 0;
            var nonReadMessagesToME =
                (await firebaseClient.Child("Messages").Child(userSendingMessages.id).Child(userGoingThroughContacts.id).Child("nonlu")
                    .OnceAsync<Message>()).AsEnumerable().ToList();

            if (nonReadMessagesToME.Count() != 0)
            {
                nbNouveauxMessages += nonReadMessagesToME.Count();
            }
            else
            {
                nbNouveauxMessages = 0;
            }
            
            return nbNouveauxMessages;
        }




    }
}