using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotDao {
    public enum State{ Learning, Monitoring}

    public class Spot {
        private int id;
        private int x;
        private int y;
        private int count;       
        private Boolean isFree;
        private Boolean oldState;

        public Spot(int x, int y) {
            this.Id = 0;
            this.X = x;
            this.Y = y;
            this.IsFree = false;
            this.Count = 1;
        }

        public Spot(int id, int x, int y, bool isFree, int count) {
            this.Id = id;
            this.X = x;
            this.Y = y;
            this.IsFree = isFree;
            this.OldState = isFree;
            this.Count = count;
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

        public int Count {
            get { return count; }
            set { count = value; }
        }

        public Boolean IsFree {
            get { return isFree; }
            set { isFree = value; }
        }

        public Boolean OldState {
            get { return oldState; }
            set { oldState = value; }
        }
    }
}
