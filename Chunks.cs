using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Filler;

namespace ROQWE
{
    class Map
    {
        public List<Chunk> ModifiedChunks = new List<Chunk>();
        public Map()
        {
        }
        public void RemoveAt(IntVector3D pos)
        {
            RemoveAt(pos.X, pos.Y, pos.Z);
        }
        public void RemoveAt(IntVector pos, int layer)
        {
            RemoveAt(pos.X, pos.Y, layer);
        }
        public void RemoveAt(int x, int y, int z)
        {
            IntVector CC = new IntVector((int)Math.Floor((x / 16f)), (int)Math.Floor((y / 16f)));          //chunk in map coordinate
            IntVector CR = new IntVector(x,y) % (16, 16);      //chunk  relative to itself coordinate
            int index = ModifiedChunks.FindIndex(X => X.ChunkCoordinate == CC);
            if (index != -1)
            {
                ModifiedChunks[index].Write((CR.X, CR.Y, z), null);
            }
        }
        public void Write(IntVector pos, int layer, char type, Cube pic, int health)
        {
            Entity entity = new Entity(pos.X, pos.Y, type, Guid.NewGuid(), pic, health) { Z = layer};
            Write(entity);
        }
        public void Write(int x, int y, int z, char type, Cube pic, int health)
        {
            Entity entity = new Entity(x, y, type, Guid.NewGuid(), pic, health) {Z = z };
            Write(entity);
        }
        public void Write(Entity entity)
        {
            IntVector CC = new IntVector((int)Math.Floor((entity.X / 16f)), (int)Math.Floor((entity.Y / 16f)));         //chunk in map coordinate
            IntVector CR = entity.Position2D % (16, 16);                                                 //chunk  relative to itself coordinate
            int index = ModifiedChunks.FindIndex(X => X.ChunkCoordinate == CC);

            if (index == -1)
            {
                Chunk Written = new Chunk(CC);
                Written.Write((CR.X, CR.Y, entity.Z), entity);
                ModifiedChunks.Add(Written);
            }
            else
            {
                ModifiedChunks[index].Write((CR.X,CR.Y, entity.Z), entity);
            }

        }

        

        public char FindChar(IntVector3D position)
        {
            return FindChar(position.X, position.Y, position.Z);
        }
        public char FindChar(int x,int y, int z)
        {
            IntVector CC = new IntVector((int)Math.Floor((x / 16f)), (int)Math.Floor((y / 16f)));         //chunk in map coordinate
            IntVector CR = new IntVector(x, y) % (16, 16);      //chunk  relative to itself coordinate
            Chunk Read = ModifiedChunks.Find(X => X.ChunkCoordinate == CC);
            if(Read == null || Read.Read((CR.X, CR.Y, z)) == null)
            {
                return ' ';
            }
            else
            {
                char character = Read.Read((CR.X, CR.Y, z)).Type;
                
                return character;
            }
        }
        /// <summary>
        /// returns a string of characters from the spcified XY coordinate,
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public string FindStr(IntVector position)
        {
            return FindStr(position.X, position.Y);
        }
        public string FindStr(int x, int y)
        {
            IntVector CC = new IntVector((int)Math.Floor((x / 16f)), (int)Math.Floor((y / 16f)));         //chunk in map coordinate
            IntVector CR = new IntVector(x, y) % (16, 16);      //chunk  relative to itself coordinate
            Chunk Read = ModifiedChunks.Find(C => C.ChunkCoordinate == CC);
            string returned = "";
            if (Read == null)
            {
                return "    ";
            }
            for (int z = 0; z < Chunk.Depth; z++)
            {
                if (Read.Read((CR.X, CR.Y, z)) == null)
                {
                    returned += " ";
                }
                else
                {
                    char character = Read.Read((CR.X, CR.Y, z)).Type;

                    returned += character;
                }
            }
            return returned;
        }

        public Entity Find(IntVector3D position)
        {
            return Find(position.X, position.Y, position.Z);
        }
        public Entity Find(int x, int y, int z)
        {
            IntVector CC = new IntVector((int)Math.Floor((x / 16f)), (int)Math.Floor((y / 16f)));         //chunk in map coordinate
            IntVector CR = new IntVector(x, y) % (16, 16);       //chunk  relative to itself coordinate
            Chunk Read = ModifiedChunks.Find(X => X.ChunkCoordinate == CC);
            if (Read == null)
            {
                return new Entity(CR.X, CR.Y, ' ', Guid.NewGuid(), null, 1);
            }
            else
            {
                if(Read.Read((CR.X, CR.Y, z)) == null)
                {
                    return new Entity(CR.X, CR.Y, ' ', Guid.NewGuid(), null, 1);
                }
                Entity character = Read.Read((CR.X, CR.Y, z));
                return character;
            }

        }

    }
    class Chunk
    {
        public IntVector ChunkCoordinate { get; set; }

        
        public static int Size = 16;
        public static int Depth = 4;
        public Entity[,,] ChunkData = new Entity[Size,Size,Depth];

        public Chunk(IntVector chunkCoordinate)
        {
            ChunkCoordinate = chunkCoordinate;
        }
        public Chunk(int x,int y)
        {
            ChunkCoordinate = new IntVector(x,y);
        }
        public void Write(IntVector3D c, Entity entity)
        {
            ChunkData[c.X, c.Y, c.Z] = entity;
        }
        public Entity Read(IntVector3D c)
        {
            return ChunkData[c.X, c.Y, c.Z];
        }
    }
}
