using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using MySql.Data.MySqlClient;
using System.Runtime.InteropServices;

namespace ROQWE
{


    class Loader
    {
       
       public static void UploadData(int path, object data)
        {
            string query;
            // Your query
            Type tiote = data.GetType();
            switch (tiote.Name)
            {
                case "Map":
                    Map map = (Map)data;
                    query = "INSERT INTO maps(`MapIndex`) VALUES ('" + path + "')";
                    PushQuery(query);
                    foreach (Chunk CData in map.ModifiedChunks)
                    {
                        query = "INSERT INTO chunks(`ChunkX`,`ChunkY`,`MapId`) VALUES ('" + CData.ChunkCoordinate.X + "', '" + CData.ChunkCoordinate.Y + "', '" + path + "')";
                        PushQuery(query);
                        for (int x = 0; x < Chunk.Size; x++)
                        {
                            for (int y = 0; y < Chunk.Size; y++)
                            {
                                for (int z = 0; z < Chunk.Depth; z++)
                                {
                                    if (CData.Read((x, y, z)) != null)
                                    {
                                        query = "INSERT INTO entity(`ChunkId`,`Type`,`X`,`Y`,`Z`) VALUES ((SELECT ChunkId FROM chunks WHERE ChunkX = " + CData.ChunkCoordinate.X + " AND ChunkY = " + CData.ChunkCoordinate.Y + " AND MapId  = " + path + " ),'" + CData.Read((x, y, z)).Type + "', '" + CData.Read((x, y, z)).X + "', '" + CData.Read((x, y, z)).Y + "', '" + CData.Read((x, y, z)).Z + "')";
                                        PushQuery(query);

                                        //Console.Write(CData.Read((x,y)).Type);
                                    }
                                    else
                                    {
                                        //Console.Write("_");
                                    }
                                }
                            }
                            //Console.WriteLine();
                        }
                    }
                    break;
                case "Chunk":
                    Chunk chunk = (Chunk)data;
                    query = "INSERT INTO chunks(`MapId`,`ChunkX`,`ChunkY`) VALUES ('" + path + "', '" + chunk.ChunkCoordinate.X + "', '" + chunk.ChunkCoordinate.Y + "')";
                    PushQuery(query);
                    break;
                case "Entity":

                    Entity entity = (Entity)data;
                    query = "INSERT INTO entity(`ChunkId`,`Type`,`X`,`Y`,`Z`) VALUES ('" + path + "','" + entity.Type + "', '" + entity.X + "', '" + entity.Y + "', '" + entity.Z + "')";
                    PushQuery(query);
                    break;
            }//*
        }
        public static object PullQuery(string query)
        {
            //change the username, password and database according to your needs
            // You can ignore the database option if you want to access all of them.
            // 127.0.0.1 stands for localhost and the default port to connect.
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=game data;";
            List<string[]> result = new List<string[]>();
            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection)
            {
                CommandTimeout = 60
            };
            MySqlDataReader reader;

            // Let's do it !
            try
            {
                // Open the database
                databaseConnection.Open();
                // Execute the query
                reader = commandDatabase.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {

                        string[] STR = new string[reader.FieldCount];
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            STR[i] = reader.GetString(i);
                        }
                        result.Add(STR);
                    }
                    return result;
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }

