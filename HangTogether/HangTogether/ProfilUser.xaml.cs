using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HangTogether
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilUser : ContentPage
    {
        public ProfilUser()
        {
           // InitializeComponent();
            

            
        }

        /*
         * Ici je gere ou aller lorsque le user
         * a cliqué sur un element du menu
         */
        public static void GestionClickMenu(string elementClique)
        {
            switch (elementClique)
            {
             case   "pote":
                 Application.Current.MainPage = new NavigationPage(new ChooseAndModifyInterests());
                 break;
             case   "loisirs":
                 Application.Current.MainPage = new NavigationPage(new ChooseAndModifyInterests());
                 break;
             case   "messages":
                 Application.Current.MainPage = new NavigationPage(new DisplayMessages());
                 break;
            }
        }

    }
}