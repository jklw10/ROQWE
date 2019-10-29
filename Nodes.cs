using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROQWE
{
    class Node : IComparable<Node>
    {
        public Vector Coordinates { get; set; }
        public int Id { get; set; }
        public bool Enabled { get; set; }

        public override string ToString()
        {
            return "(" + Coordinates.X+","+Coordinates.Y+")"+":"+Id+"="+Enabled;
        }
        public Node(Vector coordinates, int id)
        {
            Coordinates = coordinates;
            Id = id;
            Enabled = true;
        }
        public Node(Vector coordinates)
        {
            Coordinates = coordinates;
            Enabled = true;
        }
        public Node(int x ,int y , int id)
        {
            Coordinates = new Vector(x,y);
            Id = id;
            Enabled = true;
        }
        public int CompareTo(Node other)
        {
            if(other == null) { return 1; }
            else
            {
                return Coordinates.CompareTo(other.Coordinates);
            }

        }
    }
}
