using System;
using System.Collections.Generic;
using System.Text;



namespace Minesweeper {

    public class Game {


        // Variables ===============
        public int mines = 1; //10;
        public int minesLeft;
        public int dismantledTiles = 0;

        public int rows = 4;//9;
        public int columns = 4; //9;

        public bool gameEnded = false; 

        // Get/Set =================
        public int NrRows {
            get { return rows; }
            set { rows = value; }
        }

        public int NrColumns {
            get { return columns; }
            set { columns = value;
   
            }
        }
       
        public int MineAmount {
            get { return mines; }
            set {  mines = value; }

        }

        public bool GameEnded {
            get { return gameEnded; }
            set { gameEnded = value; }
        }

        // Functions ===============
        public void NewGame() {

            minesLeft = mines;
            dismantledTiles = 0; 

        }

    


    }
}
