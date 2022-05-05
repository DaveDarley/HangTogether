using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;

namespace HangTogether.ServerManager
{
    public class TableLoisirsManager
    {
        private static string[] tsLesLoisirs =
        {
            "Video Games", "Artificial Intelligence","Theater",
            "Archery", "Caligraphy", "Travelling", "Photography",
            "Yoga", "Dance", "Art", "Making music", "Volunteering",
            "Blogging", "Podcast", "Marketing", "Puzzles", "Golf",
            "Running", "Badminton", "Pilates", "Volleyball", "Ice Skating",
            "darts", "Roller Skating", "Bowling", "Surfing", "Ice Hockey",
            "Baseball", "Rock Climbing", "Karate", "Fishing","Fencing",
            "Gym", "Weight Lifting", "Kickboxing", "Oil Painting", "Sculpture",
            "Doodling", "Poetry", "Magic", "Acting", "Going to museums", "Sudoku",
            "Brain teasers", "Singing", "LEGO", "Dollhouses" ,"Dolls", "Anime", "Manga",
            "Grilling", "Bingo", "Sudoku", "Card Games", "Chess", "Judo", "Board Games",
            "Wood burning", "Wood carving", "Sightseeing", "drawing"
        };
        
        public  FirebaseClient db;
        public  string FirebaseClient = "https://anodate-ca8b9-default-rtdb.firebaseio.com/";
        public  string FrebaseSecret = "17hN90bHf0ROF4BDSUEsrBTw6AFvuFMe6n3sBFTS";
        public TableLoisirsManager()
        {
            db = new FirebaseClient(FirebaseClient,
                new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(FrebaseSecret) });
        }


        /*
         * Lorsque mon application est lancé pour la premiere fois, ma base de données est vide
         * et donc j'appele cette fonction pour créer ma table de loisirs(C'est la seule fois que cette
         * fonction sera appelé)
         */
        public  List<Loisir> createInterestsObjects()
        {
            List<Loisir> loisirs = new List<Loisir>();
            for (int i = 0; i<tsLesLoisirs.Length; i++) {
                var ticks = DateTime.Now.Ticks;
                var guid = Guid.NewGuid().ToString();
                var uniqueSessionId = ticks.ToString() +'-'+ guid; //guid created by combining ticks and guid
                loisirs.Add(new Loisir(tsLesLoisirs[i].ToUpper(),uniqueSessionId));
            }
            return loisirs;
        }
        
        /*
         * Fonction qui s'occupe d'initialiser ma table de Loisirs dans ma BD si elle
         * n'existe pas encore
         */
        public  async void createInterestsCollection()
        {
            var tableLoisirs = (await db.Child("Loisirs")
                .OnceAsync<User>()).AsEnumerable().ToList();

             int isTableLoisirsVide = tableLoisirs.Count();

             if (isTableLoisirsVide == 0)
             {
                 List<Loisir> loisirsDeDepart = createInterestsObjects();
                 foreach (var loisir in loisirsDeDepart)
                 {
                     await db.Child("Loisirs").Child(loisir.nom).PutAsync(loisir);
                 }
             }
        }
        
        /*
         * Fonction qui verifie qd un utilisateur ajoute un loisir(i.e ajoute a la table Loisir)
         * si le loisir existe pas deja dans ma BD.
         * Devrait pouvoir dire que LOISIR , LOISIRS sont pareil par (lemmatization et stemming??)
         * mais le ft pas (A modifier pour une prochaine version)
         */
        
        // How to use OrderBy() method:
        // https://stackoverflow.com/questions/71953262/orderby-property-not-working-xamarin-firebase
        public async void addInterests(User user, string nom)
        {
            GestionChoixLoisirsUser gestionChoixLoisirsUser = new GestionChoixLoisirsUser();
            var loisirSiExist = (await db.Child("Loisirs").OrderByKey().StartAt(nom).EndAt(nom)
                .LimitToFirst(1).OnceAsync<Loisir>()).ToList();

            if (loisirSiExist.Count() == 0)
                /*
                 * La table totale de loisirs a deja ce 
                 */
            {
                var ticks = DateTime.Now.Ticks;
                var guid = Guid.NewGuid().ToString();
                var uniqueSessionId = ticks.ToString() +'-'+ guid; //guid created by combining ticks and guid
                
                Loisir loisir = new Loisir(nom, uniqueSessionId);
                await db.Child("Loisirs").Child(nom).PutAsync(loisir);

                gestionChoixLoisirsUser.addChoixUser(user, nom);
            }
            else
            {
                gestionChoixLoisirsUser.addChoixUser(user, nom);
            }
        }

        /*
         * Fonction qui retourne tous les loisirs existants de ma BD;
         * Si la liste devient longue, elle prendra bcp de temps mais on a pas vraiment le choix
         */
        public async Task<List<Loisir>> getAllInterests()
        {
            var allInterests = (await db.Child("Loisirs")
                    .OnceAsync<Loisir>()).Select(loisir => loisir.Object).ToList();
            return allInterests;
        }

        // A tester
        public async Task<Loisir> getLoisir(string nom)
        {
            Loisir loisirRecherche = null;
            var loisir = (await db.Child("Loisirs").OrderByKey().StartAt(nom).EndAt(nom).LimitToFirst(1)
                .OnceAsync<Loisir>()).ToList();
            if (loisir.Count() != 0)
            {
                loisirRecherche = loisir.First().Object;
            }
            return loisirRecherche;
        }
    }
}