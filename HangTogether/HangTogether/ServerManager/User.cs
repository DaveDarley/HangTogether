using System.Collections.Generic;
using System.Collections.ObjectModel;
using Google.Cloud.Firestore;
using Xamarin.Forms;

namespace HangTogether.ServerManager
{
    [FirestoreData]
    public class User
    {
        public User(string nom, string prenom, string email, string mdp,string loisirs, string anecdotes, string saltToEncryptMdp,string id)
        {
            this.nom = nom;
            this.prenom = prenom;
            this.email = email;
            this.mdp = mdp;
            this.id = id;
            this.loisirs = loisirs;
            this.anecdotes = anecdotes;
            this.saltToEncryptMdp = saltToEncryptMdp;
        }

        [FirestoreProperty] 
        public string nom { get; set; }
        
        [FirestoreProperty]
        public string prenom { get; set; }
        
        [FirestoreProperty]
        public string email { get; set; }
        
        [FirestoreProperty]
        public string mdp { get; set; }
        
        [FirestoreProperty]
        public string loisirs { get; set; }
        
        [FirestoreProperty]
        public string id { get; set; } //To Store ID
        
        [FirestoreProperty]
        public string anecdotes { get; set; }
        
        [FirestoreProperty]
        public string saltToEncryptMdp { get; set; }


    }
}