using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Threading.Tasks;
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
                     await db.Child("Loisirs").PostAsync(loisir);
                 }
             }
        }
        
        /*
         * Fonction qui verifie qd un utilisateur ajoute un loisir(i.e ajoute a la table Loisir)
         * si le loisir existe pas deja dans ma BD.
         * Devrait pouvoir dire que LOISIR , LOISIRS sont pareil par (lemmatization et stemming??)
         * mais le ft pas (A modifier pour une prochaine version)
         */
        public async void addInterests(User user, string nom)
        {
            var Loisir = (await db.Child("Loisirs")
                .OnceAsync<Loisir>()).AsEnumerable().Where(loisir => loisir.Object.nom == nom.ToUpper()).ToList();

            if (Loisir.Count() == 0)
            {
                var ticks = DateTime.Now.Ticks;
                var guid = Guid.NewGuid().ToString();
                var uniqueSessionId = ticks.ToString() +'-'+ guid; //guid created by combining ticks and guid
                
                Loisir loisir = new Loisir(nom, uniqueSessionId);
                await db.Child("Loisirs").PostAsync(loisir);

                ChoixLoisirsUser choixLoisirsUser = new ChoixLoisirsUser(uniqueSessionId, loisir, user.id);
                await db.Child("ChoixLoisirsUser").PostAsync(choixLoisirsUser);
            }
            else
            {
                var ticks = DateTime.Now.Ticks;
                var guid = Guid.NewGuid().ToString();
                var uniqueSessionId = ticks.ToString() +'-'+ guid; //guid created by combining ticks and guid

                Loisir loisir = await getLoisir(nom);
                ChoixLoisirsUser choixLoisirsUser = new ChoixLoisirsUser(uniqueSessionId, loisir, user.id);
                await db.Child("ChoixLoisirsUser").PostAsync(choixLoisirsUser);
            }
        }

        
        public async Task<List<Loisir>> getAllInterests()
        {
            var allInterests = (await db.Child("Loisirs")
                    .OnceAsync<Loisir>()).Select(loisir => loisir.Object).ToList();
            return allInterests;
        }

        public async Task<Loisir> getLoisir(string nom)
        {
            Loisir loisirRecherche = null;
            var isLoisir =  (await db.Child("Loisirs").OnceAsync<Loisir>())
                .AsEnumerable().Where(loisir => loisir.Object.nom == nom);
            if (isLoisir.Count() != 0)
            {
                loisirRecherche = isLoisir.First().Object;
            }

            return loisirRecherche;
        }
    }
}