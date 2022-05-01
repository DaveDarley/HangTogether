using System;
using System.Collections.Generic;
using Google.Cloud.Firestore;
using Firebase.Database;

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
        
        private static FirestoreDb db;
        public TableLoisirsManager()
        {
            db = FirestoreDb.Create("https://anodate-ca8b9-default-rtdb.firebaseio.com/");
        }

        /*
         * Lorsque mon application est lancé pour la premiere fois, ma base de données est vide
         * et donc j'appele cette fonction pour créer ma table de loisirs(C'est la seule fois que cette
         * fonction sera appelé)
         */
        public static void createInterestsObjects()
        {
            List<Loisir> loisirs = new List<Loisir>();
            for (int i = 0; i<tsLesLoisirs.Length; i++) {
                loisirs.Add(new Loisir(tsLesLoisirs[i]));
            }
            createInterestsCollection(loisirs);
        }

        public static async void createInterestsCollection(List<Loisir> allInterests)
        {
            foreach (var interest in allInterests)
            {
                DocumentReference addedDocRef = db.Collection("cities").Document();
                Console.WriteLine("Added document with ID: {0}.", addedDocRef.Id);
                await addedDocRef.SetAsync(interest);
            } 
        }
    }
}