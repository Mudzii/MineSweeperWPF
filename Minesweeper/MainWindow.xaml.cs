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

        // window init
        public MainWindow() {
            InitializeComponent();

            this.Title = "Minesweeper";
            startButton.Content = "Start";
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // initialize 
            initializeTimer();
            gameLogic = new Game();
            this.DataContext = gameLogic;

            gameLogic.InitializeGame();
            AddButtons();
        }

        private void GameTick(object sender, EventArgs e) {

            gTimer++;
            timerTextBox.Text = gTimer.ToString();
            GameStatusUpdate(); 
        }

        private void initializeTimer() {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += GameTick;
        }

        private void StartButton(object sender, RoutedEventArgs e) {

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
                MessageBox.Show("Congratulations, You Won! Your time was " + gTimer + "seconds");
            }

            else if (gameLost) {
                timer.Stop();
                mineTextBox.Text = gameLogic.MineAmount.ToString();

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
