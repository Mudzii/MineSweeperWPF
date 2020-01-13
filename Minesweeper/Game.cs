using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows.Controls;  // button
using System.Windows.Threading; // dispatcherTimer

namespace Minesweeper {

    public class Game {


        // Variables ===============
        public int mines = 10;
        //public int minesLeft;
        //public int dismantledTiles = 0;
        public bool gameEnded = false;


        // new Variables =========== 

        SolidColorBrush cGrey = (SolidColorBrush)(new BrushConverter().ConvertFrom("#bdbdbd"));
        SolidColorBrush cFlag = Brushes.DarkRed; 


        public struct Tile {
            public int index;
            public GameTile gTile;
            public Button gButton;
        }

        public int rows    = 9;
        public int columns = 9;

        
        public List<Tile> gameTiles;

        // mine varialbes
        private int mineCount = 10;
        public int minesLeft = 0; 
        public int dismantledTiles = 0; 
        private bool minesCreated = false;


        // timer variables 
        private DispatcherTimer dTimer;
        private int gTime;

        // game mode variables
        private bool gameOver = false;


        // Get/Set =================

        public List<Tile> GameTiles {
            get { return gameTiles; }
            set { gameTiles = value; }
        }

        public int NrRows {
            get { return rows; }
            set { rows = value; }
        }

        public int NrColumns {
            get { return columns; }
            set { columns = value;

            }
        }


        public bool MinesCreated {
            get { return minesCreated; }
            set { minesCreated = value; }
        }

        public int MineAmount {
            get { return mines; }
            set { mines = value; }
        }

        public int DismantledTiles {
            get { return dismantledTiles; }
            set { dismantledTiles = value; }
        } 

        public int MinesLeft {
            get { return minesLeft; }
            set { minesLeft = value; }
        }



        public bool GameOver {
            get { return gameOver; }
            set { gameOver = value; }
        }

        public int GameTime {
            get { return gTime; }
            set { gTime = value; }

        }




        // OLD REMOVE LATER
        public bool GameEnded {
            get { return gameEnded; }
            set { gameEnded = value; }
        }

        // Init functions ===============

        private void TimeTick(object sender, EventArgs e) {

            gTime++;             
        }

        private void initializeTimer() {

            dTimer = new DispatcherTimer();
            dTimer.Interval = TimeSpan.FromSeconds(1);
            dTimer.Tick += TimeTick; 
        }

        public void InitializeGame() {

            initializeTimer();
            minesLeft = mineCount;

            gameTiles = new List<Tile>(); 
        }


        // ==============================

        private List<Tile> SurroundingTiles(int row, int col) {

            List<Tile> neighbouringTiles = new List<Tile>();

            Tile left  = gameTiles.Find(obj => obj.gTile.r == row && obj.gTile.c == (col - 1));
            Tile right = gameTiles.Find(obj => obj.gTile.r == row && obj.gTile.c == (col + 1));

            Tile top      = gameTiles.Find(obj => obj.gTile.r == (row - 1) && obj.gTile.c == col);
            Tile topLeft  = gameTiles.Find(obj => obj.gTile.r == (row - 1) && obj.gTile.c == (col - 1));
            Tile topRight = gameTiles.Find(obj => obj.gTile.r == (row - 1) && obj.gTile.c == (col + 1));

            Tile bottom      = gameTiles.Find(obj => obj.gTile.r == (row + 1) && obj.gTile.c == col);
            Tile bottomLeft  = gameTiles.Find(obj => obj.gTile.r == (row + 1) && obj.gTile.c == (col - 1));
            Tile bottomRight = gameTiles.Find(obj => obj.gTile.r == (row + 1) && obj.gTile.c == (col + 1));

            // check if objects are not null before adding
            if (left.gTile != null) { neighbouringTiles.Add(left); }
            if (right.gTile != null) { neighbouringTiles.Add(right); }

            if (top.gTile != null) { neighbouringTiles.Add(top); }
            if (topLeft.gTile != null) { neighbouringTiles.Add(topLeft); }
            if (topRight.gTile != null) { neighbouringTiles.Add(topRight); }

            if (bottom.gTile != null) { neighbouringTiles.Add(bottom); }
            if (bottomLeft.gTile != null) { neighbouringTiles.Add(bottomLeft); }
            if (bottomRight.gTile != null) { neighbouringTiles.Add(bottomRight); }
            

            return neighbouringTiles;
        }

        private void GetSurroundingMines(List<Tile> gTiles) {

            foreach(Tile tile in gTiles) {

            }

        }

        private Tile CreateTile(int index, int row, int col) {

            Tile temp = new Tile();

            temp.index = index;

            // Game tile
            temp.gTile.txt = ""; 
            temp.gTile.r   = row; 
            temp.gTile.c   = col;
            temp.gTile.revealed = false;
            temp.gTile.isMine   = false;
            temp.gTile.isDismantled     = false; 
            temp.gTile.surroundingBombs = 0;

            // tile button
            temp.gButton.Height   = 50;
            temp.gButton.FontSize = 16; 
            temp.gButton.Width    = temp.gButton.Height; 

            temp.gButton.Foreground = cGrey;
            temp.gButton.Background = cGrey;
            temp.gButton.IsHitTestVisible = true;


            return temp; 
        }

        private void initializeTiles() {

            int tileIndex = 0;
            List<Tile> newTiles = new List<Tile>(); 

            for(int i = 0; i < rows; i++) {
                for(int j = 0; j < columns; j++) {


                    Tile t = CreateTile(tileIndex, i, j);
                    newTiles.Add(t);
                    tileIndex++; 
                }  
            }

            GameTiles = newTiles; 
        }

        // ==============================

        private int FieldMineCount() {

            int totalMineCnt = minesLeft - dismantledTiles;
            return totalMineCnt; 
        }

        private void GameStatus() {

            // if all tiles but the ones with mines have been revealed, game is won
            if (!gameTiles.Any(x => x.gTile.isMine == false && x.gTile.revealed == false)) {
                EndGame();
            }
        }

        // Gameplay functions ===========

        public void NewGame() {

            //dTimer.Stop(); 
           
            minesLeft = 0;
            minesLeft = mineCount;

            dismantledTiles = 0; 
            minesCreated    = false;
   
            gTime = 0;
            gameTiles = null;
            gameOver  = false;

            initializeTiles(); 

        }

        public void EndGame() {

            dTimer.Stop(); 

            foreach(Tile t in gameTiles) {

                t.gButton.IsHitTestVisible = false;

                if (t.gTile.isMine && !gameOver) {

                    t.gButton.Content = "F";
                    t.gButton.Foreground = cFlag; 
                }

            }    

        }




    }
}
