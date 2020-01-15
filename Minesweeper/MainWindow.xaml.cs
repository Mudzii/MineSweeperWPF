using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


using System.Diagnostics;       //debug
using System.Windows.Threading; //for dispatcherTimer
using static Minesweeper.Game;

namespace Minesweeper {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window {


        private Game gameLogic;
        private DispatcherTimer timer;
        private int gTimer;

        Image sImageDef;
        Image sImagePushed;
        Image sImageSurp;
        Image sImageWin;
        Image sImageLose;


        // window init (Main)
        public MainWindow() {
            InitializeComponent();

            this.Title = "Minesweeper";
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

  
            // initialize 
            initializeTimer();
            gameLogic = new Game();
            this.DataContext = gameLogic;

            InitializeImages(true); 
            gameLogic.InitializeGame();

            AddButtons();
            startButton.Content = sImageDef;
        }

        // timer update
        private void GameTick(object sender, EventArgs e) {

            gTimer++;
            timerTextBox.Text = gTimer.ToString();
        }

        // initialize images used 
        private void InitializeImages(bool init) {

            if (init) {

                sImageDef = new Image();
                sImageDef.Source = new BitmapImage(new Uri("/assets/smileyTile.png", UriKind.Relative));

                sImagePushed = new Image();
                sImagePushed.Source = new BitmapImage(new Uri("/assets/clickedSmiley.png", UriKind.Relative));

                sImageSurp = new Image();
                sImageSurp.Source = new BitmapImage(new Uri("/assets/surpTile.png", UriKind.Relative));

                sImageWin = new Image();
                sImageWin.Source = new BitmapImage(new Uri("/assets/winTile.png", UriKind.Relative));

                sImageLose = new Image();
                sImageLose.Source = new BitmapImage(new Uri("/assets/gameOverTile.png", UriKind.Relative));


            }

            else {

                sImageWin.Source    = null;
                sImageDef.Source    = null;
                sImageLose.Source   = null;
                sImageSurp.Source   = null;
                sImagePushed.Source = null;
            }

        }

        // initialize the timer used for the game
        private void initializeTimer() {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += GameTick;
        }

        // change icon if start button is pressed down
        private void StartButtonDown(object sender, MouseButtonEventArgs e) {

            startButton.Content = sImagePushed;         
        }

        // start button click for new game
        private void StartButton(object sender, MouseButtonEventArgs e) {
 
            startButton.Content = sImageDef; 
            
            // stop timer and reset all windows
            timer.Stop();
            gTimer = 0; 
            timerTextBox.Text = "0";
            mineTextBox.Text = gameLogic.MineAmount.ToString();
            UnsubEvent();

            // create new game and add buttons
            gameLogic.NewGame();
            AddButtons(); 

        }

        // updates the "mine count" AKA mines - flagged mines
        private void UpdateMineCount() {
            int nrMines = gameLogic.FieldMineCount();
            mineTextBox.Text = nrMines.ToString();
        }

        // check if game is won/ lost. Update mine count
        private void GameStatusUpdate() {

            bool gameWon  = gameLogic.GameWon; 
            bool gameLost = gameLogic.GameOver;

            UpdateMineCount(); 
            if (gameWon) {

                timer.Stop(); 
                mineTextBox.Text  = "0";
                startButton.Content = sImageWin;
                MessageBox.Show("Congratulations, You Won! Your time was: " + gTimer + " seconds");
            }

            else if (gameLost) {

                timer.Stop();
                int mineC = gameLogic.MineAmount - gameLogic.DismantledTiles; 
                mineTextBox.Text = mineC.ToString();
                startButton.Content = sImageLose;

                MessageBox.Show("You lost!");
            }

        }

        // check if RMC or MMC
        private void MouseButtonClick(object sender, MouseButtonEventArgs e) {

            if (e.ChangedButton == MouseButton.Right) {
                RightClick(sender, e);
            }

            else if (e.ChangedButton == MouseButton.Middle) {
                
                startButton.Content = sImageSurp;
                MiddleClick(sender, e);
            }


        }

        // if MMC release, change image to def start image
        private void MouseUpMiddle(object sender, MouseButtonEventArgs e) {

             if (e.ChangedButton == MouseButton.Middle) {
                startButton.Content = sImageDef;
            }         
        }

        // MMC - reveal surrounding if flagged
        private void MiddleClick(object sender, RoutedEventArgs e) {

            Button b = sender as Button;
            int x = Int32.Parse((sender as Button).Tag.ToString());

            gameLogic.RevealTilesAroundFlag(x);
            GameStatusUpdate(); 
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

        // event called just as window closes
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e) {

            UnsubEvent();
            timer.Tick -= GameTick;
            timer = null;

            InitializeImages(false);

            startButton.PreviewMouseDown -= StartButtonDown;
            startButton.PreviewMouseLeftButtonUp -= StartButton;

            UnsubEvent(); 
            gameLogic.UninitializeGame();
            gameLogic = null;
            
        }

        // remove events added to tile buttons 
        public void UnsubEvent() {

            List<Tile> gTiles = gameLogic.GameTiles;

            foreach(Tile t in gTiles) {

                t.gButton.Click -= ClickedTile;
                t.gButton.MouseDown -= MouseButtonClick;
                t.gButton.PreviewMouseUp -= MouseUpMiddle;

            }

            gameLogic.GameTiles = gTiles;
            gTiles = null;

        }

        // add event/binding and buttons to field
        public void AddButtons() {

            List<Tile> gTiles = gameLogic.GameTiles;
            ICMineField.Items.Clear();
            
            foreach (Tile t in gTiles) {

                // add click events for buttons 
                t.gButton.Click += ClickedTile;
                t.gButton.MouseDown += MouseButtonClick;
                t.gButton.PreviewMouseUp += MouseUpMiddle;
                
                t.gButton.FontWeight = FontWeights.Bold; 

                // bind tile index to button
                Binding bindingTag = new Binding();
                bindingTag.Source = t.index;
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
