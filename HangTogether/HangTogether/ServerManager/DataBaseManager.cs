using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Grpc.Auth;
using HangTogether.ServerManager;
using Xamarin.Forms;

// utilisation firestore et c# : https://pieterdlinde.medium.com/netcore-and-cloud-firestore-94628943eb3c
namespace HangTogether
{
    public class DataBaseManager
    {
        public static FirestoreDb firebase;
        public DataBaseManager()
        {
            
            var firestoreDbBuilder = new FirestoreDbBuilder {
                ProjectId = "hangtogether-edc71", 
                ChannelCredentials = GoogleCredential.FromFile("/Users/davejoseph/Desktop/hangtogether-edc71-firebase-adminsdk-4ohp9-40866f045d.json").ToChannelCredentials()// or FromFileAsync()
            };
            
            firebase = firestoreDbBuilder.Build(); // or BuildAsync()
            
            // var firestoreDb = firestoreDbBuilder.Build(); // or BuildAsync()
            // var jsonString = File.ReadAllText("/Users/davejoseph/Desktop/hangtogether-edc71-firebase-adminsdk-4ohp9-40866f045d.json");
            // var builder = new FirestoreClientBuilder {JsonCredentials = jsonString};
            //
            // // string filepath = "Users/davejoseph/Downloads/hangtogether-edc71-firebase-adminsdk-4ohp9-40866f045d.json";
            // // System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", filepath);
            // firebase = FirestoreDb.Create("hangtogether-edc71",builder.Build());

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
            //await firebase.Child("Users").Child(user.Key).DeleteAsync(); 

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
         * Structure:
         * - Users(Collection)
         * -- User A
         * --- nom
         * --- prenom
         * --- email
         * --- ....
         * --- Loisirs(Collection)
         * ----Loisir 1
         * ----- reference vers table Loisirs (Pareil pour les autres documents)
         */

        public async void addReferenceToInterest(User user, string loisir)
        {
            DocumentReference userRef = firebase.Collection("Users").Document(user.id).Collection("Loisirs").Document(loisir);
            DocumentReference interestsRef = firebase.Collection("Loisirs").Document(loisir);
            await userRef.SetAsync(interestsRef);
        }

        public async void deleteUserInterest(User user, string loisir)
        {
            DocumentReference userRef = firebase.Collection("Users").Document(user.id).Collection("Loisirs").Document(loisir);
            await userRef.DeleteAsync();
        }

        /*
         * Fonction qui recupere la liste de loisirs d'un user en particulier 
         */
        public async Task<List<Loisir>> getInterestsUser(User user)
        {
            List<Loisir> interestsUser = new List<Loisir>();
            Query capitalQuery = firebase.Collection("Users").Document(user.id).Collection("Loisirs");
            QuerySnapshot capitalQuerySnapshot = await capitalQuery.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in capitalQuerySnapshot.Documents)
            {
                Loisir loisir = documentSnapshot.ConvertTo<Loisir>();
                interestsUser.Add(loisir);
            }

            return interestsUser;
        }


        /*
         * Fonction qui prend en parametre un utilisateur et une liste d'utilisateur.
         * Elle retourne une liste d'utilisateur qui ont au moins un interet en commun
         * avec l'utilisateur.
        */
        
        //src: https://www.delftstack.com/howto/csharp/check-for-an-element-inside-an-array-in-csharp/
        public async Task<List<User>> getUserWithSharedInterests(User userLookingForFriends)
        {
            List<Loisir> interestsUserLookingForFriends = await getInterestsUser(userLookingForFriends);
            List<User> usersWithSharedInterests = new List<User>();
            
            Query allUsers = firebase.Collection("Users");
            QuerySnapshot allUsersQuerySnapshot = await allUsers.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in allUsersQuerySnapshot.Documents)
            {
                User user = documentSnapshot.ConvertTo<User>();
                List<Loisir> loisirsPotentialFriends = await getInterestsUser(user);
                foreach (var loisirsUser in loisirsPotentialFriends)
                {
                    if (interestsUserLookingForFriends.Contains(loisirsUser))
                    {
                        usersWithSharedInterests.Add(user);
                        break;
                    }
                }
            }
            return usersWithSharedInterests;
        }
        





    }
}