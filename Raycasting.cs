using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ROQWE
{
    class Raycasting
    {
        
        public static Entity Cast(Vector start, Vector direction)
        {
            
            /*Cube debug = new Cube((float)(start * Game.Scale + direction * Game.Scale).X, (float)(start * Game.Scale + direction * Game.Scale).Y, 20, 1, Color.Blue, 2)
            {
                Angle = direction.Angle
            };//*/
            List<Entity> RawTiles = new List<Entity>();
            Vector Direction = direction;//(new Vector(1,1) * Vector.RotateVector(direction, Math.PI / 1)).Normalize();
            for (Vector coords = start; (start -coords).Magnitude <= 50; coords += Direction)
            {
                for (int x = -1; x < 2 ; x++)
                {
                    for (int y = -1; y < 2; y++)
                    {
                        for (int z = 0; z < 4; z++)
                        {
                            Entity tile = Game.Level[Game.Where].Find((coords + new Vector(x, y), z));
                            if (!" _".Contains(tile.Type)) RawTiles.Add(tile);
                        }
                    }
                }
            }

            List<Entity> Tiles = RawTiles.Distinct().ToList();
            
            foreach(Entity tile in Tiles)
            {
                //tile.Pic = new Quad(tile.X * Game.Scale, tile.Y * Game.Scale, Game.Scale, Game.Scale, Color.Black, -1);

                //Game.DQD.Add(tile);
                //Game.DQD.Add(tile);
                if (!" _.@".Contains(tile.Type))
                {

                    //tile.Pic = new Quad(tile.X * Game.Scale, tile.Y * Game.Scale, Game.Scale, Game.Scale, Color.Black,4);

                    //Game.DQD.Add(tile);
                    if (Vector.Raycast(start, Direction, tile))
                    {
                        return tile;
                    }
                    else
                    {
                        //debug.SetColor(Color.Red);
                    }
                }
            }

            //Game.DQD.Add(new Entity(0, 0, 'D', Guid.NewGuid(), debug, 10));
            return new Entity(new Vector(0), ' ');
        }
    }
}
