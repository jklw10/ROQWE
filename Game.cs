using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
//using VectorLib;
//using System.Threading.Tasks;

namespace ROQWE
{
    class Game : GameWindow
    {

        [DllImport("kernel32.dll")]
        static extern void OutputDebugString(string lpOutputString);

        int timer;
        static public GameWindow window;
        public static int Where = 0;
        public static List<Map> Level = new List<Map>(50);


        static List<Entity> InView = new List<Entity>();
        static bool debug = false;
        public static int debugS = 0;
        public static Entity Player;

        private static Vector3 cameraAngle;
        public static Vector3 CameraAngle 
        {
            get
            {
                return cameraAngle;
            }
            set
            {
                if (value.Y < Math.PI / 2 && value.Y > -Math.PI / 2)
                {
                    cameraAngle = value;
                }
            }
        }

        static public List<Entity> DQD = new List<Entity>(); //debug quad draw
        

        const float Zoom = 1.0f;
        public const float Scale = 1 * Zoom;
        public static int RenDis = 50;
        

        public Game(int width, int height)
            : base(width, height)
        {
            GL.Enable(EnableCap.Texture2D);
            window = this;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GL.ClearColor(Color.Green);

            Console.WriteLine(new Chunk((9,3)).ToString());
            
            Cube.CreateVisuals();
            Cube.CreateCube();
            GL.Enable(EnableCap.DepthTest);

            Level.Add(new Map());

            Generator map = new Generator(Level[Where]);
            map.Generate();

            DrawMap(Where);
            try
            {
                Cube.Offset = (Player.Position.XY * Scale);
            }
            catch
            {
                Level[Where].Write(Types.Player(new Vector(0)));
                DrawMap(Where);
                Cube.Offset = (Player.Position.XY * Scale);
            }

            //Player.inventory = new Inventory(4, 7);
            //Player.inventory[1, 1] = Item.itemTypes[(int)Item.ItemIDs.Helmet];
            //Console.WriteLine(Player.inventory[1,1].Image);
        }



        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            //PlayerPic.RotateQuad(0.1);
            //ConvertVTOD();
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {

            base.OnMouseMove(e);
            
            if (debug)
            {
                timer++;
                if (timer >= 10)
                {
                    
                    Console.Clear();
                    Console.WriteLine(
                    e.X + ", " + e.Y
                    + "\n" +
                    (e.X - Width / 2) + ", " + (Height / 2 - e.Y)
                    + "\n" +
                    Player.X + ", " + Player.Y
                    + "\n" +
                    "c#:-10%16= "+ -10%16+"  \n"+
                    "real life:-10%16= " + (new Vector(-10) % new Vector(16)).X + "  \n" +
                    new Vector((e.X - Width / 2),(Height / 2 - e.Y)).Angle
                    
                    + "\n" +
                    "tile: " + Level[Where].FindStr((IntVector)Vector.RotateVector(new Vector(e.X - Width / 2, Height / 2 - e.Y), -CameraAngle.Z) /Scale + Player.Position.XY)
                    
                    );
                    timer = 0;
                }
            }
            if (e.Mouse.RightButton == ButtonState.Pressed)
            {
                CameraAngle -= new Vector3(0,0, e.XDelta / 75f);
                if (CameraAngle.Y - e.YDelta / 75f < Math.PI/2 && CameraAngle.Y - e.YDelta / 75f > -Math.PI/2)
                {
                    CameraAngle -= new Vector3(0,e.YDelta / 75f,0);
                }
            }
            if (!e.Position.IsEmpty) { Player.Pic.SetAngle(new Vector(e.X - Width/2, Height/2 - e.Y).Angle + CameraAngle.Z); };
            
        }
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            Cube.Zoom += (e.DeltaPrecise / 10f);

        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {

            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Player.Pic.DrawInWorld(CameraAngle, Player.Position);
            lock (InView)
            {
                foreach (Entity entity in InView)
                {
                    //draws the map in 3d for player to move in
                    entity.Pic.DrawInWorld(CameraAngle, Player.Position);
                    //draws the minimap
                    entity.Pic.DrawOnScreen(CameraAngle, Player.Position, new Vector4(window.X-600,window.X, window.Y - 600, window.Y));
                }
            }
            for(int x = 0; x < Player.inventory.size.X; x++)
            {//draws each item in inventory on screen 
                for (int y = 0; y < Player.inventory.size.Y; y++)
                {
                   //Player.inventory[x, y].Image.DrawInWorld(new Vector3(0, 1, 0), new Vector3(0,0,1));//,new Vector4(0,window.X,0,window.Y));
                }
            }
            //foreach (Entity debug in DQD)
            {

                //debug.Pic.Draw();

            }

            GL.End();
            GL.Flush();
            SwapBuffers();

        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            Cube.ScreenSize = (Vector)this;
            Cube.Offset = Player.Position.XY * Scale;

        } 

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.F)
            {
                debug = !debug;
            }
            if (e.Key == Key.Right)
            {
                CameraAngle -= new Vector3(0, 0, 0.1f);
            }
            if (e.Key == Key.Left)
            {
                CameraAngle += new Vector3(0, 0, 0.1f);
            }
            if (e.Key == Key.Up)
            {
                CameraAngle -= new Vector3(0, 0.1f, 0);
            }
            if (e.Key == Key.Down)
            {
                CameraAngle += new Vector3(0, 0.1f, 0);
            }
            if (e.Key == Key.U)
            {
                DrawMap(Where);
            }
            if (e.Key == Key.H)
            {
                Thread t = new Thread(() =>
                {

                    Console.WriteLine("thread started");
                    if (Thread.CurrentThread.Name == null)
                    {
                        Thread.CurrentThread.Name = "Raycast";
                    }
                    else
                    {
                        Console.WriteLine("Unable to name a previously " +
                            "named thread.");
                    }
                    Vector position = Player.Position.XY;
                    double Angle = Player.Pic.Angle;
                    for (double rad = -Math.PI; rad < Math.PI; rad += 0.01)
                    {

                        Raycasting.Cast(position, Vector.RadiansToVector(Angle + rad, 1));
                        //Console.WriteLine("shot:" + shot.Type + "  X:" + shot.X + "  Y:" + shot.Y);
                    }
                    lock (InView)
                    {
                        InView.AddRange(DQD);
                        DQD = new List<Entity>();
                    }
                    Console.WriteLine("thread stop");
                });

                if (!(t.ThreadState == System.Threading.ThreadState.Running))
                {
                    t.Start();
                }
            }
            if (e.Key == Key.G)
            {
                if(debugS > 24)
                {
                    debugS++;
                }
                else
                {
                    debugS = 0;
                }
            }
            if (Controls(this, e))
            {
                foreach (Entity entity in InView)
                {
                    if(entity.Type == Types.snakeType)
                    {
                        EnemyThink(entity);
                    }
                    
                }
            }
            if (e.Key == Key.I)
            {
                for (int y = 0; y < Player.inventory.size.Y; y++)
                {//draws each item in inventory on screen 
                    for (int x = 0; x < Player.inventory.size.X; x++)
                    {
                        Console.Write(Player.inventory[x,y] +" ");
                    }
                    Console.WriteLine();
                }
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButton.Left)
            {
                Entity shot = Raycasting.Cast(Player.Position.XY, Vector.RadiansToVector(Player.Pic.Angle, 1));
                Console.WriteLine("shot:" + shot.Type + "  X:" + shot.X + "  Y:" + shot.Y);
                Entity lit = InView.Find(x => x.Position == shot.Position);
                if (!(lit is null)) { lit.Pic.highlight -= 10; }


                if (shot.Type == Types.snakeType)
                {
                    Level[Where].Find(shot.Position).Health -= 1;
                    if (Level[Where].Find(shot.Position).Health <= 0)
                    {
                        InView.RemoveAt(InView.FindIndex(x => x.Position.XY == shot.Position.XY));
                        Level[Where].RemoveAt(shot.Position);
                    }
                }
            }
        }

        /// <summary>
        /// Loads quads onto map files.
        /// </summary>
        static void DrawMap(int where)
            {
            char Character;
            InView.Clear();
            InView = new List<Entity>();
            Player = null;
            for (int C = 0; C < Level[where].ModifiedChunks.Count; C++)
            {
                for (int x = 0; x < Chunk.Size; x++)
                {
                    for (int y = 0; y < Chunk.Size; y++)
                    {
                        for (int z = 0; z < Chunk.Depth; z++)
                        {
                            IntVector Pos = (Level[where].ModifiedChunks[C].ChunkCoordinate) * Chunk.Size + (x, y);
                            Character = Level[where].FindChar(Pos.X, Pos.Y,z);
                            //DQD.Add(Types.Wall(Pos));
                            if (Character == Types.playerType)
                            {
                                Player = Types.Player((Pos, z));
                            }
                            if (Character == Types.snakeType)
                            {
                                InView.Add(Types.Snake((Pos,z)));
                            }
                            if (Character == Types.wallType)
                            {
                                InView.Add(Types.Wall((Pos, z)));
                            }
                            if (Character == Types.floorType)
                            {
                                InView.Add(Types.Floor((Pos, z)));
                            }
                            if (Character == Types.doorType)
                            {
                                InView.Add(Types.Door((Pos, z)));
                            }
                            
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Makes enemy move.
        /// </summary>
        /// <param name="enemy"></param>
        static void EnemyThink(Entity enemy)
        {
            MoveEntity(enemy, Pathfinding.NextStep(enemy.Position.XY ,Player.Position.XY));
        }

        /// <summary>
        /// Checks if input was an actual control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        static bool Controls(object sender, KeyboardKeyEventArgs key)
        {

            switch (key.Key)
            {
                case Key.W:
                    MoveEntity(Player, Vector.RotateVector(new Vector(-1, 0), CameraAngle.Z));
                    return true;
                case Key.A:
                    MoveEntity(Player, Vector.RotateVector(new Vector(0, -1), CameraAngle.Z));
                    return true;
                case Key.S:
                    MoveEntity(Player, Vector.RotateVector(new Vector(1, 0), CameraAngle.Z));
                    return true;
                case Key.D:
                    MoveEntity(Player, Vector.RotateVector(new Vector(0, 1), CameraAngle.Z));
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Moves an entity.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        static void MoveEntity(Entity character, IntVector move)
        {
            Vector NextSpot = character.Position.XY + move;
            Entity NextSpotEntity = Level[Where].Find(character.Position + move);
            switch (NextSpotEntity.Type)
            {
                case Types.doorType:
                case Types.floorType:
                case ' ':
                    //if (NextSpot < new Vector(Map.Size) || NextSpot > new Vector(0))
                    {
                        Level[Where].RemoveAt(character.Position);
                        character.Position = (NextSpot, character.Z);
                        Level[Where].Write(character);

                        if (character.Type == Types.snakeType)
                        {
                            character.Pic.MoveQuad(move * Scale);
                        }
                        if (character.Type == Types.playerType)
                        {
                            Player.Pic.MoveQuad(move * Scale);
                            Cube.Offset += ((Vector)move * Scale);
                        }
                    }
                    break;
                case Types.snakeType:
                case Types.playerType:
                    Attack(character,NextSpotEntity);
                    break;
                default:
                    break;
            }

        }
        public static void Attack(Entity attacker, Entity other)
        {
            Stats attack = attacker.inventory.TotalStats() + attacker.BaseStats;
            Stats defend = other.inventory.TotalStats() + other.BaseStats;
            if (attack.damage > 0)
            {
                other.Health -= Math.Abs((attack.damage - (defend.defence / (Math.Abs(attack.damage - defend.defence)+2))));
                if (other.Health <= 0)
                {
                    if(other != Player)
                    {
                        InView.Remove(other);
                        Level[Where].RemoveAt(other.Position);
                    }
                    Node emptyslot = attacker.inventory.FindEmptySlot();
                    if (emptyslot.Enabled)
                    {
                        Item item = new Item();
                        item.RollStats();
                        attacker.inventory.SetItem(emptyslot.Coordinates,item);
                    }
                }
            }
        }
    }
}
