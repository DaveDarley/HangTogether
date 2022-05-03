namespace HangTogether.ServerManager
{
    /*
     * Classe qui permettra de remplir la table qui sert de correspondance
     * entre la table de loisirs mais aussi la table de user
     */
    public class ChoixLoisirsUser
    {
        public ChoixLoisirsUser(string id, Loisir loisir, string idUser)
        {
            this.id = id;
            this.loisir = loisir;
            this.idUser = idUser;
        }

        public string id { get; set; }
        public Loisir loisir { get; set; }
        public string idUser { get; set; }
    }
}