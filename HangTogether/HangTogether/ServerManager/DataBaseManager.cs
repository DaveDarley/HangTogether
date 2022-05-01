using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using Google.Cloud.Firestore;
using HangTogether.ServerManager;
using Xamarin.Forms;

namespace HangTogether
{
    public class DataBaseManager
    {
        private static FirestoreDb firebase;
        public DataBaseManager()
        {
            firebase = FirestoreDb.Create("https://anodate-ca8b9-default-rtdb.firebaseio.com/");

        }
        
        /*
         * Fonction qui s'occupe d'ajouter un nouveau user a la base de données
         */
        public async Task AddUser(User user)
        {
            DocumentReference newDoc = firebase.Collection("Users").Document();
            user.id = newDoc.Id;
            await newDoc.SetAsync(user);
        }


        /*
         * Fonction qui s'occupe de mettre a jour les infos
         * d'un user dans ma BD
         */
        public async Task UpdateUser(User user)
        {
            DocumentReference userRef = firebase.Collection("Users").Document(user.id);
            await userRef.SetAsync(user);

        }

        /*
         * Qd un user desactive son compte , on l'efface de la DB
         */
        public async Task deleteUser(User user)
        {
            await firebase.Child("Users").Child(user.Key).DeleteAsync(); 

        }

        
        /*
         * Fonction qui retourne un user de ma BD.
         * Voir Firestore documentation;
         * Peut aussi etre utilise pour savoir si l'email est en utilisation:
         * Si le user retourne est null alors aucun user n'a deja cet email
         */
        public async Task<User> getUser(string emailUser)
        {
            User user = null;
            // recuperation du document dont le champ email == emailUser
            Query capitalQuery = firebase.Collection("Users").WhereEqualTo("email", emailUser);
            QuerySnapshot capitalQuerySnapshot = await capitalQuery.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in capitalQuerySnapshot.Documents)
            {
                user = documentSnapshot.ConvertTo<User>();
            }

            return user;
        }

        
        /*
         * Avant d'appeler cette fonction je recupere le user qui a l'email entre par le user
         * qui essaie de se connecter, je compare ensuite le mdp entré par le user avec le le mdp
         * du user entré en parametre. Si les 2 mdp correspondent alors c'est le bon user.
         */
        public bool isUserValid(User user, string mdp)
        {
            bool canUserConnect = false;
            if (!(user is null))
            {
                string saltUser = user.saltToEncryptMdp;
                Byte[] saltUserInByteArray = SecureMdp.stringToByteArraySalt(saltUser);
                String mdpEncrypt = SecureMdp.encryptPassword(mdp, saltUserInByteArray);
            
                if (user.mdp == mdpEncrypt)
                {
                    canUserConnect =  true;
                }
            }
            return canUserConnect;
        }

        
        /*
         * Fonction qui prend en parametre un utilisateur et une liste d'utilisateur.
         * Elle retourne une liste d'utilisateur qui ont au moins un interet en commun
         * avec l'utilisateur.
        */
        
        //src: https://www.delftstack.com/howto/csharp/check-for-an-element-inside-an-array-in-csharp/
        public List<User> getUserWithSharedInterests(User userLookingForFriends, List<User> allUsers)
        {
            List<User> usersWithSharedInterests = new List<User>();
            var loisirsUserLookingForFriends = (! userLookingForFriends.loisirs.Contains(',')) ? new []{ userLookingForFriends.loisirs } : userLookingForFriends.loisirs.Split(',');
            
            foreach (var utilisateur in allUsers)
            {
                var loisirsUser = (! utilisateur.loisirs.Contains(',')) ? new []{ utilisateur.loisirs } : utilisateur.loisirs.Split(',');
                
                for (int i = 0; i<loisirsUser.Length; i++)
                {
                    string loisir = loisirsUser[i];
                    if (Array.Exists(loisirsUserLookingForFriends,x => x == loisir) && userLookingForFriends.email!=utilisateur.email)
                    {
                        usersWithSharedInterests.Add(utilisateur);
                        i = loisirsUser.Length;
                        // break;
                    }
                }
            }
            return usersWithSharedInterests;
        }
        





    }
}