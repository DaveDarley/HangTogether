using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Offline;
using Firebase.Database.Query;
using HangTogether.ServerManager;
using Xamarin.Forms;

// utilisation firestore et c# : https://pieterdlinde.medium.com/netcore-and-cloud-firestore-94628943eb3c
/*
 * https://github.com/step-up-labs/firebase-database-dotnet
 * https://stackoverflow.com/questions/44940726/c-sharp-query-path-for-firebase-data-through-firebasedatabase-net
 * https://stackoverflow.com/questions/71953262/orderby-property-not-working-xamarin-firebase
 * https://stackoverflow.com/questions/60734705/how-to-create-email-key-in-realtime-database-using-firebasedatabase-net
 * https://stackoverflow.com/questions/52557203/how-should-i-make-use-of-ids-in-firebase-database
 * https://stackoverflow.com/questions/70125236/firebase-rest-apihow-to-use-orderby-and-startat-using-the-last-key-id
 * 
 */
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
        /*
         * PK: Ajoute infos dans la BD en inscrivant nous meme notre id
         * https://stackoverflow.com/questions/60734705/how-to-create-email-key-in-realtime-database-using-firebasedatabase-net
         */
        public async Task AddUser(User user)
        {
            string emailUser = Convert.ToBase64String(Encoding.ASCII.GetBytes(user.email));
            await firebase.Child("Users")
              .Child(emailUser)
              .PutAsync(user);
        }


        /*
         * Fonction qui s'occupe de mettre a jour les infos
         * d'un user dans ma BD
         */
        public async Task UpdateUser(User user)
        {
            var convertEmail = Convert.ToBase64String(Encoding.ASCII.GetBytes(user.email));
            await firebase.Child("Users")
                .Child(convertEmail).PutAsync(user);
        }

        /*
         * Qd un user desactive son compte , on l'efface de la DB
         */
        public async Task deleteUser(User user)
        {
            var convertEmail = Convert.ToBase64String(Encoding.ASCII.GetBytes(user.email));
            await firebase.Child("Users").Child(convertEmail).DeleteAsync();
        }

        
        /*
         * Fonction qui recupere le User s'il existe dans ma BD
         * Vu que la clé de chaque user est : email du user convertit en Base64String
         * Pour verifier si un user existe, on convertit son email en base64string et on verifie
         * si cette cle existe dans la BD
         */
        public async Task<User> getUser(string emailUser)
        {   // Peut etre null si le user n'existe pas 
            User monUser = null;
            
            var convertEmail = Convert.ToBase64String(Encoding.ASCII.GetBytes(emailUser));
            var userRecuperer = await firebase.Child("Users").OrderByKey().StartAt(convertEmail).EndAt(convertEmail).LimitToFirst(1).OnceAsync<User>();
            var listFirebaseObjectUser = userRecuperer.ToList();
            if (listFirebaseObjectUser.Count() != 0) // on a trouve un user avec cet email 
            {
                monUser = listFirebaseObjectUser.First().Object;
                if (monUser.id == "")
                {
                    monUser.id = convertEmail;
                    await UpdateUser(monUser);
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
            var convertEmail = Convert.ToBase64String(Encoding.ASCII.GetBytes(user.email));
            var allInterestsUser = (await firebase.Child("ChoixLoisirsUser").Child(convertEmail).OnceAsync<ChoixLoisirsUser>()).ToList();
            foreach (var loisir in allInterestsUser)
            {
                interestsUser.Add(loisir.Object.loisir);
            }
            return interestsUser;
        }
        
        
        /*
         * Fonction qui prend en parametre un utilisateur et elle retourne une liste d'utilisateur qui ont au moins un interet en commun
         * avec l'utilisateur.
         * Cette Fonction recupere tous les users de ma BD, et pour chaque user recupere ces interets.
         * Pour chaque user, si il a au moins un interet en commun avec le user passé en parametre, on l'ajoute
         * a la liste de user qui sera retourné.
         *
         * Cote non efficace: Qd le nb de users s'agrandit, cette operation peut s'averer couteuse
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
            
            // pas trop couteux, qd on a des millions de user ??
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