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
            gameLogic.minesLeft = gameLogic.mines; 
            tileButtons = new List<Button>(); 
            this.DataContext = gameLogic;
            initTiles();

            
        }


        private void MineCount() {

            int totalMineCnt = gameLogic.minesLeft - gameLogic.dismantledTiles;
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
                    tb.Height = 50;
                    tb.Width = tb.Height; 
                    tb.IsHitTestVisible = true;
                    tb.FontSize = 16;
                    tb.Click += ClickedTile;
                    //tb.MouseRightButtonDown += RightClick;
                    tb.MouseDown += MouseButtonClick; 

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

           //timer.Stop();
           int len = tiles.Count();
           var children = ICMineField.ItemsSource;

            gameLogic.minesLeft = 0;
            mineTextBox.Text = "0";

            foreach (var child in children) {

                Button c = child as Button;
                int x = Int32.Parse(c.Tag.ToString());

                GameTile t = tiles[x];

                // disable buttons from being clicked
                c.IsHitTestVisible = false; 

                // if tile is mine, display
                if (t.isMine == true) {

                    t.revealed = true;

                    c.Content = "BOM";
                    c.Foreground = Brushes.White;
                    c.Background = Brushes.Red;
                    mineTextBox.Text = "0"; 
                }



            }

            gameEnded = true;
            GameEnd(); 
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
            gameEnded = false; 

            initTiles(); 
            gTimer = 0;
            
        }
            
        private void GameStatus() {

            // if all tiles but the ones with mines have been revealed, game is won
            if (!tiles.Any(x => x.isMine == false && x.revealed == false)) {
                    
                GameEnd(); 
            }

        }

        private void GameEnd() {

            timer.Stop();

            foreach(GameTile t in tiles) {

                tileButtons[t.index].IsHitTestVisible = false;

                if (t.isMine && !gameEnded) {
                    tileButtons[t.index].Content = "F";
                    tileButtons[t.index].Foreground = Brushes.Gold;
                }
               
            }

            if (!gameEnded) {

                mineTextBox.Text = "0";
                MessageBox.Show("You Won!"); 
            }

            else {
                MessageBox.Show("You lost!");

            }

        }

        private List<GameTile> SurroundingTiles(int tileRow, int tileCol) {

            List<GameTile> neighbouringTiles = new List<GameTile>();

            GameTile left = tiles.Find(obj => obj.r == tileRow && obj.c == (tileCol - 1));
            GameTile right = tiles.Find(obj => obj.r == tileRow && obj.c == (tileCol + 1));

            GameTile top = tiles.Find(obj => obj.r == (tileRow - 1) && obj.c == tileCol);
            GameTile topLeft = tiles.Find(obj => obj.r == (tileRow - 1) && obj.c == (tileCol - 1));
            GameTile topRight = tiles.Find(obj => obj.r == (tileRow - 1) && obj.c == (tileCol + 1));

            GameTile bottom = tiles.Find(obj => obj.r == (tileRow + 1) && obj.c == tileCol);
            GameTile bottomLeft = tiles.Find(obj => obj.r == (tileRow + 1) && obj.c == (tileCol - 1));
            GameTile bottomRight = tiles.Find(obj => obj.r == (tileRow + 1) && obj.c == (tileCol + 1));


            if (left != null) { neighbouringTiles.Add(left); }
            if (right != null) { neighbouringTiles.Add(right); }

            if (top != null) { neighbouringTiles.Add(top); }
            if (topLeft != null) { neighbouringTiles.Add(topLeft); }
            if (topRight != null) { neighbouringTiles.Add(topRight); }

            if (bottom != null) { neighbouringTiles.Add(bottom); }
            if (bottomLeft != null) { neighbouringTiles.Add(bottomLeft); }
            if (bottomRight != null) { neighbouringTiles.Add(bottomRight); }

            return neighbouringTiles; 
        }

        private void GetSurroundingMines(List<GameTile> t) {

            foreach (GameTile tile in t) {

                if(tile.isMine == false) {

                    int mineTile = 0; 
                    int tileRow = tile.r;
                    int tileCol = tile.c;

                    var nTiles = SurroundingTiles(tileRow, tileCol); 

                    foreach (GameTile gt in nTiles) {

                        if (gt.isMine == true) { mineTile++; }
                    }


                    tile.surroundingBombs = mineTile;

                    if (mineTile == 1)
                        tileButtons[tile.index].Foreground = Brushes.Blue;


                    else if (mineTile == 2)
                        tileButtons[tile.index].Foreground = Brushes.Green;

                    else
                        tileButtons[tile.index].Foreground = Brushes.Red;
                
                }


            }
        }
       
        private void RevealNeightbours(GameTile t) {

            int tileRow = t.r;
            int tileCol = t.c; 
            var nTiles = SurroundingTiles(tileRow, tileCol);

          
            foreach (GameTile gt in nTiles) {

                if(gt.revealed == false && gt.isDismantled == false) {
                    gt.revealed = true;

                    if (gt.isMine == true && gt.isDismantled == false){
                        
                        tileButtons[gt.index].Background = Brushes.Red;
                        tileButtons[gt.index].Foreground = Brushes.White;
                        tileButtons[gt.index].Content = "BOM";
                        UncoverMines();
                        return; 

                    }

                    if (!gt.isMine) {



                        if (gt.surroundingBombs == 0) {

                            tileButtons[gt.index].Background = Brushes.DarkGray;
                            RevealNeightbours(gt);
                            tileButtons[gt.index].IsHitTestVisible = false;
                            tileButtons[gt.index].Content = ""; 
                        }

                        else {
                            tileButtons[gt.index].Background = Brushes.DarkGray;
                            tileButtons[gt.index].Content = gt.surroundingBombs.ToString();
                        }
                    }


                }

            }

            GameStatus(); 
        }

        private void MouseButtonClick(object sender, MouseButtonEventArgs e) {

           

            if (e.ChangedButton == MouseButton.Right) {
                RightClick(sender, e); 
            }

            else if(e.ChangedButton == MouseButton.Middle) {
                MiddleClick(sender, e);
            }
            

        }

        private void MiddleClick(object sender, RoutedEventArgs e) {

            //if you middle-click a number, and it is surrounded by exactly that many flags 
            //(as indicated by the number), all covered tiles become uncovered

            Button b = sender as Button;
            int x = Int32.Parse((sender as Button).Tag.ToString());

            GameTile t = tiles[x];

            if(t.surroundingBombs > 0 && t.revealed == true) {

                int nDisarmed = 0; 
                var nTiles = SurroundingTiles(t.r, t.c);

                // get how many flags are arround the tile
                nDisarmed = nTiles.Count(x => x.isDismantled == true); 

                if(nDisarmed == t.surroundingBombs) {

                    RevealNeightbours(t); 
                   
                }


            }

        }

        private void RightClick(object sender, RoutedEventArgs e) {

            Button b = sender as Button;
            int x = Int32.Parse((sender as Button).Tag.ToString());

            int tileIndex = tiles[x].index; 

            if (b.Content == "") {
                b.Content = "F";
                b.Foreground = Brushes.Gold; 
                tiles[tileIndex].isDismantled = true;
                gameLogic.dismantledTiles++; 
            }

            else if (b.Content == "F") {
                b.Content = "?";
                b.Foreground = Brushes.DarkMagenta;
                tiles[tileIndex].isDismantled = false;
                gameLogic.dismantledTiles--;
            }

            else if (b.Content == "?") {
                b.Content = "";
                tiles[tileIndex].isDismantled = false;

                if (tiles[tileIndex].surroundingBombs == 1)
                    tileButtons[tileIndex].Foreground = Brushes.Blue;


                else if (tiles[tileIndex].surroundingBombs == 2)
                    tileButtons[tileIndex].Foreground = Brushes.Green;

                else
                    tileButtons[tileIndex].Foreground = Brushes.Red;

            }

            MineCount();
            GameStatus();
        }

        private void ClickedTile(object sender, RoutedEventArgs e) {

            // start timer as soon as left button is clicked
            if (gTimer == 0)
                timer.Start();

            Button b = sender as Button;
            int x = Int32.Parse((sender as Button).Tag.ToString());
            
            GameTile t = tiles[x];

            if(tiles[x].isDismantled == false) {
              
                t.revealed = true;

                if (!minesCreated) {

                    CreateMines(tiles);
                    GetSurroundingMines(tiles); 
                    minesCreated = true; 
                }

                if (b.Content == "?") {
                    b.Content = "";     
                }

                if (t.isMine == true) {
                    b.Content = "BOM"; 
                    b.Foreground = Brushes.White;
                    b.Background = Brushes.Red;

                    gameEnded = true; 
                    UncoverMines(); 
                }

                else {

                    if(t.surroundingBombs != 0) {

                        b.Content = t.surroundingBombs.ToString();
                        b.Background = Brushes.DarkGray;
                    }

                    else {
                        b.Background = Brushes.DarkGray;
                        RevealNeightbours(t); 
                    }

                    MineCount();
                }

            }

            GameStatus(); 

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
