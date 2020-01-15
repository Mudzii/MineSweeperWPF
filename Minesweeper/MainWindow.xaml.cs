using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Windows.Media.Imaging;


using System.Diagnostics;       //debug
using System.Windows.Threading; //for dispatcherTimer
using static Minesweeper.Game;

namespace Minesweeper {

    public partial class MainWindow : Window {

        // Variables ===============
        private Game gameLogic;
        private List<Image> sImages;

        private DispatcherTimer timer;
        private int gTimer;


        // window init (Main) ======
        public MainWindow() {
            InitializeComponent();

            this.Title = "Minesweeper";
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // initialize 
            initializeTimer();
            gameLogic        = new Game();
            this.DataContext = gameLogic;

            InitializeImages(true); 
            gameLogic.InitializeGame();

            AddButtons();
            startButton.Content = sImages[0];
        }

        // Timer functions =========

        // timer update
        private void GameTick(object sender, EventArgs e) {

            gTimer++;
            timerTextBox.Text = gTimer.ToString();
        }

        // initialize the timer used for the game
        private void initializeTimer() {
            timer          = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick    += GameTick;
        }

        // =========================

        // initialize images used 
        private void InitializeImages(bool init) {

            if (init) {

                sImages = new List<Image>(); 

                Image sImageDef  = new Image();
                sImageDef.Source = new BitmapImage(new Uri("/assets/smileyTile.png", UriKind.Relative));
                sImages.Add(sImageDef);

                Image sImagePushed  = new Image();
                sImagePushed.Source = new BitmapImage(new Uri("/assets/clickedSmiley.png", UriKind.Relative));
                sImages.Add(sImagePushed);

                Image sImageSurp  = new Image();
                sImageSurp.Source = new BitmapImage(new Uri("/assets/surpTile.png", UriKind.Relative));
                sImages.Add(sImageSurp);

                Image sImageWin  = new Image();
                sImageWin.Source = new BitmapImage(new Uri("/assets/winTile.png", UriKind.Relative));
                sImages.Add(sImageWin);

                Image sImageLose  = new Image();
                sImageLose.Source = new BitmapImage(new Uri("/assets/gameOverTile.png", UriKind.Relative));
                sImages.Add(sImageLose);


            }

            else {

                for(int i = 0; i < 5; i++) {
                    sImages[i].Source = null; 
                }

            }

        }

        // updates the "mine count" AKA mines - flagged mines
        private void UpdateMineCount() {
            int nrMines = gameLogic.FieldMineCount();
            mineTextBox.Text = nrMines.ToString();
        }


        // Button functions ========

        // change icon if start button is pressed down
        private void StartButtonDown(object sender, MouseButtonEventArgs e) {

            startButton.Content = sImages[1];         
        }

        // start button click for new game
        private void StartButton(object sender, MouseButtonEventArgs e) {
 
            startButton.Content = sImages[0]; 
            
            // stop timer and reset all windows
            timer.Stop();
            gTimer = 0; 
            timerTextBox.Text = "0";
            mineTextBox.Text  = gameLogic.MineAmount.ToString();
            UnsubEvent();

            // create new game and add buttons
            gameLogic.NewGame();
            AddButtons(); 

        }

        // check if RMC or MMC
        private void MouseButtonClick(object sender, MouseButtonEventArgs e) {

            if (e.ChangedButton == MouseButton.Right) {
                RightClick(sender, e);
            }

            else if (e.ChangedButton == MouseButton.Middle) {
                
                startButton.Content = sImages[2];
                MiddleClick(sender, e);
            }


        }

        // if MMC release, change image to def start image
        private void MouseUpMiddle(object sender, MouseButtonEventArgs e) {

             if (e.ChangedButton == MouseButton.Middle) {
                startButton.Content = sImages[0];
            }         
        }

        // MMC - reveal surrounding if flagged
        private void MiddleClick(object sender, RoutedEventArgs e) {

            Button b = sender as Button;
            int x = Int32.Parse((sender as Button).Tag.ToString());

            gameLogic.RevealTilesAroundFlag(x);
            GameStatusUpdate(); 
        }

    // check if game is won/ lost. Update mine count
        private void GameStatusUpdate() {

            bool gameWon  = gameLogic.GameWon; 
            bool gameLost = gameLogic.GameOver;

            UpdateMineCount(); 
            if (gameWon) {

                timer.Stop(); 
                mineTextBox.Text    = "0";
                startButton.Content = sImages[3];
                MessageBox.Show("Congratulations, You Won! Your time was: " + gTimer + " seconds");
            }

            else if (gameLost) {

                timer.Stop();
                int mineC = gameLogic.MineAmount - gameLogic.DismantledTiles; 
                mineTextBox.Text    = mineC.ToString();
                startButton.Content = sImages[4];

                MessageBox.Show("You lost!");
            }

        }

        // RMC - mark a tile
        private void RightClick(object sender, RoutedEventArgs e) {

            Button b = sender as Button;
            int x = Int32.Parse((sender as Button).Tag.ToString());

            gameLogic.MarkTile(x);
            GameStatusUpdate();
        }

        // LMC - reveal tiles
        private void ClickedTile(object sender, RoutedEventArgs e) {

            // if timer hasn't started, start it
            if (gTimer == 0)
                timer.Start(); 

            Button b = sender as Button;
            int x = Int32.Parse((sender as Button).Tag.ToString());

            gameLogic.RevealTile(x);    
            GameStatusUpdate();

        }

        // =========================

        // event called just as window closes
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e) {

            UnsubEvent();
            timer.Tick -= GameTick;
            timer       = null;

            InitializeImages(false);

            startButton.PreviewMouseDown         -= StartButtonDown;
            startButton.PreviewMouseLeftButtonUp -= StartButton;

            UnsubEvent(); 
            gameLogic.UninitializeGame();
            gameLogic = null;
            
        }

        // remove events added to tile buttons 
        private void UnsubEvent() {

            List<Tile> gTiles = gameLogic.GameTiles;

            foreach(Tile t in gTiles) {

                t.gButton.Click     -= ClickedTile;
                t.gButton.MouseDown -= MouseButtonClick;
                t.gButton.PreviewMouseUp -= MouseUpMiddle;

            }

            gameLogic.GameTiles = gTiles;
            gTiles = null;

        }

        // add event/binding and buttons to field
        private void AddButtons() {

            List<Tile> gTiles = gameLogic.GameTiles;
            ICMineField.Items.Clear();
            
            foreach (Tile t in gTiles) {

                // add click events for buttons 
                t.gButton.Click     += ClickedTile;
                t.gButton.MouseDown += MouseButtonClick;
                t.gButton.PreviewMouseUp += MouseUpMiddle;
                
                t.gButton.FontWeight = FontWeights.Bold; 

                // bind tile index to button
                Binding bindingTag = new Binding();
                bindingTag.Source  = t.index;
                t.gButton.SetBinding(Button.TagProperty, bindingTag);
            }

            // set new list to gameTiles
            gameLogic.GameTiles = gTiles;
           
            // add buttons from list to mine field
            foreach (Tile t in gTiles) {
                ICMineField.Items.Add(t.gButton);
            }

            gTiles = null;

        }

       
    }

}
