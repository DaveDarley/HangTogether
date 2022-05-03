using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using HangTogether.ServerManager;
using Xamarin.Forms;

// utilisation firestore et c# : https://pieterdlinde.medium.com/netcore-and-cloud-firestore-94628943eb3c
namespace HangTogether
{
    public class DataBaseManager
    {
        public  FirebaseClient firebase;
        public  string FirebaseClient = "https://anodate-ca8b9-default-rtdb.firebaseio.com/";
        public  string FrebaseSecret = "17hN90bHf0ROF4BDSUEsrBTw6AFvuFMe6n3sBFTS";
        public DataBaseManager()
        { 
            
         firebase = new FirebaseClient(FirebaseClient,
            new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(FrebaseSecret) });
        }
        
        /*
         * Fonction qui s'occupe d'ajouter un nouveau user a la base de données
         */
        public async Task AddUser(User user)
        {
            await firebase.Child("Users")
                .PostAsync(user);
            User userToUpdateId = await getUser(user.email);
            await UpdateUser(userToUpdateId);
        }


        /*
         * Fonction qui s'occupe de mettre a jour les infos
         * d'un user dans ma BD
         */
        public async Task UpdateUser(User user)
        {
            await firebase.Child("Users")
                .Child(user.id).PutAsync(user);
        }

        /*
         * Qd un user desactive son compte , on l'efface de la DB
         */
        public async Task deleteUser(User user)
        {
            await firebase.Child("Users").Child(user.id).DeleteAsync(); 

        }

        
        /*
         * Qd on utilise ".OnceAsync" ca ns retourne un "IReadOnlyCollection"
         * alors je le convertit en Enumerable et j'applique un filter sur mon IEnumarable<FirebaseObject<User>>
         * Comme il existe un seul user avec cet email s'il existe, alors j'utilise single() sur le IEnumarable
         * pour retourner le FirebaseObject<User> et ensuite je fs .Object pr me retourner le user en question
         *
         * Si where(..) retourner rien et ce que le querry retourne null??
         */
        public async Task<User> getUser(string emailUser)
        {   // Peut etre null si le user n'existe pas 
            User monUser = null;
            var userRecuper = (await firebase.Child("Users")
                .OnceAsync<User>()).AsEnumerable().Where(user => user.Object.email == emailUser).ToList();

            if (userRecuper.Count() != 0)
            {
                FirebaseObject<User> user = userRecuper.FirstOrDefault();
                monUser = user.Object;
                if (monUser.id == "")
                {
                    monUser.id = user.Key;
                   // await UpdateUser(monUser);
                }
            }

            return monUser;
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
         * Fonction qui recupere la liste de loisirs d'un user en particulier 
         */
        public async Task<List<Loisir>> getInterestsUser(User user)
        {
            List<Loisir> interestsUser = new List<Loisir>();
            List<FirebaseObject<ChoixLoisirsUser>> choixUser = (await firebase.Child("ChoixLoisirsUser")
                    .OnceAsync<ChoixLoisirsUser>()).AsEnumerable()
                .Where(userChoice => userChoice.Object.idUser == user.id)
                .ToList();
            foreach (var firebaseObjectLoisir in choixUser)
            {
                interestsUser.Add(firebaseObjectLoisir.Object.loisir);
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
            List<string> interestsUserLookingForFriendsInString = new List<string>();
            foreach (var loisir in interestsUserLookingForFriends)
            {
                interestsUserLookingForFriendsInString.Add(loisir.nom);
            }
            
            List<User> usersWithSharedInterests = new List<User>();
            var allUsers = (await firebase.Child("Users").OnceAsync<User>())
                .AsEnumerable().Select(user => user.Object).ToList();

            foreach (var user in allUsers)
            {
                if (user.email != userLookingForFriends.email)
                {
                    List<Loisir> loisirsOfUser = await getInterestsUser(user);
                
                    List<String> loisirsOfUserInString = new List<string>();
                    foreach (var loisirUser in loisirsOfUser)
                    {
                        loisirsOfUserInString.Add(loisirUser.nom); 
                    }
                
                    foreach (var loisir in loisirsOfUserInString)
                    {
                        if (interestsUserLookingForFriendsInString.Contains(loisir))
                        {
                            usersWithSharedInterests.Add(user);
                            break;
                        }
                    }
                }
            }
            return usersWithSharedInterests;
        }
        





    }
}