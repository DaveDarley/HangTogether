using Google.Cloud.Firestore;

namespace HangTogether.ServerManager
{
    [FirestoreData]
    public class Loisir
    {
        public Loisir(string nom)
        {
            this.nom = nom;
        }

        [FirestoreProperty]
        public string nom { get; set; }
    }
}