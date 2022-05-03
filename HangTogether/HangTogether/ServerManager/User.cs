

namespace HangTogether.ServerManager
{
    
    public class User
    {
        public User(string nom, string prenom, string email, string mdp, string anecdotes, string saltToEncryptMdp,string id)
        {
            this.nom = nom;
            this.prenom = prenom;
            this.email = email;
            this.mdp = mdp;
            this.id = id;
            this.anecdotes = anecdotes;
            this.saltToEncryptMdp = saltToEncryptMdp;
        }

        public string nom { get; set; }
  
        public string prenom { get; set; }
       
        public string email { get; set; }
        
        public string mdp { get; set; }
        
        public string id { get; set; } //To Store ID
        
        public string anecdotes { get; set; }
        public string saltToEncryptMdp { get; set; }



    }
}