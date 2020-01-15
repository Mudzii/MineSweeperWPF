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

        /* =================
         EASY: 9x9 | 10
         MEDIUM: 16x16 | 40
         EXPERT: 16x30 | 99
         =================  */

        public struct Tile {
            public int index;
            public GameTile gTile;
            public Button gButton;

            public List<Image> tImages; 
        }

        // Variables ===============
        private List<Tile> gameTiles;

        // field size 
        private int rows = 9;
        private int columns = 9;

        // mine varialbes
        private int minesLeft  = 0; 
        private int mineCount = 10;
        private int dismantledTiles = 0; 
        private bool minesCreated  = false;

        // game mode variables
        private bool gameOver = false;
        private bool gameWon  = false; 

        // Get/Set  Functions ======
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


        // Initialize functions =========

        // uninitialize images used  
        public void UninitializeImages() {

            foreach (Tile t in gameTiles) {

                for (int i = 0; i < 4; i++) {

                    t.tImages[0].Source = null;
                }

            }

        }

        // Initialize objects
        public void InitializeGame() {

            minesLeft = mineCount;
           
            gameTiles = new List<Tile>();
            initializeTiles();

        }

        // uninitialize objects 
        public void UninitializeGame() {

            UninitializeImages(); 
            gameTiles = null;
            
        }

        // Tile functions ===============

        // get all the tiles surrounding a tile(3x3) 
        private List<Tile> SurroundingTiles(int row, int col) {

            List<Tile> neighbouringTiles = new List<Tile>();

            // get the neighbouring tiles 
            Tile left  = gameTiles.Find(obj => obj.gTile.r == row && obj.gTile.c == (col - 1));
            Tile right = gameTiles.Find(obj => obj.gTile.r == row && obj.gTile.c == (col + 1));

            Tile top      = gameTiles.Find(obj => obj.gTile.r == (row - 1) && obj.gTile.c == col);
            Tile topLeft  = gameTiles.Find(obj => obj.gTile.r == (row - 1) && obj.gTile.c == (col - 1));
            Tile topRight = gameTiles.Find(obj => obj.gTile.r == (row - 1) && obj.gTile.c == (col + 1));

            Tile bottom      = gameTiles.Find(obj => obj.gTile.r == (row + 1) && obj.gTile.c == col);
            Tile bottomLeft  = gameTiles.Find(obj => obj.gTile.r == (row + 1) && obj.gTile.c == (col - 1));
            Tile bottomRight = gameTiles.Find(obj => obj.gTile.r == (row + 1) && obj.gTile.c == (col + 1));

            // check if objects are not null before adding (null means there is no tile)
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

        // check how many mines surround a tile
        private void GetSurroundingMines(List<Tile> gTiles) {

            foreach(Tile tile in gTiles) {

                // if the tile is not mine, check if there are surrounding mines
                if(tile.gTile.isMine == false) {

                    int mineTile = 0;
                    var nTiles = SurroundingTiles(tile.gTile.r, tile.gTile.c); 

                    // if any neighbour is mine, add to counter
                    foreach(Tile t in nTiles) {
                        if (t.gTile.isMine)
                            mineTile++; 
                    }

                    tile.gTile.surroundingBombs = mineTile;

                    // set foreground color based on surroundingMines
                    if (mineTile == 1)
                        tile.gButton.Foreground = Brushes.Blue; 

                    else if(mineTile == 2)
                        tile.gButton.Foreground = Brushes.Green;

                    else if(mineTile == 3)
                        tile.gButton.Foreground = Brushes.Red;

                    else if (mineTile == 4)
                        tile.gButton.Foreground = Brushes.DarkBlue;

                    else if (mineTile == 5)
                        tile.gButton.Foreground = Brushes.DarkRed;

                    else if (mineTile == 6)
                        tile.gButton.Foreground = Brushes.DarkCyan;

                    else if (mineTile == 7)
                        tile.gButton.Foreground = Brushes.Black;

                    else 
                        tile.gButton.Foreground = Brushes.DarkSlateGray;

                }

            }

        }

        // show all neighbouring tiles 
        private void RevealNeighbours(Tile gTile) {

            // reveal the original tile clicked
            if(!gTile.gTile.isMine && gTile.gTile.surroundingBombs == 0) {
                gTile.gButton.Content = "";
                gTile.gButton.IsHitTestVisible = false;
                gTile.gButton.Background = Brushes.DarkGray;
            }

            // get neighbouring tiles
            var nTiles = SurroundingTiles(gTile.gTile.r, gTile.gTile.c);
            foreach (Tile gt in nTiles) {

                // if it's still hidden and not dismantled (flagged) 
                if(gt.gTile.revealed == false && gt.gTile.isDismantled == false) {
                    gt.gTile.revealed = true;

                    // if it is a mine, game is lost
                    if (gt.gTile.isMine) {
                        gt.gButton.Content = gt.tImages[3];
                        UncoverMines();
                        return;              
                     }

                    // else, reveal tiles
                    else if (!gt.gTile.isMine) {

                        if(gt.gTile.surroundingBombs == 0) {

                            gt.gButton.Content = ""; 
                            gt.gButton.IsHitTestVisible = false;
                            gt.gButton.Background = Brushes.DarkGray;

                            RevealNeighbours(gt); 

                        }

                        else {
                            gt.gButton.Background = Brushes.DarkGray;
                            gt.gButton.Content = gt.gTile.surroundingBombs.ToString(); 

                        }

                    }

                }

            }

            GameStatus(); 
        }

        // create a tile 
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

        // create all tiles used 
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

        // mine functions ===============

        // make mines for the field
        private void CreateMines(List<Tile> gTiles) {

            Random rand = new Random();
            int len = gTiles.Count(); 

            // if number of mines is bigger than the field, override count
            if(mineCount > len) {
                mineCount = (rows * columns) - 1; 
            }

            // randomize which tiles are mines 
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

        // uncover all mines on the field 
        private void UncoverMines() {

            minesLeft = 0; 
            foreach(Tile tile in gameTiles) {

                if (tile.gTile.isMine) {

                    tile.gTile.revealed  = true;
                    tile.gButton.Content = tile.tImages[3];  
                }
            }

            gameOver = true;
            EndGame(); 
        }

        // ==============================

        // MMC - reveal all tiles around clicked tile if surroundign flags = tiles surrounding bomb count  
        public void RevealTilesAroundFlag(int ind) {

            Tile tile = gameTiles[ind];

            // check if tile has surrounding bombs and is revealed
            if (tile.gTile.surroundingBombs > 0 && tile.gTile.revealed == true) {

                // check how many surrounding tiles are disarmed
                var nTiles = SurroundingTiles(tile.gTile.r, tile.gTile.c);
                int nDisarmed = nTiles.Count(x => x.gTile.isDismantled == true);

                // reveal neightbours if disarmed tiles = surrounding bombs
                if(nDisarmed == tile.gTile.surroundingBombs) {
                    RevealNeighbours(tile); 
                }
            }
        }

        // LMC - reveal a tile
        public void RevealTile(int ind) {

            Tile tile = gameTiles[ind];

            // only reveal if it isn't flagged
            if(tile.gTile.isDismantled == false) {
                tile.gTile.revealed = true;

                // if first click, created mines and check surrounding mine count for each tile
                if (!minesCreated) {
                    CreateMines(gameTiles);
                    GetSurroundingMines(gameTiles);                
                }

                // if clicked tile is mine = game over
                if (tile.gTile.isMine == true) {
                    tile.gButton.Content = tile.tImages[3];
                    tile.gButton.Background = Brushes.Red;

                    gameOver = true;
                    UncoverMines();
                }

                // if not mine
                else {

                    // if the tile has a number on it, reveal just that tile
                    if(tile.gTile.surroundingBombs != 0) {
                        tile.gButton.Content = tile.gTile.surroundingBombs.ToString();
                        tile.gButton.Background = Brushes.DarkGray; 
                    }

                    // if the tile is empty, reveal surrounding tiles
                    else {
                        tile.gButton.Background = Brushes.DarkGray;
                        RevealNeighbours(tile);
                    }

                    // update mine count
                    FieldMineCount(); 
                }

            }

            // check the game status
            GameStatus(); 
        }


        // MMC - mark/unmark a tile
        public void MarkTile(int ind) {

            Tile tile = gameTiles[ind];

            // if the tile is default, flag tile and 'dismantle'
            if (tile.gButton.Content == tile.tImages[0]) {
                tile.gButton.Content = tile.tImages[1]; 
                tile.gTile.isDismantled = true;

                dismantledTiles++; 
            }

            // if the tile is flagged, set to ? and remove dismantle 
            else if (tile.gButton.Content == tile.tImages[1]) {
                tile.gButton.Content = tile.tImages[2];
                tile.gTile.isDismantled = false;

                dismantledTiles--; 
            }

            // if tile has ?, set to default tile
            else if (tile.gButton.Content == tile.tImages[2]) {
                tile.gButton.Content = tile.tImages[0];
                tile.gTile.isDismantled = false;

            }

            // update mine count and status
            FieldMineCount();
            GameStatus(); 
        }

        // ==============================

        // get the "mine count" for mines and marked tiles
        public int FieldMineCount() {

            int totalMineCnt = minesLeft - dismantledTiles;
            return totalMineCnt; 
        }

        // check if win state is met
        private void GameStatus() {

            // if all tiles but the ones with mines have been revealed, game is won
            if (!gameTiles.Any(x => x.gTile.isMine == false && x.gTile.revealed == false)) {
                gameWon = true; 
                EndGame();
            }
        }

        // Gameplay functions ===========
    
        // initialize a new game
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

        // initialize endgame (make buttons unclickable) + reveal mines
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
