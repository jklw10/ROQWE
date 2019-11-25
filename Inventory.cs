using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace ROQWE
{
    class Inventory //: IEnumerable<Item>
    {
        public Vector size;
        public Item[,] itemArray = new Item[,] { };
        public Item this[int x, int y]
        {
            get 
            {
                return itemArray[x, y];
            }
            set 
            {
                itemArray[x, y] = value;
            }
        }
        public Inventory(int width, int height)
        {
            size = new Vector(width, height);
            itemArray = new Item[width, height];
        }
        /// <summary>
        /// changes item at position to item
        /// </summary>
        /// <param name="position"></param>
        /// <param name="item"></param>
        public void SetItem(IntVector position, Item item)
        {
            itemArray[position.X, position.Y] = item;
        }
        public Node FindEmptySlot()
        {
            Node returned = new Node(0, 0, 0) { Enabled = false };
            for (int x = 0; x < size.X; x++)
            {
                for (int y = 0; y < size.Y; y++)
                {
                    if (itemArray[x,y].itemType == (int)Item.ItemIDs.Empty)
                    {
                        return new Node(x, y,0) { Enabled = true };
                    }
                }
            }
            return returned;
        }
        public Stats TotalStats()
        {
            Stats returned = new Stats(0);
            for (int x = 0; x < size.X; x++)
            {
                for (int y = 0; y < size.Y; y++)
                {
                    if (this[x, y].equipped)
                    {
                        returned += this[x, y].Statuses;
                    }
                }
            }
            return returned;
        }
    }
    class Item 
    {
        public int itemType;
        public bool equipped;
        private Stats statuses;
        public Stats Statuses 
        { 
            get 
            {
                return statuses;
            } 
        }
        private Cube image;
        public Cube Image
        {
            get
            {
                return image; 

            }
            set 
            {
                image = value;
            }
        }
        public override string ToString()
        {
            return "{" + statuses+"}";
        }
        public void RollStats()
        {
            Random rng = new Random(DateTime.Now.Millisecond);
            statuses.damage = (float)rng.NextDouble() * 10;
            statuses.health = (float)rng.NextDouble() * 100;
            statuses.defence = (float)rng.NextDouble() * 50;
        }
        //public static readonly Cube emptySlot = new Cube((IntVector3D)(0, 0, 0), 1, 1, 1, 0);
        /// <summary>
        /// creates an item with pic and stats
        /// </summary>
        /// <param name="type"></param>
        /// <param name="pic"></param>
        /// <param name="stats">should be of length Item.Stat.Length</param>
        public Item(int type, Cube pic, Stats stats)
        {
            itemType = type;

            image = pic;
            statuses = stats;
            equipped = false;
        }
        /// <summary>
        /// creates a new empty item
        /// </summary>
        /*public Item()
        {
            itemType = (int)ItemIDs.Empty;

            image = new Cube((IntVector3D)(0, 0, 0), 1, 1, 1, 0);
            statuses = new Stats(0, 0, 0);
        }//*/
        public enum ItemIDs
        {
            Empty,
            Helmet,
            Chestplate,
            Pants,
            Boots,
            Weapon,
            Accessories,
            Misc
        }

    }
    struct Stats 
    {
        public float health;
        public float damage;
        public float defence;
        public Stats(float health,float damage, float defence)
        {
            this.health = health;
            this.damage = damage;
            this.defence = defence;
        }
        public Stats(float all)
        {
            this.health = all;
            this.damage = all;
            this.defence = all;
        }
        public override string ToString()
        {
            return (int)Math.Round(health) +" hp " + (int)Math.Round(damage) + " dmg " + (int)Math.Round(defence) + " def";
        }

        /// <summary>
        /// adds values together
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Stats operator +(Stats a, Stats b)
        {
            return new Stats(a.health + b.health, a.damage + b.damage, a.defence + b.defence);
        }
        public static Stats operator -(Stats a, Stats b)
        {
            return new Stats(a.health + b.health, a.damage + b.damage, a.defence + b.defence);
        }
    }
}
