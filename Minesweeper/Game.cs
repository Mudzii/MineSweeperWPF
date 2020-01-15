using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.Windows.Controls;  // button
using System.Windows.Threading; // dispatcherTimer
using System.Windows.Media.Imaging;

namespace Minesweeper {

    public class Game {


        public struct Tile {
            public int index;
            public GameTile gTile;
            public Button gButton;

            public List<Image> tImages; 
        }

        // Variables ===============

        SolidColorBrush cGrey = (SolidColorBrush)(new BrushConverter().ConvertFrom("#bdbdbd"));
        
        public int rows    = 9;
        public int columns = 9;

        private List<Tile> gameTiles;

        // mine varialbes
        private int minesLeft  = 0; 
        private int mineCount = 10;
        private int dismantledTiles = 0; 
        private bool minesCreated  = false;

        // game mode variables
        private bool gameOver = false;
        private bool gameWon  = false; 

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
            get { return mineCount; }
            set { mineCount = value; }
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

        public bool GameWon {
            get { return gameWon; }
            set { gameWon = value; }
        }


        // ==============================

        public void UninitializeImages() {

            foreach (Tile t in gameTiles) {

                for (int i = 0; i < 4; i++) {

                    t.tImages[0].Source = null;
                }

            }

        }

        public void InitializeGame() {

            
            minesLeft = mineCount;
           
            gameTiles = new List<Tile>();
            initializeTiles();

        }

        public void UninitializeGame() {

            UninitializeImages(); 

            gameTiles = null;
            cGrey = null; 
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

                if(tile.gTile.isMine == false) {

                    int mineTile = 0;
                    var nTiles = SurroundingTiles(tile.gTile.r, tile.gTile.c); 

                    foreach(Tile t in nTiles) {
                        if (t.gTile.isMine)
                            mineTile++; 
                    }

                    tile.gTile.surroundingBombs = mineTile;

                    if (mineTile == 1)
                        tile.gButton.Foreground = Brushes.Blue; 

                    else if(mineTile == 2)
                        tile.gButton.Foreground = Brushes.Green;

                    else
                        tile.gButton.Foreground = Brushes.Red;



                }

            }

        }

        private void RevealNeighbours(Tile gTile) {

            if(!gTile.gTile.isMine && gTile.gTile.surroundingBombs == 0) {
                gTile.gButton.Content = "";
                gTile.gButton.IsHitTestVisible = false;
                gTile.gButton.Background = Brushes.DarkGray;
            }

            var nTiles = SurroundingTiles(gTile.gTile.r, gTile.gTile.c);
            foreach (Tile gt in nTiles) {

                if(gt.gTile.revealed == false && gt.gTile.isDismantled == false) {
                    gt.gTile.revealed = true;

                    if (gt.gTile.isMine == true && gt.gTile.isDismantled == false) {

                        //gt.gButton.Background = Brushes.Red;
                        //gt.gButton.Background = Brushes.White;
                        gt.gButton.Content = gt.tImages[3];

                        UncoverMines();
                        return;              
                     }

                    if (!gt.gTile.isMine) {

                        if(gt.gTile.surroundingBombs == 0) {

                            gt.gButton.Content = ""; 
                            gt.gButton.IsHitTestVisible = false;
                            gt.gButton.Background = Brushes.DarkGray;

                            RevealNeighbours(gt); 

                        }

                        else {
                            gt.gButton.Background = Brushes.DarkGray;
                            gt.gButton.Content = gt.gTile.surroundingBombs.ToString(); 

                            if (gt.gTile.surroundingBombs == 1)
                                gt.gButton.Foreground = Brushes.Blue;

                            else if (gt.gTile.surroundingBombs == 2)
                                gt.gButton.Foreground = Brushes.Green;

                            else
                                gt.gButton.Foreground = Brushes.Red;

                        }

                    }

                }

            }

            GameStatus(); 
        }

