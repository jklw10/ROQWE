using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using VectorLib;

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
                if (itemArray[x, y] is null)
                {//if the item at point in array is null get an empty item
                    Item returned = Item.itemTypes[(int)Item.ItemIDs.Empty];
                    returned.Image.MoveQuad(x,y);
                    return returned;
                }
                else
                {
                    return itemArray[x, y];
                }
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
        public void ItemAt(IntVector position, Item item)
        {
            itemArray[position.X, position.Y] = item;
        }
    }
    class Item
    {
        public int itemType;
        public bool equipped;

        private Cube image;
        public Cube Image
        {
            get
            {
                if (image is null) 
                {
                    return empty;
                } else 
                { 
                    return image; 
                }

            }
            set 
            {
                image = value;
            }
        }
        readonly Stats statuses;

        public static readonly Cube empty = new Cube((IntVector3D)(0,0,0),1,1,1,0);
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
        }
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
        public static Item[] itemTypes = new Item[]
        {
            new Item((int)ItemIDs.Empty,empty, new Stats(0,0,0))
        };
    }
    class Stats 
    {
        float health;
        float damage;
        float defence;
        public Stats(float health,float damage, float defence)
        {
            this.health = health;
            this.damage = damage;
            this.defence = defence;
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
