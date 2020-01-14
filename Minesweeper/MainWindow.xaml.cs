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


        // window init
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

        private void GameTick(object sender, EventArgs e) {

            gTimer++;
            timerTextBox.Text = gTimer.ToString();
            if(gTimer > 0)
                GameStatusUpdate(); 
        }

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
                sImageLose.Source = new BitmapImage(new Uri("/assets/qmTile.png", UriKind.Relative));


            }

            else {

                sImageWin.Source    = null;
                sImageDef.Source    = null;
                sImageLose.Source   = null;
                sImageSurp.Source   = null;
                sImagePushed.Source = null;
            }

            int i = 0; 
        }


        private void initializeTimer() {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += GameTick;
        }

        private void startButtonDown(object sender, MouseButtonEventArgs e) {

            startButton.Content = sImagePushed;

        }

        private void StartButton(object sender, RoutedEventArgs e) {


            
            //startButton.Content = sImageDef; 
            
            timer.Stop();
            gTimer = 0; 
            timerTextBox.Text = "0";

            mineTextBox.Text = gameLogic.MineAmount.ToString(); 

            gameLogic.NewGame();
            AddButtons(); 

        }

        private void UpdateMineCount() {
            int nrMines = gameLogic.FieldMineCount();
            mineTextBox.Text = nrMines.ToString();
        }

        private void GameStatusUpdate() {

            bool gameWon  = gameLogic.GameWon; 
            bool gameLost = gameLogic.GameOver;

            UpdateMineCount(); 
            if (gameWon) {

                timer.Stop(); 
                mineTextBox.Text  = "0";
                MessageBox.Show("Congratulations, You Won! Your time was: " + gTimer + " seconds");
            }

            else if (gameLost) {
                timer.Stop();
                int mineC = gameLogic.MineAmount - gameLogic.DismantledTiles; 
                mineTextBox.Text = mineC.ToString();

                MessageBox.Show("You lost!");
            }

        }

        private void MouseButtonClick(object sender, MouseButtonEventArgs e) {

            if (e.ChangedButton == MouseButton.Right) {
                RightClick(sender, e);
            }

            else if (e.ChangedButton == MouseButton.Middle) {
                MiddleClick(sender, e);
            }


        }

        private void MiddleClick(object sender, RoutedEventArgs e) {

            //if you middle-click a number, and it is surrounded by exactly that many flags 
            //(as indicated by the number), all covered tiles become uncovered

            Button b = sender as Button;
            int x = Int32.Parse((sender as Button).Tag.ToString());

            gameLogic.RevealTilesAroundFlag(x);
            UpdateMineCount(); 
        }

        private void RightClick(object sender, RoutedEventArgs e) {

            Button b = sender as Button;
            int x = Int32.Parse((sender as Button).Tag.ToString());

            gameLogic.MarkTile(x);
            UpdateMineCount(); 
        }

        private void ClickedTile(object sender, RoutedEventArgs e) {

            if (gTimer == 0)
                timer.Start(); 

            Button b = sender as Button;
            int x = Int32.Parse((sender as Button).Tag.ToString());

            gameLogic.RevealTile(x);
            UpdateMineCount(); 
        }

        // event called just as window closes
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e) {

            UnsubEvent();
            timer.Tick -= GameTick;
            timer = null;

            InitializeImages(false); 

            gameLogic.UninitializeGame();
            gameLogic = null;
            
        }

        public void UnsubEvent() {

            List<Tile> gTiles = gameLogic.GameTiles;

            foreach(Tile t in gTiles) {

                t.gButton.Click -= ClickedTile;
                t.gButton.MouseDown -= MouseButtonClick; 
            }

            gameLogic.GameTiles = gTiles;
            gTiles = null;

        }

        public void AddButtons() {

            UnsubEvent(); 
            List<Tile> gTiles = gameLogic.GameTiles;
            ICMineField.Items.Clear();
            
            foreach (Tile t in gTiles) {

                t.gButton.Click += ClickedTile;
                t.gButton.MouseDown += MouseButtonClick;
                t.gButton.FontWeight = FontWeights.Bold; 

                Binding bindingTxt = new Binding();
                bindingTxt.Source = t.gTile.Txt;
                t.gButton.SetBinding(Button.ContentProperty, bindingTxt);

                Binding bindingTag = new Binding();
                bindingTag.Source = t.index;
                t.gButton.SetBinding(Button.TagProperty, bindingTag);
            }

            gameLogic.GameTiles = gTiles;
           
            foreach (Tile t in gTiles) {

                ICMineField.Items.Add(t.gButton);
            }

            gTiles = null;
            
            //ICMineField.ItemsSource = tileButtons; 

        }

        
    }

}
