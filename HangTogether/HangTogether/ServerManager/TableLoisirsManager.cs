using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

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
        
        public  FirestoreDb db;
        public TableLoisirsManager()
        {
            db = FirestoreDb.Create("hangtogether-edc71");
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
                loisirs.Add(new Loisir(tsLesLoisirs[i].ToUpper()));
            }
            return loisirs;
        }
        
        /*
         * Fonction qui s'occupe d'initialiser ma table de Loisirs dans ma BD si elle
         * n'existe pas encore
         */
        public  async void createInterestsCollection()
        {
            Query isLoisirsTableExists = db.Collection("Loisirs").Limit(1);
            QuerySnapshot isLoisirsTableExistsSnapshot = await isLoisirsTableExists.GetSnapshotAsync();
            if (isLoisirsTableExistsSnapshot.Documents.Count == 0)
            {
                List<Loisir> allInterests = createInterestsObjects();
                foreach (var interest in allInterests)
                {
                    // DocumentReference addedDocRef = db.Collection("cities").Document();
                    // Console.WriteLine("Added document with ID: {0}.", addedDocRef.Id);
                    DocumentReference addedDocRef = db.Collection("Loisirs").Document(interest.nom);
                    await addedDocRef.SetAsync(interest);
                }
            }
 
        }

        
        /*
         * Fonction qui verifie qd un utilisateur ajoute un loisir(i.e ajoute a la table Loisir)
         * si le loisir existe pas deja dans ma BD.
         * Devrait pouvoir dire que LOISIR , LOISIRS sont pareil par (lemmatization et stemming??)
         * mais le ft pas (A modifier pour une prochaine version)
         */
        public async void addInterests(string nom)
        {
            string loisir = nom.ToUpper();
            // check si loisir existe deja :
            // Si oui on ft rien 
            // Sinon on l'ajoute
            DocumentReference docRef = db.Collection("Loisirs").Document(loisir);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (!snapshot.Exists)
            {
                Loisir nouveauLoisir = new Loisir(nom);
                await docRef.SetAsync(nouveauLoisir);
            }
        }

        
        public async Task<List<Loisir>> getAllInterests()
        {
            List<Loisir> allInterests = new List<Loisir>();
            // recuperation du document dont le champ email == emailUser
            Query capitalQuery = db.Collection("Loisirs");
            QuerySnapshot capitalQuerySnapshot = await capitalQuery.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in capitalQuerySnapshot.Documents)
            {
                Loisir loisir = documentSnapshot.ConvertTo<Loisir>();
                allInterests.Add(loisir);
            }

            return allInterests;
        }
    }
}