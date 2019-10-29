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

        public IntVector Position2D
        {
            get { return (X, Y); }
            set { X = value.X; Y = value.Y; }
        }
        public IntVector3D Position
        {
            get { return (X, Y, Z); }
            set { X = value.X; Y = value.Y; Z = value.Z; }
        }
        public Cube Pic   { get; set; }
        public char Type  { get; set; }
        public Guid ID    { get; set; }
        public int Health { get; set; }
        public Inventory inventory = new Inventory(4, 7);
        public Entity(int x, int y, char type, Guid Id, Cube pic, int health)
        {
            X = x;
            Y = y;
            ID = Id;
            Type = type;
            Pic = pic;
            Health = health;
        }

        
        public Entity(Vector v, char type)
        {
            X = (int)v.X;
            Y = (int)v.Y;
            Type = type;
        }

        /*public Entity(int x, int y)
        {
            X = x;
            Y = y;
        }//*/
        public int CompareTo(Entity entity)
        {
            if (entity == null) return 1;
            return ID.CompareTo(entity.ID);
        }
        

    }
}
