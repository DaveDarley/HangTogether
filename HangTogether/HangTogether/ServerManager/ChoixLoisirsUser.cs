namespace HangTogether.ServerManager
{
    /*
     * Classe qui permettra de remplir la table qui sert de correspondance
     * entre la table de loisirs mais aussi la table de user
     */
    public class ChoixLoisirsUser
    {
        public ChoixLoisirsUser(string idChoix,string id, Loisir loisir)
        {
            this.loisir = loisir;
            this.id = id;
            this.idChoix = idChoix;
        }

        public Loisir loisir { get; set; }
        public string id { get; set; }

        public string idChoix { get; set; }
    }
}