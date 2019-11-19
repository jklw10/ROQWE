using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROQWE
{
    class Pathfinding
    {
        public static Vector NextStep(Vector position, Vector target)
        {
            Vector direction = target - position;
            Vector Direction = new Vector(0);
            if (position != target) 
            {
                if (Math.Abs(direction.X) > Math.Abs(direction.Y))
                {
                    Direction.X = Math.Sign(direction.X);
                }
                else
                {
                    Direction.Y = Math.Sign(direction.Y);
                }
            }
            return new Vector(Direction.X, Direction.Y);
        }
        public static Vector StraightPath(Vector position, Vector target)
        {
            Vector direction = target - position;
            Vector Direction = new Vector(0);
            if (position != target)
            {
                if (Math.Abs(direction.X) < Math.Abs(direction.Y))
                {
                    Direction.X = Math.Sign(direction.X);
                }
                else
                {
                    Direction.Y = Math.Sign(direction.Y);
                }
            }
            return new Vector(Direction.X, Direction.Y);
        }
    }
}
