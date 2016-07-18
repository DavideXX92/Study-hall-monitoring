using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableDAO {
    public class Table {
        int id;      
        int x;
        int y;
        int height;       
        int width;

        public Table(int x, int y, int h, int w) {
            this.id = 0;
            this.x = x;
            this.y = y;
            this.height = h;
            this.width = w;
        }

        public Table(int id, int x, int y, int h, int w) {
            this.id = id;
            this.x = x;
            this.y = y;
            this.height = h;
            this.width = w;
        }

        public int Id {
            get { return id; }
            set { id = value; }
        }

        public int X {
            get { return x; }
            set { x = value; }
        }

        public int Y {
            get { return y; }
            set { y = value; }
        }

        public int Height {
            get { return height; }
            set { height = value; }
        }

        public int Width {
            get { return width; }
            set { width = value; }
        }
    }
}
