using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROQWE
{
    struct Node : IComparable<Node>
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
        public Node(int x ,int y , int id)
        {
            Coordinates = new Vector(x,y);
            Id = id;
            Enabled = true;
        }
        public int CompareTo(Node other)
        {
                return Coordinates.CompareTo(other.Coordinates);
        }
        /// <summary>
        /// not recommended
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return this == (Node)obj;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public static bool operator ==(Node a, Node b)
        {
            return (a.CompareTo(b) == 0);
        }
        public static bool operator !=(Node a, Node b)
        {
            return !(a.CompareTo(b) == 0);
        }
        public static bool operator <=(Node a, Node b)
        {
            return (a.CompareTo(b) <= 0);
        }
        public static bool operator >=(Node a, Node b)
        {
            return (a.CompareTo(b) >= 0);
        }
        public static bool operator <(Node a, Node b)
        {
            return (a.CompareTo(b) < 0);
        }
        public static bool operator >(Node a, Node b)
        {
            return (a.CompareTo(b) > 0);
        }
    }
}
