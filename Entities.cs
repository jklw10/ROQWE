using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROQWE
{
    class Entity : IComparable<Entity>
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }


        public IntVector3D Position
        {
            get { return (X, Y, Z); }
            set { X = value.X; Y = value.Y; Z = value.Z; }
        }
        public Cube Pic { get; set; }
        public char Type { get; set; }
        public Guid ID { get; set; }
        public Stats BaseStats { get; set; }
        public float Health { get; set; }

        public Inventory inventory = new Inventory(4, 7);
        public Entity(int x, int y, int z, char type, Guid Id, Cube pic, Stats stats)
        {
            X = x;
            Y = y;
            Z = z;
            ID = Id;
            Type = type;
            Pic = pic;
            BaseStats = stats;
            Health = stats.health;
        }
        public override string ToString()
        {
            return "type: " + Type + ", position: " + Position;
        }

        public Entity(Vector v, char type)
        {
            X = (int)v.X;
            Y = (int)v.Y;
            Type = type;
        }

        public int CompareTo(Entity other)
        {
            return ID.CompareTo(other.ID);
        }
    }
}
