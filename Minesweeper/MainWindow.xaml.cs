﻿using System;
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


namespace Minesweeper {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window {

        //public enum GameState {
        //    DEFAULT,
        //    NEW_GAME,
        //    GAME_WIN,
        //    GAME_OVER
        //};

        //GameState gameState = GameState.DEFAULT;

        private List<GameTile> tiles;
        private Game gameLogic;
        private List<Button> tileButtons; 

        private DispatcherTimer timer;
        private int gTimer;
        private bool minesCreated;
        private bool gameEnded = false; 


        // window init
        public MainWindow() {
            InitializeComponent();

            this.Title = "Minesweeper";
            startButton.Content = "Start";
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // initialize 
            initTimer();
            gameLogic = new Game();
            tileButtons = new List<Button>(); 
            this.DataContext = gameLogic;
            initTiles();

            
        }


        private void MineCount() {

            int totalMineCnt = gameLogic.mines - gameLogic.dismantledTiles;
            mineTextBox.Text = totalMineCnt.ToString(); 
        }


        // timer =======
        private void GameTick(object sender, EventArgs e) {

            gTimer++;
            timerTextBox.Text = gTimer.ToString();

            // change to ONLY check when a click is made
            MineCount(); 
        }

        private void initTimer() {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += GameTick;
            //timer.Start(); 
        }

        // tiles =======
        private void initTiles() {

            int tileIndex = 0;
            List<GameTile> newTiles = new List<GameTile>();

            for (int i = 0; i < gameLogic.rows; i++) {

                for (int j = 0; j < gameLogic.columns; j++) {


                    GameTile newTile = new GameTile {
                        index = tileIndex,
                        revealed = false,
                        r = i,
                        c = j,     
                    };


                    Button tb = new Button();
                    tb.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#bdbdbd"));
                    tb.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#bdbdbd"));
                    tb.FontWeight = FontWeights.Bold;
                    tb.IsHitTestVisible = true;
                    tb.FontSize = 16;
                    tb.Click += ClickedTile;

                    Binding bindingTxt = new Binding();
                    bindingTxt.Source = newTile.Txt;
                    tb.SetBinding(Button.ContentProperty, bindingTxt);

                    Binding bindingTag = new Binding();
                    bindingTag.Source = newTile.Index;
                    tb.SetBinding(Button.TagProperty, bindingTag);

                    tileButtons.Add(tb);

                    

                    newTiles.Add(newTile);
                    tileIndex++;

                }
            }


            AddButtons(); 
            Tiles = newTiles;
           
        }

        private void CreateMines(List<GameTile> t) {

            Random rand = new Random();
            int len = tiles.Count();

            int m = 0;
            while (m < gameLogic.mines) { 
                int nr = rand.Next(0, len);
                if (!t[nr].isMine && t[nr].revealed == false) {
                    t[nr].isMine = true;
                    m++; 
                }

            }
        }


        private void UncoverMines() {

           timer.Stop();
           int len = tiles.Count();
           var children = ICMineField.ItemsSource;

            foreach (var child in children) {

                Button c = child as Button;
                int x = Int32.Parse(c.Tag.ToString());

                GameTile t = tiles[x];

                // disable buttons from being clicked
                c.IsHitTestVisible = false; 

                // if tile is mine, display
                if (t.isMine == true) {

                    t.revealed = true;

                    c.Content = "BOMB";
                    c.Foreground = Brushes.White;
                    c.Background = Brushes.Red;
                    mineTextBox.Text = "0"; 
                }



            }


            //ICMineField.ItemsSource = Tiles; 
        }

        // buttons =======
        private void StartButton(object sender, RoutedEventArgs e) {

            timer.Stop();
            //gameState = GameState.NEW_GAME;

            
            Tiles = null;
            tileButtons = null;
            tileButtons = new List<Button>();
            gameLogic.NewGame();
            timerTextBox.Text = "0";
            minesCreated = false;
            

            initTiles(); 
            gTimer = 0;
            timer.Start();
        }

