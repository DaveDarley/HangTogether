using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        // List qui contient tous les loisirs du user sous forme de Frame
        private List<Frame> allFrame = new List<Frame>();

        // ici est stocké les choix de loisirs du User
        private List<Frame> choixUser = new List<Frame>();
        
        // ici est stocké les anecdotes du user
        private string textePresentationUser;

        private bool isMenuOpen = false;

        public ChooseAndModifyInterests()
        {
            InitializeComponent();
            CreateLoisirsFrame();
            addLoisirsToLayout();
            invisibleMenu();
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
                    HasShadow = true,
                    IsVisible = true,
                    Margin = new Thickness(0,4,0,4),
                    Content = new Label()
                    {
                        Text = loisirsUser[i],
                        TextColor = Color.Black,
                        HorizontalOptions = LayoutOptions.StartAndExpand,
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
                RemoveUserChoice(monFrame);
            }
            else
            {
                monFrame.BackgroundColor = Color.Black;
                var monLabel = (Label) monFrame.Content;
                monLabel.TextColor= Color.White;
                choixUser.Add(monFrame);
            }
        }

        /*
         * Fonction qui permet d'enlever une frame dans la liste de
         * choix du User qd celui-ci deselectionne le loisir;
         * Elle recoit en parametre la frame qu'elle est censée enlever
         * (Celle-ci a ete deselectionnée par le user)
         */
        public void RemoveUserChoice(Frame toRemove)
        {
            var labelFrame = (Label)toRemove.Content;
            string titreLoisir = labelFrame.Text;
            
            for (int i = 0; i < choixUser.Count; i++)
            {
                var possibleFrameToRemove = (Label)choixUser[i].Content;
                string titrePossibleFrameToRemove = possibleFrameToRemove.Text;

                if (titreLoisir == titrePossibleFrameToRemove)
                {
                   choixUser.RemoveAt(i);
                   return;
                }
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

        public void invisibleMenu()
        {
            this.frameMenu.TranslationY +=   (this.frameMenu.HeightRequest+50);
        }
        
        /*
         * Fonction qui s'occupe de l'apparition du menu
         * sur l'ecran
         */

        async void OnTapMenu(Object o, EventArgs e)
        {
            if (isMenuOpen)
            {
                await this.frameMenu.TranslateTo(0, this.frameMenu.TranslationY + this.frameMenu.HeightRequest,
                    1000);
                isMenuOpen = false;
            }
            else
            {
                await this.frameMenu.TranslateTo(0, this.frameMenu.TranslationY - this.frameMenu.HeightRequest,
                    1000);
                isMenuOpen = true;
            }
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
                    HasShadow = true,
                    IsVisible = true,
                    Margin = new Thickness(0,4,0,4),
                    Content = new Label()
                    {
                        Text = loisirsAjoute,
                        TextColor = Color.White,
                        HorizontalOptions = LayoutOptions.StartAndExpand,
                        LineBreakMode = LineBreakMode.WordWrap
                       
                        
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
                choixUser.Add(frame);
                addLoisirsToLayout();
            }
        }

        
         async void OnTapRecherche(object sender, EventArgs args)
        {
            
            if (validateUserChoice())
            {
                await Navigation.PushAsync(new DisplayPotentialFriends()); 
            }
            else
            {
                await DisplayAlert ("Alert", "Veuillez indiquer au moins un loisir et une anecdote", "OK");
            }

        }

         /*
          * Dans ces 4 prochaines fonctions , je gere lorsque le
          * user clique sur un element du menu
          */
          void OnTapFindFriends(object o, EventArgs e)
         {
             ProfilUser.GestionClickMenu("pote");
         }
           void OnTapChooseInterests(object o, EventArgs e)
         {
             ProfilUser.GestionClickMenu("loisirs");
         }
           void OnTapViewMessages(object o, EventArgs e)
         {
             ProfilUser.GestionClickMenu("messages");
         }
         async void OnTapDeactivateAccount(object o, EventArgs e)
         {
             bool desactiverCompte = await DisplayAlert ("Desactivation Compte", "Etes vous sur de vouloir desactiver votre compte", "Oui", "Non");
             if (desactiverCompte)
             {
                 // Supprmimer user de la base de données
                 Application.Current.MainPage = new NavigationPage(new LogInSignUp());
             }
         }
         
         
        
        
        /*
         * Qd user clique sur le boutton de recherche de nouveauPote;
         * Il faut s'assurer que le User a fait au moins un choix de loisirs
         * mais a aussi inscrit au moins une acdotes;
         * On savegarde ensuite les loisirs choisies par le user mais aussi les
         * anecdotes du User dans les variables qui leurs correspondent
         */
        public bool validateUserChoice()
        {
            var canUserGoToNextPage = true;
            var lesAnecdotesDuUser = this.anecdotesUser.Text;
            if (choixUser.Count > 0)
            {
            }
            else
            {
                canUserGoToNextPage = false;
            }

            if (!String.IsNullOrEmpty(lesAnecdotesDuUser))
            {
                textePresentationUser = lesAnecdotesDuUser;
            }
            else
            {
                canUserGoToNextPage = false;
            }
            return canUserGoToNextPage;
        }


        
    }
}