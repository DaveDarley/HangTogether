using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Firebase.Database;
using HangTogether.ServerManager;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace HangTogether
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChooseAndModifyInterests : ContentPage
    {

        private TableLoisirsManager gestionLoisirs;

        private FirebaseClient firebase;
        // LE USER EN QUESTION
        private User user;
        public  string FirebaseClient = "https://anodate-ca8b9-default-rtdb.firebaseio.com/";
        public  string FrebaseSecret = "17hN90bHf0ROF4BDSUEsrBTw6AFvuFMe6n3sBFTS";

        public ChooseAndModifyInterests(User activeUser)
        {
            InitializeComponent();
            firebase = new FirebaseClient(FirebaseClient);
            
            gestionLoisirs = new TableLoisirsManager();
            frameAnecdotes.HeightRequest =
                DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density * 0.15;
            
            DeviceDisplay.MainDisplayInfoChanged += OnMainDisplayInfoChanged;
            
            user = activeUser;
            initializeListsOfInterests();
        }
        
        /*
         * Fonction qui recupere les loisirs globales de l'application mais
         * aussi les loisirs propres au user qui se trouve sur la page "choix de loisirs".
         * Et appele une fonction qui dessine les differents loisirs sous forme de frame
         * sur l'ecran du user.
         */
        public async void initializeListsOfInterests()
        {
            this.ContentView.IsVisible = true;
            this.indicator.IsRunning = true;
            List<Loisir> allInterests =  await gestionLoisirs.getAllInterests();
            List<Loisir> interestsUser = await getLoisirsUser();
            List<string> nomChoixUser = new List<string>();
            foreach (var loisirUser in interestsUser)
            {
                nomChoixUser.Add(loisirUser.nom); 
            }
        
            if (! String.IsNullOrEmpty(user.anecdotes))
            {
                this.anecdotesUser.Text = user.anecdotes;
            }
        
            List<Frame> frameAllInterests = CreateLoisirsFrame(allInterests,nomChoixUser);
            
            this.ContentView.IsVisible = false;
            this.indicator.IsRunning = false;
            
            addLoisirsToLayout(frameAllInterests);
                
            }
        
        
        /*
         * Cette fonction prend en parametre la liste globale de loisirs et une liste des loisirs
         * du user(sous forme de string) et dessine chaque loisir de la liste globale de loisirs
         * sous forme de Frame sur l'ecran du user.
         * Si un loisir appartient a la liste globale mais aussi a la liste de loisirs du user(i.e user
         * a selectionne ce loisir), on le dessine en Frame avec bg noir et texte blanc(ce qui dessine
         * le choix du user).
         */
        public List<Frame>  CreateLoisirsFrame(List<Loisir>InterestsGlobal,List<string>InterestsUser)
        {
            Color frameColor;
            Color textColor;
            List<Frame> frameADessinerSurlayout = new List<Frame>();
            for (int i = 0; i < InterestsGlobal.Count; i++)
            {
                if (InterestsUser.Contains(InterestsGlobal[i].nom))
                {
                    frameColor = Color.Black;
                    textColor = Color.White;
                }
                else
                { 
                    frameColor = Color.White;
                    textColor = Color.Black;
                }

                Frame frame = new Frame()
                {
                    
                    BackgroundColor = frameColor,
                    CornerRadius = 30,
                    HasShadow = true,
                    IsVisible = true,
                    Margin = new Thickness(0,4,0,4),
                    Content = new Label()
                    {
                        Text = InterestsGlobal[i].nom,
                        TextColor = textColor,
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
         * Fonction qui recupere les loisirs d'un user pour ensuite les dessiner
         * bg black text blanc sur mon flexlayout
         */
        public async Task<List<Loisir>> getLoisirsUser()
        {
            DataBaseManager dataBaseManager = new DataBaseManager();
            return  await dataBaseManager.getInterestsUser(user);
        }


        /*
         * Fonction qui permet de savoir les choix du User
         * Qd user selectionne Frame(bg devient noir et textcolor Blanc)
         * Si bg deja noir alors user veux deselectionner son choix.
         * On sauvegarde chaque choix de l'user dans ma BD
         */
        public async void OnTapFramFrame(Object s, EventArgs e)
        {
            var monFrame = (Frame) s;
            // Verifions si le frame a deja ete selectionné ou pas 
            if (monFrame.BackgroundColor == Color.Black)
            {
                monFrame.BackgroundColor = Color.White;
                var monLabel = (Label) monFrame.Content;
                monLabel.TextColor= Color.Black;
                
                
                GestionChoixLoisirsUser gestionChoixLoisirsUser = new GestionChoixLoisirsUser();
                ChoixLoisirsUser choixLoisirsUser = await gestionChoixLoisirsUser.getChoixUser(user, monLabel.Text);
                gestionChoixLoisirsUser.deleteChoixUser(choixLoisirsUser);
            }
            else
            {   // Utilisateur fait un choix de loisir
                monFrame.BackgroundColor = Color.Black;
                var monLabel = (Label) monFrame.Content;
                monLabel.TextColor= Color.White;

                TableLoisirsManager tableLoisirsManager = new TableLoisirsManager();
                tableLoisirsManager.addInterests(user,monLabel.Text);
            }
        }
        

        /*
         * Fonction qui s'occupe de mettre a jour les anecdotes du user dans la BD
         */
        public async void updateInterestsUser()
        {
            User toUpdate = user;
            DataBaseManager dataBaseManager = new DataBaseManager();
            toUpdate.anecdotes = String.IsNullOrEmpty(anecdotesUser.Text) ? "" : anecdotesUser.Text;
            
            this.user = toUpdate;
            await dataBaseManager.UpdateUser(toUpdate);
        }

        
        /*
         * Fonction qui s'occupe de dessiner les frames recues en parametre a l'ecran
         */
        public void addLoisirsToLayout(List<Frame> interestsToDrawOnScreen)
        {
            var layoutUser = this.layoutInterest;
            foreach (var interest in interestsToDrawOnScreen)
            {
                layoutUser.Children.Add(interest);
            }
            
        }
        
        /*
         * Fonction qui prend le texte entré  par le user dans
         * le searchBar et affiche dans le flexLayout les
         * Frames qui correspond a la recherche (s'il y en a)
         *
         * Lorsque User tape texte dans le searchbar, on parcours notre liste de frame
         * si le texte du frame contient le query du user (ou query contient texte du frame) on met le frame.IsVisible
         * a true sinon on le met a false.
         */
        void OnUserSearchChanged(object sender, EventArgs e)
        {
            SearchBar searchBar = (SearchBar)sender;
            string queryUser = searchBar.Text;

            // Si user cherche rien on affiche TOUS les loisirs sur l'ecran
            if (! String.IsNullOrEmpty(queryUser))
            {
                var framesOnLayout = layoutInterest.Children.Cast<Frame>().ToList();
                //Cherchons le frame qui contient le query recherché par le user
                for (int i = 0; i<framesOnLayout.Count; i++)
                {
                    var labelFrame = (Label) framesOnLayout[i].Content;
                    var textFrame = (labelFrame.Text).ToLower();
                    queryUser = queryUser.ToLower();
                
                    if (queryUser.Length > textFrame.Length)
                    {
                        if (queryUser.Contains(textFrame))
                        {
                            framesOnLayout[i].IsVisible = true;
                            continue;
                        }
                    }
                    else
                    {
                        if (textFrame.Contains(queryUser))
                        {
                            framesOnLayout[i].IsVisible = true;
                            continue;
                        }
                    }

                    framesOnLayout[i].IsVisible = false;
                }
            }
            else
            {
                var framesOnLayout = layoutInterest.Children.Cast<Frame>().ToList();
                foreach (var frame in framesOnLayout)
                {
                    frame.IsVisible = true;
                }
            }
        }
        
        /*
         * Qd un user ajoute un nouveau loisir:
         * On verifie si loisir n'existe pas deja dans ma table loisir
         * Si oui on cree une reference de mon user vers ce loisir
         * Si non, on ajoute loisir dans ma table Loisirs et on cree ensuite
         * la reference entre user et loisir
         * En dernier on le dessine sur le flexlayout
         */
        async void OnAddInterests(Object s, EventArgs e)
        {
            string loisirsAjoute = (await DisplayPromptAsync("Ajout de Loisirs", "Veuillez ajouter votre loisir", keyboard: Keyboard.Text));
            if (!(String.IsNullOrEmpty(loisirsAjoute)))
            {
                loisirsAjoute = loisirsAjoute.ToUpper();
                Loisir loisir = await gestionLoisirs.getLoisir(loisirsAjoute);
                if (loisir is null)
                {
                    gestionLoisirs.addInterests(user, loisirsAjoute);

                    Frame frame = new Frame()
                    {
                        BackgroundColor = Color.Black,
                        CornerRadius = 30,
                        HasShadow = true,
                        IsVisible = true,
                        Margin = new Thickness(0, 4, 0, 4),
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
                    tapGestureRecognizer.Tapped += (obj, ev) =>
                    {
                        // handle the tap
                        OnTapFramFrame(obj, ev);
                    };
                    frame.GestureRecognizers.Add(tapGestureRecognizer);

                    // Dessine le nouveau frame dans mon stack
                    layoutInterest.Children.Add(frame);
                }
                else
                {
                    GestionChoixLoisirsUser gestionChoixLoisirsUser = new GestionChoixLoisirsUser();
                    ChoixLoisirsUser choixLoisirsUser = await gestionChoixLoisirsUser.getChoixUser(user, loisirsAjoute);
                    
                    // Loisir existe dans la liste globale mais pas dans le choix du user
                    if (choixLoisirsUser is null)
                    {
                        gestionLoisirs.addInterests(user, loisirsAjoute);
                    }
                    int nbFramesOnScreen = layoutInterest.Children.Count();
                    for (int i = 0; i < nbFramesOnScreen; i++)
                    {
                        var monFrame = (Frame) layoutInterest.Children[i];
                        var labelFrame = (Label) monFrame.Content;
                        if (labelFrame.Text == loisirsAjoute)
                        {
                            monFrame.BackgroundColor = Color.Black;
                            var monLabel = (Label) monFrame.Content;
                            monLabel.TextColor = Color.White;

                        }
                    }
                } 
            }

    }
        
        /*
         * Qd user clique sur le boutton de recherche de nouveauPote;
         * Il faut s'assurer que le User a fait au moins un choix de loisirs
         * mais a aussi inscrit au moins une acdotes;
         * On savegarde ensuite les loisirs choisies par le user mais aussi les
         * anecdotes du User dans les variables qui leurs correspondent
        */
        public async Task<bool> validateUserChoice()
        {
            var canUserGoToNextPage = true;
            var lesAnecdotesDuUser = this.anecdotesUser.Text;
            DataBaseManager dataBaseManager = new DataBaseManager();
            var listInterestUser = await dataBaseManager.getInterestsUser(user);
            if (listInterestUser.Count() == 0)
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
            if (await validateUserChoice())
            {
                this.ContentView.IsVisible = false;
                this.indicator.IsRunning = false;
                updateInterestsUser();
               await Navigation.PushAsync(new DisplayPotentialFriends(user)); 
            }
            else
            {
                await DisplayAlert ("oh oh, on dirait une erreur!", "Veuillez indiquer au moins un loisir et une anecdote", "OK");
            }

        }


        /*
         * Fonction qui s'occupe de baisser le menu lorsque le user clique n'importe ou
         * sur l'ecran:
         * Si GridLoisirs.IsEnabled = false c-a-d mon menu est sur l'ecran alors je descends
         * le menu est j'active les interactions avec les users
         */
        async void OnTapBg(Object o, EventArgs e)
        {
            if (!this.gridLoisirs.IsEnabled)
            {
                Action<double> callback = input => frameMenu.HeightRequest = input;
                double startHeight = /*mainDisplayInfo.Height*/ Application.Current.MainPage.Height/3;
                double endiendHeight = 0;
                uint rate = 32;
                uint length = 500;
                Easing easing = Easing.SinOut;
                frameMenu.Animate("anim", callback, startHeight, endiendHeight, rate, length, easing);
                this.gridLoisirs.IsEnabled = true;
            }
        }


        /*
         * Fonction qui s'occupe de l'apparition et de la disparition du menu sur l'ecran
         * SRC: http://xamaringuyshow.com/2020/06/21/xamarin-forms-bottom-slider/
         * SRC: https://stackoverflow.com/questions/32494482/disable-user-interaction-to-current-page
         * SRC: https://social.msdn.microsoft.com/Forums/en-US/4db8de72-a7c0-4431-b09b-4f8da19de0dd/how-to-check-if-anything-happened-in-the-app-like-screen-was-touched?forum=xamarinios
         */
        async void OnTapMenu(Object o, EventArgs e)
        {
           if (frameMenu.HeightRequest == 0)
           {
               Action<double> callback = input => frameMenu.HeightRequest = input;
               double startHeight = 0;
               double endHeight = Application.Current.MainPage.Height/3;
               uint rate = 32;
               uint length = 500;
               Easing easing = Easing.CubicOut;
               frameMenu.Animate("anim", callback, startHeight, endHeight, rate, length, easing);
               this.gridLoisirs.IsEnabled = false;// empecher a ce que user clique en bg lorsque le menu apparait 
               return;
           }
           else
           {
               Action<double> callback = input => frameMenu.HeightRequest = input;
               double startHeight = Application.Current.MainPage.Height/3;
               double endiendHeight = 0;
               uint rate = 32;
               uint length = 500;
               Easing easing = Easing.SinOut;
               frameMenu.Animate("anim", callback, startHeight, endiendHeight, rate, length, easing);
               this.gridLoisirs.IsEnabled = true; // je rends les interactions (event) du grid possible
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
                 ProfilUser.GestionClickMenu("delete",user);
             }
         }
         
         
         /*
          * Fonction qui s'occupe de la dimension de la case anecdotes dependemments de l'orientation
          * de l'ecran
          * Alternative : mettre * dans grid et non Auto
          * Src: https://stackoverflow.com/questions/61937146/find-device-orientation-not-app-orientation-using-xamarin-forms
          */
         public void OnMainDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
         {
             frameMenu.HeightRequest = 0; // si le menu etait ouvert qd on rotate le tlf, il doit devenir fermer
             gridLoisirs.IsEnabled = true; // si menu ouvert alors arriere unclickable, si on rotate l'ecran alors menu descends mais 
             // bg reste tjrs non clickable, alors on le re-rend clickable
             if (e.DisplayInfo.Orientation.ToString() == "Portrait")
             {
                 frameAnecdotes.HeightRequest =
                     DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density * 0.25;
                 anecdotesUser.HeightRequest =
                     DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density * 0.22;
                 return;
             }
             if (e.DisplayInfo.Orientation.ToString() == "Landscape")
             {
                 frameAnecdotes.HeightRequest =
                     DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density * 0.2;
                 anecdotesUser.HeightRequest =
                     DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density * 0.18;
             }
         }
         
        
        


        

        
    }
}