        private void MarkTile() {

        }

        private void GetSurroundingMines(List<GameTile> t) {

            foreach (GameTile tile in t) {

                if(tile.isMine == false) {

                    int mineTile = 0; 
                    int tileRow = tile.r;
                    int tileCol = tile.c;

                    // get surrounding tiles
                    GameTile left  = tiles.Find(obj => obj.r == tileRow && obj.c == (tileCol - 1));
                    GameTile right = tiles.Find(obj => obj.r == tileRow && obj.c == (tileCol + 1));

                    GameTile top      = tiles.Find(obj => obj.r == (tileRow - 1) && obj.c == tileCol);
                    GameTile topLeft  = tiles.Find(obj => obj.r == (tileRow - 1) && obj.c == (tileCol - 1));
                    GameTile topRight = tiles.Find(obj => obj.r == (tileRow - 1) && obj.c == (tileCol + 1));

                    GameTile bottom      = tiles.Find(obj => obj.r == (tileRow + 1) && obj.c == tileCol);
                    GameTile bottomLeft  = tiles.Find(obj => obj.r == (tileRow + 1) && obj.c == (tileCol - 1));
                    GameTile bottomRight = tiles.Find(obj => obj.r == (tileRow + 1) && obj.c == (tileCol + 1));

                    // check if surrounding tiles exist and are mines
                    if (left  != null && left.isMine   == true) { mineTile++; }
                    if (right != null  && right.isMine == true) { mineTile++; }
                    if (top   != null && top.isMine    == true) { mineTile++; }
                    if (topLeft  != null && topLeft.isMine  == true) { mineTile++; }
                    if (topRight != null && topRight.isMine == true) { mineTile++; }
                    if (bottom   != null && bottom.isMine   == true) { mineTile++; }
                    if (bottomLeft  != null && bottomLeft.isMine  == true) { mineTile++; }
                    if (bottomRight != null && bottomRight.isMine == true) { mineTile++; }


                    tile.surroundingBombs = mineTile;

                    if(mineTile == 1) 
                        tileButtons[tile.index].Foreground = Brushes.Blue;
                    

                    else if (mineTile == 2)
                        tileButtons[tile.index].Foreground = Brushes.Green;

                    else 
                        tileButtons[tile.index].Foreground = Brushes.Red;




                    //if(mineTile != 0) {

                    //    tileButtons[tile.index].Content = tile.surroundingBombs.ToString();
                    //}

                    //else {
                    //    tileButtons[tile.index].Background = Brushes.DarkGray;
                    //}

                }


            }
        }
       
        private void ClickedTile(object sender, RoutedEventArgs e) {

            if (gTimer == 0)
                timer.Start();


            Button b = sender as Button;

            var prnt = b.Parent; 
            int x = Int32.Parse((sender as Button).Tag.ToString());
            
            GameTile t = tiles[x];
            t.revealed = true;
           

            if (!minesCreated) {

                CreateMines(tiles);
                GetSurroundingMines(tiles); 
                minesCreated = true; 
            }

            if(t.isMine == true) {
                b.Content = "BOMB"; 
                b.Foreground = Brushes.White;
                b.Background = Brushes.Red;

                gameEnded = true; 
                UncoverMines(); 
            }



            else {

                if(t.surroundingBombs != 0)
                    b.Content = t.surroundingBombs.ToString();

                else {
                    b.Background = Brushes.DarkGray; 
                }
            }


            // = MessageBox.Show((sender as Button).Tag.ToString()); 


        }

        public void AddButtons() {

            ICMineField.ItemsSource = tileButtons; 

        }

        

        public List<GameTile> Tiles {
            get { return tiles; }

            set {
                tiles = value;
                //ICMineField.ItemsSource = tiles;
            }
        }





    }

}
