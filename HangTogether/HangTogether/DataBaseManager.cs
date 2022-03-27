using System.Collections.Generic;
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

        public async Task UpdateUser(User user)
        {
            //var emailUser = user.email;
            await firebase.Child("Users").Child(user.Key)
                .PutAsync(user); 
        }

        /*
         * Cette fonction cree un champ Recover Password dans ma base de données
         * et ajoute {email, verification Code} du user qui veut retrouver son mdp.
         * Qd user change son mdp on efface le champ dans ce tableau et on update
         * champ mdp du user dans table Users
         *
         * Idée: On pouvait mettre champ {string verifCode} dans la table User mais
         * je trouve ca pas de sens qu'un user ait un champ verifCode des le depart
         * vu que cette situation n'arrivera pas a tous les users
         */
        
        
        //Je pense pas que c'est necessaire d'ajouter cette information dans une BD
      /*  public async Task adduserPasswordRecovery(RecoverPasswordUser passwordRecovery)
        {
            await firebase
                .Child("Recover Password")
                .PostAsync(passwordRecovery);
        }

        public async Task<List<RecoverPasswordUser>> getAllUsersRecoveringPassword()
        {
            return (await firebase
                .Child("Recover Password")
                .OnceAsync<RecoverPasswordUser>()).Select(item => new RecoverPasswordUser(
                    item.Object.email, item.Object.verifCode)).ToList();
        }

        
        public RecoverPasswordUser getUserRecoveringPassword(string email, List<RecoverPasswordUser>allUsersRecovering)
        {
            RecoverPasswordUser user = new RecoverPasswordUser("", "");
            foreach (var userInRecovery in allUsersRecovering)
            {
                if (userInRecovery.email == email)
                {
                    user = userInRecovery;
                }
            }

            return user;
        }*/

        /*
         * Fonction qui recuopere tous les users de mon DB
         * Peut etre preferable de retourner un user en particulier et non toutes
         * la base de données??
         */
        public async Task<List<User>> GetAllUsers()
        {
            return (await firebase
                .Child("Users")
                .OnceAsync<User>()).Select(item => new User(
                item.Object.nom, item.Object.prenom, item.Object.email, item.Object.mdp ,item.Key)
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
            User monUser = new User("", "", "", "","");
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
         * Fonction qui s'occupe de mettre a jour la liste de loisirs
         * d'un User dans la Base de données
         */
        public async void updateInfosUser(User monUser, List<Frame>nouveauxLoisirs)
        {
            monUser.loisirsUser = nouveauxLoisirs;
            await AddUser(monUser);
        }


    }
}