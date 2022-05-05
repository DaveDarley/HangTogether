using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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


        /*
         * L'id du loisir est pareil que l'id du user qui lui a selectionné
         */
        public async void deleteChoixUser(User user, ChoixLoisirsUser choixLoisirsUser)
        {
            var convertEmail = Convert.ToBase64String(Encoding.ASCII.GetBytes(user.email));
            await firebase.Child("ChoixLoisirsUser").Child(convertEmail).Child(choixLoisirsUser.idChoix).DeleteAsync();
        }

        /*
         * Lorsqu'un user selectionne un loisir(Tape sur un frame loisir de l'ecran), on verifie si il avait pas auparavant
         * selectionne ce loisir(i.e deja mis dans la table ChoixLoisirsUser)
         */
        public async Task<ChoixLoisirsUser> getChoixUser(User user, string nomLoisir)
        {
            var convertEmail = Convert.ToBase64String(Encoding.ASCII.GetBytes(user.email));
            string cleChoixUser = nomLoisir + convertEmail;
            var loisirSiExist = (await firebase.Child("ChoixLoisirsUser").Child(convertEmail).OrderByKey().StartAt(cleChoixUser).EndAt(cleChoixUser)
                .LimitToFirst(1).OnceAsync<ChoixLoisirsUser>()).ToList();
            ChoixLoisirsUser choixUser = null;

            if (loisirSiExist.Count() != 0)
            {
                choixUser = loisirSiExist.First().Object;
            }
            return choixUser;
        }

        public async void addChoixUser(User user, string nomLoisir)
        {
            var convertEmail = Convert.ToBase64String(Encoding.ASCII.GetBytes(user.email));
            string cleChoixUser = nomLoisir + convertEmail;

            TableLoisirsManager tableLoisirsManager = new TableLoisirsManager();
            Loisir loisir = await tableLoisirsManager.getLoisir(nomLoisir);
            ChoixLoisirsUser choixLoisirsUser = new ChoixLoisirsUser(cleChoixUser,user.id,loisir);

         //  await firebase.Child("ChoixLoisirsUser").Child(cleChoixUser).PutAsync(choixLoisirsUser);
           
           await firebase.Child("ChoixLoisirsUser").Child(convertEmail).Child(cleChoixUser).PutAsync(choixLoisirsUser);
        }






    }
}