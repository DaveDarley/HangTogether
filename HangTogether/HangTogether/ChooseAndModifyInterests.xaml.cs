using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Contracts;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HangTogether
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChooseAndModifyInterests : ContentPage
    {
        private string[] loisirsUser =
        {
            "Video Games", "Artificial Intelligence","Theater",
            "Archery", "Caligraphy", "Travelling", "Photography",
            "Yoga", "Dance", "Art", "Making music", "Volunteering",
            "Blogging", "Podcast", "Marketing", "Puzzles", "Golf",
            "Running", "Badminton", "Pilates", "Volleyball", "Ice Skating",
            "darts", "Roller Skating", "Bowling", "Surfing", "Ice Hockey",
            "Baseball", "Rock Climbing", "Karate", "Fishing","Fencing",
            "Gym", "Weight Lifting", "Kickboxing", "Oil Painting", "Sculpture",
            "Doodling", "Poetry", "Magic", "Acting", "Going to museums", "Sudoku",
            "Brain teasers", "Singing", "LEGO", "Dollhouses" ,"Dolls", "Anime", "Manga",
            "Grilling", "Bingo", "Sudoku", "Card Games", "Chess", "Judo", "Board Games",
            "Wood burning", "Wood carving", "Sightseeing", "drawing"
        };

        // List qui contient les loisirs du user sous forme de Frame
        private List<Frame> allFrame = new List<Frame>();

        public ChooseAndModifyInterests()
        {
            InitializeComponent();
            CreateLoisirsFrame();
            addLoisirsToLayout();
        }

        /*
         * Fonction qui prend une liste de loisirs sous forme
         * de String et crée une liste de Frame
         */
        public void CreateLoisirsFrame()
        {
            for (int i = 0; i < loisirsUser.Length; i++)
            {
                Frame frame = new Frame()
                {
                    BackgroundColor = Color.White,
                    CornerRadius = 30,
                    Margin = new Thickness(5,5,5,0),
                    HasShadow = true,
                    IsVisible = true,
                    Content = new Label()
                    {
                        Text = loisirsUser[i],
                        TextColor = Color.Black,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        VerticalOptions = LayoutOptions.FillAndExpand
                    }
                };
                
                // TapEvent on frame :
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += (s, e) => {
                    // handle the tap
                    OnTapFramFrame(s,e);
                };
                frame.GestureRecognizers.Add(tapGestureRecognizer);
                allFrame.Add(frame);
            }
        }
        

        /*
         * Fonction qui permet de savoir les choix du User
         * Qd user selectionne Frame(bg devient noir et textcolor Blanc)
         * Si bg deja noir alors user veux deselectionner son choix.
         * On sauvegarde dans un tableau les choix de l'user
         * pour les stocker ensuite dans la Base de données
         */
        public void OnTapFramFrame(Object s, EventArgs e)
        {
            var monFrame = (Frame) s;
            // Verifions si le frame a deja ete selectionné ou pas 
            if (monFrame.BackgroundColor == Color.Black)
            {
                monFrame.BackgroundColor = Color.White;
                var monLabel = (Label) monFrame.Content;
                monLabel.TextColor= Color.Black;
            }
            else
            {
                monFrame.BackgroundColor = Color.Black;
                var monLabel = (Label) monFrame.Content;
                monLabel.TextColor= Color.White;
            }
        }

        
        /*
         * Fonction qui prend une liste de Frame et un FlexLayout
         * et dessine tous les frames ,dont l'attribut IsVisble est true, sur le FlexLayout
         */
        public void addLoisirsToLayout()
        {
            List<Frame> listInterests = allFrame;
            var layoutUser = this.layoutInterest;
            for (int i = 0; i < listInterests.Count; i++)
            {
                if (listInterests[i].IsVisible)
                {
                    layoutUser.Children.Add(listInterests[i]);
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
                for (int i = 0; i<allFrame.Count; i++)
                {
                    allFrame[i].IsVisible = true;
                }
                addLoisirsToLayout();
                return;
            }

            //Cherchons le frame qui contient le query recherché par le user
            for (int i = 0; i<allFrame.Count; i++)
            {
                var labelFrame = (Label) allFrame[i].Content;
                var textFrame = (labelFrame.Text).ToLower();
                queryUser = queryUser.ToLower();
                
                if (queryUser.Length > textFrame.Length)
                {
                    if (queryUser.Contains(textFrame))
                    {
                        allFrame[i].IsVisible = true;
                        continue;
                    }
                }
                else
                {
                    if (textFrame.Contains(queryUser))
                    {
                        allFrame[i].IsVisible = true;
                        continue;
                    }
                }

                allFrame[i].IsVisible = false;
                
            }
            addLoisirsToLayout();
        }

        async void OnTapMenu(Object s, EventArgs e)
        {
            
        }
        
        // Lorsque un user ne trouve pas son loisirs dans la liste de loisirs
        // il va l'ajouter lui meme (On peut qd meme pas enumerer tous les loisirs :) )
        async void OnAddInterests(Object s, EventArgs e)
        {
            string loisirsAjoute = await DisplayPromptAsync("Ajout Loisirs", "Veuillez ajouter votre loisirs", keyboard: Keyboard.Text);
            if (!String.IsNullOrEmpty(loisirsAjoute))
            {
                Frame frame = new Frame()
                {
                    BackgroundColor = Color.Black,
                    CornerRadius = 30,
                    Margin = new Thickness(5,5,5,0),
                    HasShadow = true,
                    IsVisible = true,
                    Content = new Label()
                    {
                        Text = loisirsAjoute,
                        TextColor = Color.White,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        VerticalOptions = LayoutOptions.FillAndExpand
                    }
                };
                
                // Ajout d'un eventListener sur le nouveau frame cree :
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += (obj, ev) => {
                    // handle the tap
                    OnTapFramFrame(obj,ev);
                };
                frame.GestureRecognizers.Add(tapGestureRecognizer);
                // on ajout ce nouveau frame a la liste des anciens frames
                allFrame.Add(frame);
                addLoisirsToLayout();
            }
        }






    }
}