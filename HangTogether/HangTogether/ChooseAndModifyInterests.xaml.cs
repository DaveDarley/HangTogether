using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HangTogether.ServerManager;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HangTogether
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChooseAndModifyInterests : ContentPage
    {
        private string[] tsLesLoisirs =
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

        // List qui contient tous les loisirs sous forme de Frame
        public List<Frame> loisirsEnFrameADessiner = new List<Frame>();

        // List qui contient les loisirs selectionnées par le user
       public List<Frame> choixUserEnFrameADessiner = new List<Frame>();

        // List qui contient une liste de loisirs selectionnes par le user
        public List<string> choixDeLutilisateur = new List<string>();


        private bool isMenuOpen = false;

        // LE USER EN QUESTION
        private User user;

        public ChooseAndModifyInterests(User activeUser)
        {
            InitializeComponent();
            user = activeUser;
            initializeListsOfInterests();
            
            addLoisirsToLayout(choixUserEnFrameADessiner);

            
            invisibleMenu();
        }

        /*
         * Soit un User qui a deja fait son choix de loisirs et anecdotes,
         * lorsque l'utilisateur se reconnecte on recupere ses choix de la BD
         * et on les affiche de nouveau.
         */
        public void initializeListsOfInterests()
        {
            if (! String.IsNullOrEmpty(user.loisirs))
            {
                var loisirs = user.loisirs.Split(',');
                choixDeLutilisateur = new List<string>(loisirs);
                choixUserEnFrameADessiner = CreateLoisirsFrame(loisirs);
            }
            if (! String.IsNullOrEmpty(user.anecdotes))
            {
                this.anecdotesUser.Text = user.anecdotes;
            }
            loisirsEnFrameADessiner = CreateLoisirsFrame(tsLesLoisirs);
        }
        
        
        /*
         * Fonction qui prend une liste de loisirs sous forme
         * de String et retourne une liste de Frame
         */
        public List<Frame> CreateLoisirsFrame(string [] loisirs)
        {
            List<Frame> frameADessinerSurlayout = new List<Frame>();
            for (int i = 0; i < loisirs.Length; i++)
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
                        Text = loisirs[i],
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
                frameADessinerSurlayout.Add(frame);
            }
            return frameADessinerSurlayout;
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
            {   // Utilisateur fait un choix de loisir
                monFrame.BackgroundColor = Color.Black;
                var monLabel = (Label) monFrame.Content;
                monLabel.TextColor= Color.White;
                
                string loisirChoisi = monLabel.Text;
                choixDeLutilisateur.Add(loisirChoisi);
                
                choixUserEnFrameADessiner.Add(monFrame);
                updateInterestsUser(); // user a ajoute un loisir dans sa liste de loisirs alors on Update la BD
            }
        }
        

        /*
         * Fonction qui s'occupe de mettre a jour la liste de loisirs
         * du user (Qd user ajoute, selectionne ou deselectionne un loisir)
         */
        public async void updateInterestsUser()
        {
            DataBaseManager dataBaseManager = new DataBaseManager();
            List<User> allUsers = await dataBaseManager.GetAllUsers();
            User toUpdate = dataBaseManager.getUser(allUsers, user.email);

            string loisirsUpdate = choixDeLutilisateur.Count == 0  
               ? "" 
               : String.Join(",", choixDeLutilisateur.ToArray());

            toUpdate.loisirs = loisirsUpdate;
            toUpdate.anecdotes = String.IsNullOrEmpty(anecdotesUser.Text) ? "" : anecdotesUser.Text;
            
            this.user = toUpdate;
            await dataBaseManager.UpdateUser(toUpdate);
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
            string titreLoisirDeselectionne = labelFrame.Text;
            
            for (int i = 0; i < choixUserEnFrameADessiner.Count; i++)
            {
                var possibleFrameToRemove = (Label)choixUserEnFrameADessiner[i].Content;
                string titrePossibleFrameToRemove = possibleFrameToRemove.Text;

                if (titreLoisirDeselectionne == titrePossibleFrameToRemove)
                { 
                    choixUserEnFrameADessiner.RemoveAt(i);

                }
            }

            for (int i = 0; i < choixDeLutilisateur.Count; i++)
            {
                if (choixDeLutilisateur[i] == titreLoisirDeselectionne)
                {
                    choixDeLutilisateur.RemoveAt(i);
                }
            }
            updateInterestsUser(); // user a enleve un loisir dans sa liste de loisirs alors on Update la BD
           
        }


        /*
         * Cette fonction recoit en parametre la liste de choix de loisirs d'un user
         * Si un loisir appartient a la liste de loisirs d'un user mais aussi appartient
         * a la liste globale de loisirs (Allframe) on le dessine et on l'enleve de la liste de loisirs du user.
         * Si un loisir appartient juste a Allframe et l'attribut IsVisible == true
         * alors on le dessine.
         * En dernier on parcours la liste de choix du User et on dessine tous les frames
         * qu'elle contient
         */
        public void addLoisirsToLayout(List<Frame> choixUsers)
        {
            var layoutUser = this.layoutInterest;
            if (choixUsers.Count > 0)
            {
                var frameChoiceUser = choixUserEnFrameADessiner;
                foreach (var loisirs in loisirsEnFrameADessiner)
                {
                    var textFrameLoisirsDeDepart = ((Label) loisirs.Content).Text;
                    for (int i = 0; i<frameChoiceUser.Count; i++)
                    {
                        var textFrameChoixUser = ((Label) frameChoiceUser[i].Content).Text;
                        if (textFrameLoisirsDeDepart == textFrameChoixUser)
                        {
                            var labelFrame = (Label)loisirs.Content;
                            labelFrame.TextColor = Color.White;
                            loisirs.BackgroundColor = Color.Black;
                            frameChoiceUser.RemoveAt(i);
                        }
                    }

                    layoutUser.Children.Add(loisirs);
                }
                foreach (var choiceUser in frameChoiceUser)
                {
                    var labelFrame = (Label)choiceUser.Content;
                    labelFrame.TextColor = Color.White;
                    choiceUser.BackgroundColor = Color.Black;
                    layoutUser.Children.Add(choiceUser);
                }

            }
            else
            {
                foreach (var availableInterests in loisirsEnFrameADessiner)
                {
                    if (availableInterests.IsVisible)
                    {
                        layoutUser.Children.Add(availableInterests);
                    }
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
                for (int i = 0; i<loisirsEnFrameADessiner.Count; i++)
                {
                    loisirsEnFrameADessiner[i].IsVisible = true;
                }
                addLoisirsToLayout(choixUserEnFrameADessiner); 
                return;
            }

            //Cherchons le frame qui contient le query recherché par le user
            for (int i = 0; i<loisirsEnFrameADessiner.Count; i++)
            {
                var labelFrame = (Label) loisirsEnFrameADessiner[i].Content;
                var textFrame = (labelFrame.Text).ToLower();
                queryUser = queryUser.ToLower();
                
                if (queryUser.Length > textFrame.Length)
                {
                    if (queryUser.Contains(textFrame))
                    {
                        loisirsEnFrameADessiner[i].IsVisible = true;
                        continue;
                    }
                }
                else
                {
                    if (textFrame.Contains(queryUser))
                    {
                        loisirsEnFrameADessiner[i].IsVisible = true;
                        continue;
                    }
                }

                loisirsEnFrameADessiner[i].IsVisible = false;
                
            }
            addLoisirsToLayout(choixUserEnFrameADessiner);
        }
        
        /*
         * Fonction qui cree un nouveau frame lorsqu'un user ajoute un loisirs
         * Cette fonction ajoute ce nouveau Frame dans la liste de loisirs selectionnes
         * par le user(choixUser) mais aussi dans la liste de toutes les loisirs (allFrame).
         */
        async void OnAddInterests(Object s, EventArgs e)
        {
            string loisirsAjoute = await DisplayPromptAsync("Ajout de Loisirs", "Veuillez ajouter votre loisir", keyboard: Keyboard.Text);
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
                loisirsEnFrameADessiner.Add(frame);
                choixUserEnFrameADessiner.Add(frame);
                choixDeLutilisateur.Add(loisirsAjoute);
                updateInterestsUser(); // on update la liste de choix du user dans ma BD
                addLoisirsToLayout(choixUserEnFrameADessiner);
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
            if (String.IsNullOrEmpty(user.loisirs))
            {
                canUserGoToNextPage = false;
            }

            if (String.IsNullOrEmpty(this.anecdotesUser.Text))
            {
                canUserGoToNextPage = false;
            }
            return canUserGoToNextPage;
        }
        
        /*
         * Fonction qui met a jour les anecdotes du user dans la BD au fur et a mesure
         * que le user est entrain de taper ses anecdotes
         */
        private void Editor_TextChanged(object sender, EventArgs e)
        {
            updateInterestsUser();
        }
        
        /*
         * Qd user va chercher la liste de personnes qui ont les memes
         * centres d'interet que lui, on enregistre sa liste de loisirs
         * dans la base de donnees, pour ensuite lui afficher que les gens
         * que les gens qui ont des centre d'interet en commun avec lui.
         */
        async void OnTapRecherche(object sender, EventArgs args)
        {
            if (validateUserChoice())
            {
                updateInterestsUser();
                await Navigation.PushAsync(new DisplayPotentialFriends(user)); 
            }
            else
            {
                await DisplayAlert ("oh oh, on dirait une erreur!", "Veuillez indiquer au moins un loisir et une anecdote", "OK");
            }

        }
        
        

        
        public async void invisibleMenu()
        {
           // await DisplayAlert("mesure", frameMenu.TranslationY + "", "accept");
           // Pk +40, jcomprends pas encore mais ca marche normale
            this.frameMenu.TranslationY +=   (this.frameMenu.HeightRequest +40);
            //await DisplayAlert("mesure", frameMenu.TranslationY + "", "accept");
        }
        
        /*
         * Fonction qui s'occupe de l'apparition du menu
         * sur l'ecran
         */

        // J'utilise pas await pour gerer le cas ou user presse boutton 
        // menu qd le menu est entrain de monter (ou descendre)
        async void OnTapMenu(Object o, EventArgs e)
        {
            
            if (frameMenu.TranslationY.Equals(frameMenu.HeightRequest +40))
            {
                await this.frameMenu.TranslateTo(0, 0, 1000);
                return;
            }

            if (frameMenu.TranslationY.Equals(0 ))
            {
                await frameMenu.TranslateTo(0, frameMenu.TranslationY+frameMenu.HeightRequest+40 , 1000);
                return;
            }
        }

        
        
         
         /*
          * Dans ces 4 prochaines fonctions , je gere lorsque le
          * user clique sur un element du menu
          */
          void OnTapFindFriends(object o, EventArgs e)
         {
             ProfilUser.GestionClickMenu("pote", user);
         }
           void OnTapChooseInterests(object o, EventArgs e)
         {
             ProfilUser.GestionClickMenu("loisirs",user);
         }
           void OnTapViewMessages(object o, EventArgs e)
         {
             ProfilUser.GestionClickMenu("messages",user);
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
         
         
        
        


        

        
    }
}