using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using HangTogether.ServerManager;
using Xamarin.Forms;

namespace HangTogether
{
    public class DataBaseManager
    {
        FirebaseClient firebase;
        public DataBaseManager()
        {
            firebase = new FirebaseClient("https://anodate-ca8b9-default-rtdb.firebaseio.com/");

        }
        
        /*
         * Fonction qui s'occupe d'ajouter un nouveau user a la base de données
         */
        public async Task AddUser(User user)
        {
            await firebase
                .Child("Users")
                .PostAsync(user);
        }

        /*
         * Fonction qui s'occupe de mettre a jour les infos
         * d'un user dans ma BD
         */
        public async Task UpdateUser(User user)
        {
            await firebase.Child("Users").Child(user.Key)
                .PutAsync(user); 
        }


        /*
         * Fonction qui recupere tous les users de mon DB
         * Peut etre preferable de retourner un user en particulier et non toutes
         * la base de données??
         * Utilisation Proxy si l'info a deja ete recherche dans la BD
         */
        public async Task<List<User>> GetAllUsers()
        {
            return (await firebase
                .Child("Users")
                .OnceAsync<User>()).Select(item => new User(
                item.Object.nom, item.Object.prenom, item.Object.email, item.Object.mdp ,item.Key,item.Object.loisirs, item.Object.anecdotes)
           /* {
                nom = item.Object.nom,
                prenom = item.Object.prenom,
                email = item.Object.email,
                mdp = item.Object.mdp
            }*/).ToList();
        }

        
        /*
         * Fonction qui retourne un user de ma BD.
         * Cette fonction est appelée lorsqu'on est sur
         * que cet user existe dans la BD
         */
        public User getUser(List<User> mesUsers, string emailUser)
        {
            User monUser = new User("", "", "", "","","","");
            foreach (var user in mesUsers)
            {
                if (user.email == emailUser)
                {
                    monUser = user;
                }
            }
            return monUser;
        }

        /*
         * Fonction qui verifie lors du sign up d'un nouveau user , s'il n'existe
         * pas deja un user avec l'email que le nouveau user essaie de sign
         */
        public bool isEmailAlreadyInUsage(string emailUser, List<User> mesUsers)
        {
            bool isEmailInUsage = false;
            foreach (var user in mesUsers)
            {
                if (user.email == emailUser)
                {
                    isEmailInUsage = true;
                }  
            }
            return isEmailInUsage;
        }
        
        
        /*
         * On verifie si l'email et le mdp entre par le user lors de l'etape de
         * sign in existe dans la BD mais aussi est ce qu'ils appartiennent les 2
         * a la meme entrée; Si oui le user est valide.
         */
        public bool isUserValid(string emailUser, string mdp, List<User> mesUsers)
        {
            bool canUserConnect = false;
            foreach (var user in mesUsers)
            {
                if (user.email == emailUser && user.mdp == mdp)
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