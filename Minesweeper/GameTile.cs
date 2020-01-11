﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace Minesweeper {
   
    public class GameTile {

        public int index   = -1;
        public bool isMine = false;
        public bool revealed     = false;
        public bool isDismantled = false;
       
        public string txt = "";
        SolidColorBrush clr = (SolidColorBrush)(new BrushConverter().ConvertFrom("#bdbdbd"));

        public int r = -1;
        public int c = -1; 


        

        public string Txt {
            get { return txt; }
            set { txt = value; }
        }

        public SolidColorBrush Clr {
            get { return clr; }
            set { clr = value; }
        }

        public int Index {
            get { return index; }
            set { index = value; }
        }

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