        private Tile CreateTile(int index, int row, int col) {

            Tile temp    = new Tile();
            temp.gTile   = new GameTile();
            temp.gButton = new Button();
            temp.tImages  = new List<Image>();
            

            temp.index = index;

            // Game tile
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
 
            // tile images
            Image defTileImage = new Image();
            defTileImage.Source = new BitmapImage(new Uri("/assets/defTile.png", UriKind.Relative));
            temp.tImages.Add(defTileImage);

            Image flagTileImage = new Image();
            flagTileImage.Source = new BitmapImage(new Uri("/assets/flagTile.png", UriKind.Relative));
            temp.tImages.Add(flagTileImage);

            Image qTileImage = new Image();
            qTileImage.Source = new BitmapImage(new Uri("/assets/qmTile.png", UriKind.Relative));
            temp.tImages.Add(qTileImage);

            Image mineImage = new Image();
            mineImage.Source = new BitmapImage(new Uri("/assets/mineTile.png", UriKind.Relative));
            temp.tImages.Add(mineImage);

            // set button image to default tile
            temp.gButton.Content = temp.tImages[0]; 

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
        private void CreateMines(List<Tile> gTiles) {

            Random rand = new Random();
            int len = gTiles.Count(); 

            int m = 0; 
            while(m < mineCount) {

                int nr = rand.Next(0, len);
                if(!gTiles[nr].gTile.isMine && gTiles[nr].gTile.revealed == false) {
                    gTiles[nr].gTile.isMine = true;
                    m++; 
                }
            }


            minesCreated = true; 
        }

        private void UncoverMines() {

            int len = gameTiles.Count();
            minesLeft = 0; 

            foreach(Tile tile in gameTiles) {

                //tile.gButton.IsHitTestVisible = false;

                if (tile.gTile.isMine) {

                    tile.gTile.revealed  = true;
                    tile.gButton.Content = tile.tImages[3];
                    
                }

            }


            gameOver = true;
            EndGame(); 
        }
        // ==============================

        public void RevealTilesAroundFlag(int ind) {

            Tile tile = gameTiles[ind];

            if (tile.gTile.surroundingBombs > 0 && tile.gTile.revealed == true) {

                int nDisarmed = 0;
                var nTiles = SurroundingTiles(tile.gTile.r, tile.gTile.c);

                nDisarmed = nTiles.Count(x => x.gTile.isDismantled == true);

                if(nDisarmed == tile.gTile.surroundingBombs) {
                    RevealNeighbours(tile); 
                }
            }
        }

        public void RevealTile(int ind) {

            Tile tile = gameTiles[ind];

            if(tile.gTile.isDismantled == false) {

                tile.gTile.revealed = true;

                if (!minesCreated) {
                    CreateMines(gameTiles);
                    GetSurroundingMines(gameTiles); 
                    
                }

              
                if (tile.gButton.Content == tile.tImages[2]) {
                    tile.gButton.Content = "";

                 }

                if (tile.gTile.isMine == true) {

                    tile.gButton.Content = tile.tImages[3];
                    tile.gButton.Background = Brushes.Red;

                    gameOver = true;
                    UncoverMines(); 

                }

                else {

                    if(tile.gTile.surroundingBombs != 0) {
                        tile.gButton.Content = tile.gTile.surroundingBombs.ToString();
                        tile.gButton.Background = Brushes.DarkGray; 
                    }

                    else {
                        tile.gButton.Background = Brushes.DarkGray;
                        RevealNeighbours(tile);
                    }


                    FieldMineCount(); 
                }

            }

            GameStatus(); 

        }

        public void MarkTile(int ind) {

            Tile tile = gameTiles[ind];

            if (tile.gButton.Content == tile.tImages[0]) {
                tile.gTile.isDismantled = true;
                tile.gButton.Content = tile.tImages[1]; 

                dismantledTiles++; 
            }

            else if (tile.gButton.Content == tile.tImages[1]) {
                tile.gButton.Content = tile.tImages[2];
                tile.gTile.isDismantled = false;

                dismantledTiles--; 
            }

            else if (tile.gButton.Content == tile.tImages[2]) {
                tile.gButton.Content = tile.tImages[0];
                tile.gTile.isDismantled = false;

                if (tile.gTile.surroundingBombs == 1)
                    tile.gButton.Foreground = Brushes.Blue;


                else if (tile.gTile.surroundingBombs == 2)
                    tile.gButton.Foreground = Brushes.Green;

                else
                    tile.gButton.Foreground = Brushes.Red;
            }


            FieldMineCount();
            GameStatus(); 
        }

        // ==============================
        public int FieldMineCount() {

            int totalMineCnt = minesLeft - dismantledTiles;
            return totalMineCnt; 
        }

        private void GameStatus() {

            // if all tiles but the ones with mines have been revealed, game is won
            if (!gameTiles.Any(x => x.gTile.isMine == false && x.gTile.revealed == false)) {
                gameWon = true; 
                EndGame();
            }
        }

        // Gameplay functions ===========
    

        public void NewGame() {


            minesLeft = 0;
            minesLeft = mineCount;

            dismantledTiles = 0; 
            minesCreated    = false;
        
            UninitializeImages();

            GameTiles = null; 
            gameOver  = false;
            gameWon   = false;

            initializeTiles(); 

        }

        public void EndGame() {


            foreach(Tile t in gameTiles) {

                t.gButton.IsHitTestVisible = false;

                if (t.gTile.isMine && !gameOver) {
                    t.gButton.Content = t.tImages[1];
                    
                }

            }

        }




    }
}
