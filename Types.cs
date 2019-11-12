using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using VectorLib;

namespace ROQWE
{

    
    class Types
    {

        readonly static Color door = Color.Brown;
        readonly static Color snake = Color.Green;
        readonly static int wall = Loader.LoadTexture("images.jpg");
        readonly static int player = Loader.LoadTexture("images.jpg");
        readonly static Color floor = Color.Gray;
        public const char snakeType = 'S';
        public const char wallType = '#';
        public const char playerType = '@';
        public const char doorType = 'D';
        public const char floorType = '_';
        /// <summary>
        /// makes a entity type based on default values (in the future from an xml file)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Entity Snake(int x, int y, int z)
        {
            return new Entity(x, y,z, snakeType, Guid.NewGuid(), new Cube(x,y,z,Game.Scale, Game.Scale, Game.Scale, snake), 10);
        }
        public static Entity Wall(int x, int y, int z)
        {
            return new Entity(x, y,z, wallType, Guid.NewGuid(), new Cube(x, y,z, Game.Scale, Game.Scale, Game.Scale, wall),  1);
        }
        public static Entity Player(int x, int y, int z)
        {
            return new Entity(x, y,z, playerType, Guid.NewGuid(), new Cube(x,y,z, Game.Scale, Game.Scale, Game.Scale, player), 10);
        }
        public static Entity Door(int x, int y, int z)
        {
            return new Entity(x, y,z, doorType, Guid.NewGuid(), new Cube(x,y,z, Game.Scale, Game.Scale, Game.Scale, door), 10);
        }
        public static Entity Floor(int x, int y, int z)
        {
            return new Entity(x, y,z, floorType, Guid.NewGuid(), new Cube(x,y,z, Game.Scale, Game.Scale, 0.01f, floor), 10);
        }
        public static Entity Door(IntVector3D Pos)
        {
            return Door(Pos.X, Pos.Y, Pos.Z);
        }
        public static Entity Floor(IntVector3D Pos)
        {
            return Floor(Pos.X, Pos.Y, Pos.Z);
        }
        public static Entity Snake(IntVector3D Pos)
        {
            return Snake(Pos.X, Pos.Y, Pos.Z);
        }
        public static Entity Wall(IntVector3D Pos)
        {
            return Wall(Pos.X, Pos.Y, Pos.Z);
        }
        public static Entity Player(IntVector3D Pos)
        {
            return Player(Pos.X, Pos.Y, Pos.Z);
        }
        public static Entity Door(IntVector Pos)
        {
            return Door((Pos, 1));
        }
        public static Entity Floor(IntVector Pos)
        {
            return Floor((Pos, 0));
        }
        public static Entity Snake(IntVector Pos)
        {
            return Snake((Pos,1));
        }
        public static Entity Wall(IntVector Pos)
        {
            return Wall((Pos,1));
        }
        public static Entity Player(IntVector Pos)
        {
            return Player((Pos, 1));
        }
    }
}
