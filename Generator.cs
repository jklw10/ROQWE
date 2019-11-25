using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Filler;

namespace ROQWE
{
    class Generator
    {
        readonly Map Level;
        public List<List<Room>> connections = new List<List<Room>> { };
        public List<Room> Rooms = new List<Room> { };
        public Generator(Map level)
        {
            Level = level;
        }
        
        public void Generate()
        {
            //generates rooms
            for (int i = 0; i < 10; i++)
            {
                IntVector size = (10, 10);
                Random rng = new Random();
                IntVector start = (rng.Next(0, 200), rng.Next(0, 200));
                bool possible = true;
                //checks if room overlaps with other rooms
                foreach (Room room in Rooms)
                {
                    if (!(start > room.end || start + size < room.start))
                    {
                        possible = false;
                    }
                }
                if (possible)
                {
                    CreateBox(size, start, i);
                    if (i == 0)
                    {
                        //puts player into the first room that is generated
                        Level.Write(Types.Player(start + size / 2));
                    }
                    else
                    {

                        Level.Write(Types.Snake(start + size / 2));
                    }
                }
                else
                {
                    i--;
                }
            }
            //generates pathing between rooms
            for (int i = 0; i < Rooms.Count; i++)
            {
                Rooms[i].enabled = false;
                Room[] roomSet = FindClosestRooms(Rooms.ToArray(), new Room[] { Rooms[i] }, true);
                Rooms[i].enabled = true;


                //takes the rooms bottom left corner and tries to find a rooms nodes based on id's

                bool found = false; 
                List<int> ids = new List<int>(); //List for room id's to check if an id has already been found earlier'

                //tests if connections to rooms made earlier exist
                for (int x = 0; x < connections.Count; x++)
                {
                    for (int y = 0; y < connections[x].Count; y++)
                    {
                        if (connections[x][y].Id == roomSet[0].Id && !ids.Contains(roomSet[0].Id))
                        {
                            ids.Add(roomSet[0].Id);
                            connections[x].Add(roomSet[0]);
                            found =true;
                        }
                        else if (connections[x][y].Id == roomSet[1].Id && !ids.Contains(roomSet[1].Id))
                        {
                            ids.Add(roomSet[1].Id);
                            connections[x].Add(roomSet[1]);
                            found = true;
                        }
                    }
                }
                if (!found) // if no connections between one of the rooms and other rooms is found then make a new connection list
                {
                    ids.Add(roomSet[0].Id);
                    ids.Add(roomSet[1].Id);
                    connections.Add(roomSet.ToList());
                }
                CreatePathing(roomSet[0], roomSet[1]);
            }
            //connects all unconnected rooms
            for (int x = 0; x < connections.Count-1; x++)
            {
                Room[] candidates = null;
                //gets 2 closest rooms from 2 room clusters
                for (int y = 0; y < connections[x].Count; y++)
                {
                    candidates = FindClosestRooms(connections[x].ToArray(), connections[x+1].ToArray(),false);
                }
                CreatePathing(candidates[0],candidates[1]);
            }
        }
        /// <summary>
        /// find's the 2 closest nodes in the room returned in an array where first element is the node from the first room and second element fromt he second room
        /// </summary>
        /// <param name="room1"></param>
        /// <param name="room2"></param>
        /// <param name="ignoreSecond"></param>
        /// <returns></returns>
        public static Node[] FindClosest(Node[] room1, Node[] room2, bool ignoreSecond)
        {
            List<Node> comparisons = new List<Node>();

            for (int y = 0; y < room2.Length; y++)
            {
                for (int x = 0; x < room1.Length; x++)
                {
                    if (room1[x].Enabled) //checks if node is usable
                    {
                        if (room2[y].Enabled || ignoreSecond) //checks if node is usable
                        {
                            comparisons.Add(new Node(room1[x].Coordinates - room2[y].Coordinates, x + y * room1.Length)); //takes the distance between 2 nodes adds it to a list with identifier based on node locations in sets
                        }
                    }
                }
            }
            comparisons.Sort();//sorts distances to eachother from the shortest to longest 
            Node start  = room1[comparisons[0].Id % room1.Length]; //selects node from the list that had the shortest distance to nodeset2
            Node end    = room2[comparisons[0].Id / room1.Length]; //same for for this set
            return new Node[] { start, end };
        }
        /// <summary>
        /// finds closest 2 rooms from a list
        /// </summary>
        /// <param name="nodeSet1"></param>
        /// <param name="nodeSet2"></param>
        /// <returns></returns>
        public static Room[] FindClosestRooms(Room[] room1, Room[] room2, bool ignoreSecond)
        {
            List<Node> comparisons = new List<Node>();

            for (int y = 0; y < room2.Length; y++)
            {
                for (int x = 0; x < room1.Length; x++)
                {
                    if (room1[x].enabled)
                    {
                        if (room2[y].enabled || ignoreSecond)
                        {
                            comparisons.Add(new Node(room1[x].start - room2[y].start, x + y * room1.Length));//takes distance between 2 rooms
                        }
                    } 
                     
                }
            }
            comparisons.Sort();//sorts distances to eachother from the shortest to longest 
            Room start  = room1[comparisons[0].Id % room1.Length]; //selects node from the list that had the shortest distance to nodeset2
            Room end    = room2[comparisons[0].Id / room1.Length]; //same for for this set
            return new Room[] { start, end };
        }
        /// <summary>
        /// finds the closest 2 nodes of node sets and makes a path between them.
        /// </summary>
        /// <param name="nodeSet1"></param>
        /// <param name="nodeSet2"></param>
        public void CreatePathing(Room room1, Room room2)
        {
            Node[] set = FindClosest(room1.roomNodes, room2.roomNodes, false); //find the nodes closest to eachother from each room

            room1.roomNodes[Array.IndexOf(room1.roomNodes, set[0])].Enabled = false;//disables node's ability to be found in FindClosest()
            room2.roomNodes[Array.IndexOf(room2.roomNodes, set[1])].Enabled = false;

            IntVector start = set[0].Coordinates;
            IntVector end =   set[1].Coordinates;
            DrawWalls(start, end);
            DrawFloors(start, end);
            //Console.WriteLine("{0} connects to {1}", set);
            for (int x = 0; x <= (int)Room.Nodes.CornerIndexes; x++) //0-3 are indexes for room Corner nodes (so this is if in corner do:)
            {
                if (set[0] == room1.roomNodes[x])
                {
                    Node[] set2 = FindClosest(room1.roomNodes, new Node[] { set[0] }, true);
                    room1.roomNodes[Array.IndexOf(room1.roomNodes, set2[1])].Enabled = false; 

                    start = set2[1].Coordinates;
                    end = set2[0].Coordinates;

                    DrawWalls(start, end);
                    DrawFloors(start, end);
                }
                if (set[1] == room2.roomNodes[x])
                {
                    Node[] set2 = FindClosest(room2.roomNodes, new Node[] { set[1] }, true);
                    room2.roomNodes[Array.IndexOf(room2.roomNodes, set2[1])].Enabled = false;

                    start = set2[1].Coordinates;
                    end = set2[0].Coordinates;

                    DrawWalls(start, end);
                    DrawFloors(start, end);
                }
            }
            room1.roomNodes[Array.IndexOf(room1.roomNodes, set[0])].Enabled = true;
            room2.roomNodes[Array.IndexOf(room2.roomNodes, set[1])].Enabled = true;
        }
        /// <summary>
        /// makes a 3 wide line of cubes between 2 points on the map
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void DrawWalls(IntVector start, IntVector end)
        {

            IntVector walker = start;

            while ((walker != end))
            {
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        if (Level.FindStr(walker + (x, y)).IndexOfAny("D#_".ToCharArray()) == -1)
                        {
                           Level.Write(Types.Wall(walker + (x, y)));
                        }

                    }
                }
                walker += (IntVector)Pathfinding.NextStep(walker, end);
            }
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (Level.FindStr(walker + (x, y)).IndexOfAny("D#_".ToCharArray()) == -1)
                    {
                        Level.Write(Types.Wall(walker + (x, y)));
                    }

                }
            }

        }
        /// <summary>
        /// makes a pathway of floor tiles. deletes walls if met
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void DrawFloors(IntVector start, IntVector end)
        {

            IntVector walker = start;

            while ((walker != end))
            {
                Level.RemoveAt(walker, 1);
                Level.Write(Types.Floor(walker));
                walker += (IntVector)Pathfinding.NextStep(walker, end);
            }

            Level.RemoveAt(walker, 1);
            Level.Write(Types.Floor(walker));
        }
        public void CreateBox(IntVector size, IntVector start, int id)
        {
            CreateBox(size.X, size.Y, start.X, start.Y, id);
        }
        /// <summary>
        /// creates a box with upper left corner of x, y
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="id"></param>
        public void CreateBox(int width, int height, int x, int y, int id)
        {
            //saves the rooms bottom left and top right corners
            Rooms.Add(new Room((x,y),(width,height),id));

            //fills the rooms corners with walls
            Level.Write(Types.Wall((x + width,  y + height)));
            Level.Write(Types.Wall((x + width,  y)));
            Level.Write(Types.Wall((x,          y + height)));
            Level.Write(Types.Wall((x,          y)));

            for (int X = 0; X <= width; X++)
            {
                for (int Y = 0; Y <= height; Y++)
                {
                    //checks if coordinate is on the edge of the room
                    if (X <= 0 || Y <= 0 || X >= width || Y >= height)
                    {
                        if (X == (width / 2) || Y == (height / 2))
                        {
                            //writes a door if spot is in the central axis of the room
                            Entity door = Types.Door((X + x, Y + y));
                            Level.Write(door);
                            //writes a loor under the door
                            Entity floor = Types.Floor((X + x, Y + y));
                            Level.Write(floor);
                        }
                        else
                        {
                            //makes a square around th perimeter of a rectangle
                            Entity wall = Types.Wall((X + x, Y + y));
                            Level.Write(wall);
                        }
                    }
                    else
                    {
                        //sets position to floor in cases where it's inside x,y - x,y+size
                        Level.Write(Types.Floor((X + x, Y + y)));
                    }
                }   
            }
        }
    }
    /// <summary>
    /// data type for storing information about room for ease of use
    /// </summary>
    class Room : IComparable<Room>
    {
        public Node[] roomNodes = new Node[] { };
        public IntVector start;
        public IntVector end;
        public bool enabled;
        public int Id;
        public enum Nodes
        {
            BottomLeftCorner,
            BottomRightCorner,
            TopRightCorner,
            TopLeftCorner,

            BottomCenter,
            TopCenter,
            RightCenter,
            LeftCenter,
            CornerIndexes = 3,

        };

        public Room(Vector startCorner, Vector roomSize, int id)
        {
            roomNodes = new Node[]
            {
                //*
                new Node((startCorner.X - 1,                startCorner.Y - 1               ),id){Enabled = true},
                new Node((startCorner.X + roomSize.X + 1,   startCorner.Y - 1               ),id){Enabled = true},
                new Node((startCorner.X + roomSize.X + 1,   startCorner.Y + roomSize.Y + 1  ),id){Enabled = true},
                new Node((startCorner.X - 1,                startCorner.Y + roomSize.Y + 1  ),id){Enabled = true},

                new Node((startCorner.X + roomSize.X / 2,   startCorner.Y - 1               ),id){Enabled = true},
                new Node((startCorner.X + roomSize.X / 2,   startCorner.Y + roomSize.Y + 1  ),id){Enabled = true},
                new Node((startCorner.X - 1,                startCorner.Y + roomSize.Y / 2  ),id){Enabled = true},
                new Node((startCorner.X + roomSize.X + 1,   startCorner.Y + roomSize.Y / 2  ),id){Enabled = true}
            };
            start   = startCorner;
            end     = startCorner + roomSize;
            Id = id;
            enabled = true;
        }
        public int CompareTo(Room other)
        {
            return this.start.CompareTo(other.start);
        }
    }
}