                // Finally close the connection
                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                // Show any error message.
                Console.WriteLine(ex.Message);
            }

            commandDatabase.Dispose();
            return null;
        }
        public static void PushQuery(string query)
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=game data;";
            if (query != null)
            {
                // Prepare the connection
                MySqlConnection databaseConnection = new MySqlConnection(connectionString);
                MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection)
                {
                    CommandTimeout = 60
                };
                
                try
                {
                    databaseConnection.Open();
                    MySqlDataReader myReader = commandDatabase.ExecuteReader();
                    //Console.WriteLine("User succesfully registered");

                    databaseConnection.Close();
                }
                catch (Exception ex)
                {
                    // Show any error message.
                    Console.WriteLine(ex.Message);
                }
                commandDatabase.Dispose();
            }
        }

        public static Entity LoadEntity(IntVector3D coordinate, int MapId)
        {
           //change the username, password and database according to your needs
           // You can ignore the database option if you want to access all of them.
           // 127.0.0.1 stands for localhost and the default port to connect.
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=game data;";
            // Your query,
            string query = "SELECT * FROM entity WHERE  X = " + coordinate.X+" AND Y = "+coordinate.Y+ " AND Z = " + coordinate.Z + " AND ChunkId = (SELECT ChunkId FROM chunks WHERE ChunkX = " + (int)Math.Floor(coordinate.X / 16f) + " AND ChunkY = " + (int)Math.Floor(coordinate.Y / 16f) + " AND MapId  = " + MapId + " )";

            // Prepare the connection
            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection)
            {
                CommandTimeout = 60
            };
            MySqlDataReader reader;

            // Let's do it !
            try
            {
                // Open the database
                databaseConnection.Open();

                // Execute the query
                reader = commandDatabase.ExecuteReader();

                // All succesfully executed, now do something

                // IMPORTANT : 
                // If your query returns result, use the following processor :

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        // As our database, the array will contain : ID 0, FIRST_NAME 1,LAST_NAME 2, ADDRESS 3
                        // Do something with every received database ROW
                        string[] row = { reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3) };
                        if (reader.GetString("Type") == "s" && reader.GetInt32("Z") ==  coordinate.Z)
                        {
                            return Types.Snake((reader.GetInt32("X"), reader.GetInt32("Y")));
                        }
                        if (reader.GetString("Type") == "#" && reader.GetInt32("Z") == coordinate.Z)
                        {
                            return Types.Wall((reader.GetInt32("X"), reader.GetInt32("Y")));
                        }
                        if (reader.GetString("Type") == "@" && reader.GetInt32("Z") == coordinate.Z)
                        {
                            return Types.Player((reader.GetInt32("X"), reader.GetInt32("Y")));
                        }

                    }
                }
                else
                {
                    Console.WriteLine("entity not found at x:{0} y{1}", coordinate.X, coordinate.Y);
                    

                }

                // Finally close the connection
                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                // Show any error message.
                Console.WriteLine(ex.Message);
            }

            commandDatabase.Dispose();
            return null;
        }

        public static Map LoadMap(int MapId)
        {
            //change the username, password and database according to your needs
            // You can ignore the database option if you want to access all of them.
            // 127.0.0.1 stands for localhost and the default port to connect.
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=game data;";
            // Your query,
            string query = "SELECT * FROM chunks WHERE MapId = "+MapId+"";

            // Prepare the connection
            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection)
            {
                CommandTimeout = 60
            };
            MySqlDataReader reader;
            Map Result = new Map();


            // Let's do it !
            try
            {
                // Open the database
                databaseConnection.Open();

                // Execute the query
                reader = commandDatabase.ExecuteReader();

                // All succesfully executed, now do something

                // IMPORTANT : 
                // If your query returns result, use the following processor :

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string[] row = { reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3) };
                        Result.ModifiedChunks.Add(LoadChunk((reader.GetInt32("ChunkX"), reader.GetInt32("ChunkY") ), MapId));

                    }
                }
                else
                {
                    Console.WriteLine("no Map with that ID");


                }

                // Finally close the connection
                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                // Show any error message.
                Console.WriteLine(ex.Message);
            }

            commandDatabase.Dispose();
            return Result;
        }

        public static Chunk LoadChunk(IntVector chunkCoordinate, int MapId)
        {
            //change the username, password and database according to your needs
            // You can ignore the database option if you want to access all of them.
            // 127.0.0.1 stands for localhost and the default port to connect.
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=game data;";
            // Your query,
            string query = "SELECT * FROM entity WHERE ChunkId = (SELECT ChunkId FROM chunks WHERE ChunkX = " + chunkCoordinate.X + " AND ChunkY = " + chunkCoordinate.Y + " AND MapId  = " + MapId + " )";

            // Prepare the connection
            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
             
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection)
            {
                CommandTimeout = 60
            };
            MySqlDataReader reader;
            Chunk Result = new Chunk(chunkCoordinate);


            // Let's do it !
            try
            {
                // Open the database
                databaseConnection.Open();

                // Execute the query
                reader = commandDatabase.ExecuteReader();

                // All succesfully executed, now do something

                // IMPORTANT : 
                // If your query returns result, use the following processor :

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        // As our database, the array will contain : ID 0, FIRST_NAME 1,LAST_NAME 2, ADDRESS 3
                        // Do something with every received database ROW
                        string[] row = { reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3) };
                        if (reader.GetString("Type") == "s")
                        {
                            Entity Snake = Types.Snake((reader.GetInt32("X"), reader.GetInt32("Y")));
                            Result.Write(Snake.Position, Snake);
                        }
                        if (reader.GetString("Type") == "#")
                        {
                            Entity Wall = Types.Wall((reader.GetInt32("X"), reader.GetInt32("Y")));
                            Result.Write(Wall.Position, Wall);
                        }
                        if (reader.GetString("Type") == "@")
                        {
                            Entity Player = Types.Player((reader.GetInt32("X"), reader.GetInt32("Y")));
                            Result.Write(Player.Position, Player);
                        }
                        if (reader.GetString("Type") == "_")
                        {
                            Entity Floor = Types.Floor((reader.GetInt32("X"), reader.GetInt32("Y")));
                            Result.Write(Floor.Position, Floor);
                        }
                        if (reader.GetString("Type") == "D")
                        {
                            Entity Door = Types.Door((reader.GetInt32("X"), reader.GetInt32("Y")));
                            Result.Write(Door.Position, Door);
                        }

                    }
                }
                else
                {
                    Console.WriteLine("entity not found at x:{0} y{1}", chunkCoordinate.X, chunkCoordinate.Y);


                }

                // Finally close the connection
                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                // Show any error message.
                Console.WriteLine(ex.Message);
            }

            commandDatabase.Dispose();
            return Result;
        }

        
        public static int LoadTexture(string path)
        {
            if (!File.Exists("Content/" + path))
            {
                throw new FileNotFoundException("file not found at Content/" + path);
            }

            int ID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, ID);

            Bitmap BMP = new Bitmap("Content/" + path);
            BitmapData data = BMP.LockBits(
                new Rectangle(0, 0, BMP.Width, BMP.Height),
                ImageLockMode.ReadOnly,System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            //int ID = GL.GenTexture();
            //GL.BindTexture(TextureTarget.Texture2D, ID);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height,
                0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            BMP.UnlockBits(data);
            BMP.Dispose();
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            
            return ID;

        }
        public static int LoadColor(Color color)
        {

            Bitmap BMP = new Bitmap(1, 1);
            using (Graphics gfx = Graphics.FromImage(BMP))
            using (SolidBrush brush = new SolidBrush(color))
            {
                gfx.FillRectangle(brush, 0, 0, 1, 1);
            }
            BitmapData data = BMP.LockBits(
                new Rectangle(0, 0, BMP.Width, BMP.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            int ID = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, ID);
            

            GL.TexImage2D(TextureTarget.Texture2D, 1, PixelInternalFormat.Rgba, data.Width, data.Height,
                0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            BMP.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
                                                 
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);


            return ID;

        }
    }
}
