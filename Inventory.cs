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
            set { }
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
        public enum Stat
        {
            Health,
            Length
        } 

        public int itemType;
        private Cube imageQuad;
        public Cube Image
        {
            get
            {
                if (this is null) 
                { 
                    return empty;
                } else 
                { 
                    return imageQuad; 
                }

            }
            set { }
        }
        readonly float[] statuses = new float[(int)Stat.Length];

        public static readonly Cube empty = new Cube((IntVector3D)(0,0,0),1,1,0);
        /// <summary>
        /// creates an item with pic and stats
        /// </summary>
        /// <param name="type"></param>
        /// <param name="pic"></param>
        /// <param name="stats">should be of length Item.Stat.Length</param>
        public Item(int type, Cube pic, float[] stats)
        {
            itemType = type;
            imageQuad = pic;
            statuses = stats;
        }
        public enum ItemIDs
        {
            Empty
        }
        public static Item[] itemTypes = new Item[]
        {
            new Item(0,empty, new float[]{ })
        };
    }
}
