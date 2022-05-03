using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;

namespace HangTogether.ServerManager
{
    
    public class GestionChoixLoisirsUser
    {
        public FirebaseClient firebase;
        public  string FirebaseClient = "https://anodate-ca8b9-default-rtdb.firebaseio.com/";
        public  string FrebaseSecret = "17hN90bHf0ROF4BDSUEsrBTw6AFvuFMe6n3sBFTS";
        
        public GestionChoixLoisirsUser()
        {
            firebase = new FirebaseClient(FirebaseClient,
                new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(FrebaseSecret) });
        }


        public async void deleteChoixUser(ChoixLoisirsUser choixLoisirsUser)
        {
            await firebase.Child("ChoixLoisirsUser").Child(choixLoisirsUser.id).DeleteAsync();
        }

        /*
         * Fonction est appelé qd un user selectionne un choix alors c'est sure le table choixuser
         * contient au moins un choix (A ameliorer francais)
         * i.e choix user sera jamais null
         */
        public async Task<ChoixLoisirsUser> getChoixUser(User user, string nomLoisir)
        {
            ChoixLoisirsUser choixUser = null;
            var firebaseObjectChoixUser = (await firebase.Child("ChoixLoisirsUser").OnceAsync<ChoixLoisirsUser>())
                .AsEnumerable().Where(choixuser =>
                    choixuser.Object.idUser == user.id && choixuser.Object.loisir.nom == nomLoisir);
            if (firebaseObjectChoixUser.Count() != 0)
            {
                choixUser = firebaseObjectChoixUser.First().Object;
                choixUser.id = firebaseObjectChoixUser.First().Key;
            }
            return choixUser;
        }




    }
}