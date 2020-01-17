using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace Minesweeper {
   
    public class GameTile {

     
        public bool isMine = false;
        public int surroundingBombs = 0; 

        public bool revealed     = false;
        public bool isDismantled = false;

        public int r = -1;
        public int c = -1; 
        

        public bool IsMine {
            get { return isMine; }
            set { isMine = value; }
        }

        public bool Revealed {
            get { return revealed; }
            set { revealed = value; }
        }

        public bool IsDismantled {
            get { return isDismantled; }
            set { isDismantled = value; }
        }

        public int PosRow {
            get { return r; }
            set { r = value; }
        }

        public int PosCol {
            get { return c; }
            set { c = value; }
        }

    }
}
