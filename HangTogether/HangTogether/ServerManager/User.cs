using System.Collections.Generic;
using Xamarin.Forms;

namespace HangTogether.ServerManager
{
    public class User
    {
        public User(string nom, string prenom, string email, string mdp,string key)
        {
            this.nom = nom;
            this.prenom = prenom;
            this.email = email;
            this.mdp = mdp;
            this.Key = key;
        }

        public string nom { get; set; }
        public string prenom { get; set; }
        public string email { get; set; }
        public string mdp { get; set; }

        public List<Frame> loisirsUser { get; set; }
        
        public string Key { get; set; } //To Store ID

    }
}