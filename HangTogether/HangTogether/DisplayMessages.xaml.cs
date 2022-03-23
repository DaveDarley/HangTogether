using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;
using Xamarin.Forms.Xaml;

namespace HangTogether
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DisplayMessages : ContentPage
    {
        
        // Ici sera stocke toutes les conversations d'un user
        Dictionary<string,List<string>> listMessagesUser = new Dictionary<string,List<string>>();
        
        // Ici sera stocke les differentes conversations qui vont apparaitre dans la liste de 
        // Message du user
        private List<Frame> userConvo = new List<Frame>();

        public DisplayMessages()
        {
            InitializeComponent();
            invisibleMenu();
            //Test: 
            fullDictionary();
            getAllConversations();
            displayAllConvos();
        }
        
        class Global  
        {  
            public static bool isMenuOpen = false;  
            
        } 
        
        public void invisibleMenu()
        {
            this.frameMenu.TranslationY +=   (this.frameMenu.HeightRequest + 50);
        }

        async void OnTapMenu(Object o, EventArgs e)
        {
            if (Global.isMenuOpen)
            {
                this.frameMenu.TranslateTo(0, this.frameMenu.TranslationY + this.frameMenu.HeightRequest,
                    1000);
                Global.isMenuOpen = false;
            }
            else
            {
                this.frameMenu.TranslateTo(0, this.frameMenu.TranslationY - this.frameMenu.HeightRequest,
                    1000);
                Global.isMenuOpen = true;
            }
        }
        
        // Tesstons en ajoutant nous memes des infos a notre dictionnaire:
        public void fullDictionary()
        {
            for (int i = 0; i<15; i++)
            {
                var nom = "Dave " + i;
                List<string> text = new List<string>{"lala", "je m'en vais dormir", "C'est le matin reveille toi"};
                listMessagesUser.Add(nom,text);
            }
        }

        /*
         * Chaque user a une liste de conversation
         * Soit allConvos = <RecepteurMessage, [ts les messages avec ce recepteur]>
         * On parcours allConvos et je recupere chaque RecepteurMessage et le dernier message
         * entre le user et le recepteur , je cree une frame avec ces 2 infos recuperees et
         * je stocke le frame dans une liste : userConvo
         */
        public void getAllConversations()
        {
            foreach(KeyValuePair<string, List<string>> entry in listMessagesUser)
            {
                var nom = entry.Key;
                var messageToDisplay = entry.Value[entry.Value.Count - 1 ];

                Frame monFrame = new Frame()
                {
                    CornerRadius = 10,
                    BorderColor = Color.Chartreuse,
                    Margin = new Thickness(5,10,5,10),
                    IsVisible = true,
                    Content = new StackLayout()
                    {
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        Orientation = StackOrientation.Vertical,
                        Children =
                        {
                            new Label()
                            {
                                FontAttributes = FontAttributes.Bold,
                                TextColor = Color.Black,
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                HorizontalTextAlignment = TextAlignment.Center,
                                Text = nom
                                
                            },

                            new PancakeView()
                            {
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                CornerRadius = new CornerRadius(20,0,0,40),
                                VerticalOptions = LayoutOptions.FillAndExpand,
                                Border = new Border()
                                {
                                   Color = Color.DarkCyan,
                                   Thickness = 2
                                },
                                Content = new Label()
                                {
                                    TextColor = Color.Black,
                                    HorizontalOptions = LayoutOptions.FillAndExpand,
                                    HorizontalTextAlignment = TextAlignment.Center,
                                    MaxLines = 1,
                                    Padding = new Thickness(20,10,40,10),
                                    Text = messageToDisplay
                                }
                            }
                        }
                    }
                };
                userConvo.Add(monFrame);
            }
        }


        /*
         * Cette fonction prend ts les frame(Convos entre user et un recepteur)
         * de ma liste "userConvo" et affiche ces frames a l'ecran
         */
        public void displayAllConvos()
        {
            List<Frame> listConvos = userConvo;
            var layoutUser = this.Messages;
            for (int i = 0; i < listConvos.Count; i++)
            {
                if (listConvos[i].IsVisible)
                {
                    layoutUser.Children.Add(listConvos[i]);
                }
            }
        }
        
        /*
 * Fonction qui prend le texte entré  par le user dans
 * le searchBar et affiche dans le flexLayout les
 * Frames qui correspond a la recherche (s'il y en a)
 *
 * Losrque User tape texte dans le searchbar, on parcours notre liste de frame
 * si le texte du frame contient le query du user (ou query contient texte du frame) on met le frame.IsVisible
 * a true sinon on le met a false.
 */
        void OnUserSearchChanged(object sender, EventArgs e)
        {
            SearchBar searchBar = (SearchBar)sender;
            string queryUser = searchBar.Text;

            // Si user cherche rien on affiche TOUS les loisirs sur l'ecran
            if (String.IsNullOrEmpty(queryUser))
            {
                for (int i = 0; i<userConvo.Count; i++)
                {
                    userConvo[i].IsVisible = true;
                }
                displayAllConvos();
                return;
            }

            //Cherchons le frame qui contient le query recherché par le user
            for (int i = 0; i<userConvo.Count; i++)
            {
                var stackLayout = (StackLayout) userConvo[i].Content;
                Label nom = (Label) stackLayout.Children[0];
                var textFrame = (nom.Text).ToLower();
                queryUser = queryUser.ToLower();
                
                if (queryUser.Length > textFrame.Length)
                {
                    if (queryUser.Contains(textFrame))
                    {
                        userConvo[i].IsVisible = true;
                        continue;
                    }
                }
                else
                {
                    if (textFrame.Contains(queryUser))
                    {
                        userConvo[i].IsVisible = true;
                        continue;
                    }
                }

                userConvo[i].IsVisible = false;
                
            }
            displayAllConvos();
        }

    }
}