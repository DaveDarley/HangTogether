using HangTogether.ServerManager;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HangTogether
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilUser : ContentPage
    {
        private User user;
        public ProfilUser()
        {
           // InitializeComponent();
            

            
        }

        /*
         * Ici je gere ou aller lorsque le user
         * a cliqué sur un element du menu
         */
        public static async  void GestionClickMenu(string elementClique, User user)
        {
            switch (elementClique)
            {
             case   "pote":
                 Application.Current.MainPage = new NavigationPage(new ChooseAndModifyInterests(user));
                 break;
             case   "loisirs":
                 Application.Current.MainPage = new NavigationPage(new ChooseAndModifyInterests(user));
                 break;
             case   "messages":
                 //await Navigation.PushAsync(new Contacts(user));
                 Application.Current.MainPage = new NavigationPage(new Contacts(user));

                 break;
            }
        }

    }
}