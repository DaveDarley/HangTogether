using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace HangTogether.ServerManager
{
    public class User
    {
        public User(string nom, string prenom, string email, string mdp,string Key,string loisirs, string anecdotes, string saltToEncryptMdp, string isUserReadMessage)
        {
            this.nom = nom;
            this.prenom = prenom;
            this.email = email;
            this.mdp = mdp;
            this.Key = Key;
            this.loisirs = loisirs;
            this.anecdotes = anecdotes;
            this.saltToEncryptMdp = saltToEncryptMdp;
            this.isUserReadMessage = isUserReadMessage;
        }

        public string nom { get; set; }
        public string prenom { get; set; }
        public string email { get; set; }
        public string mdp { get; set; }

        public string loisirs { get; set; }
        
        public string Key { get; set; } //To Store ID

        public string anecdotes { get; set; }

        public string saltToEncryptMdp { get; set; }

        public string isUserReadMessage { get; set; }


    }
}