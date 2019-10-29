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
        public List<Node[]> Rooms = new List<Node[]> { };
        readonly Map Level;
        public List<Node[]> Nodelist = new List<Node[]> { };
        public List<List<Node[]>> connected = new List<List<Node[]>> { };
        
        public Generator(Map level)
        {
            Level = level;
        }
        public static Node[] Test()
        {
            return FindClosest(new Node[] {new Node(10,10,0), new Node(20, 20, 0) {Enabled=false } }, new Node[] { new Node(30, 20, 0) { Enabled = false}, new Node(40, 30, 0) }, false);
            
        }
        
        public void Generate()
        {
            //generates rooms
            for (int i = 0; i < 5; i++)
            {
                IntVector size = (10, 10);
                Random rng = new Random();
                IntVector start = (rng.Next(0, 200), rng.Next(0, 200));
                bool possible = true;
                //checks if room overlaps with other rooms
                foreach (Node[] room in Rooms)
                {
                    if (!(start > (IntVector)room[1].Coordinates || start + size < (IntVector)room[0].Coordinates))
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
                }
                else
                {
                    i--;
                }
            }
            //adds all rooms' bottom left corners to a list
            Node[] rooms = new Node[Rooms.Count];
            for (int y = 0; y < Rooms.Count; y++)
            {
                rooms[y] = Rooms.ToArray()[y][0];
                rooms[y].Enabled = true;
            }
            //generates pathing between rooms
            for (int i = 0; i < Rooms.Count; i++)
            {
                Node[] thisRoom = new Node[] { Rooms[i][0] };

                thisRoom[0].Enabled = false;
                Node[] roomSet = FindClosest(rooms, thisRoom, true); 
                thisRoom[0].Enabled = true;

                Node[] room1Nodes = new Node[8];
                Node[] room2Nodes = new Node[8];

                //takes the rooms bottom left corner and tries to find a rooms nodes based on id's
                room1Nodes = Nodelist.Find(x => x[0].Id == roomSet[0].Id);
                room2Nodes = Nodelist.Find(x => x[0].Id == roomSet[1].Id);

                bool found = false; 
                List<int> ids = new List<int>(); //List for room id's to check if an id has already been found earlier'

                //tests if connections to rooms made earlier exist
                for (int x = 0; x < connected.Count; x++)
                {
                    for (int y = 0; y < connected[x].Count; y++)
                    {
                        if (connected[x][y][0].Id == room1Nodes[0].Id && !ids.Contains(room1Nodes[0].Id))
                        {
                            ids.Add(room1Nodes[0].Id);
                            connected[x].Add(room2Nodes);
                            found =true;
                        }
                        else if (connected[x][y][0].Id == room2Nodes[0].Id && !ids.Contains(room2Nodes[0].Id))
                        {
                            ids.Add(room2Nodes[0].Id);
                            connected[x].Add(room2Nodes);
                            found = true;
                        }
                    }
                }
                if (!found) // if no connections between one of the rooms and other rooms is found then make a new connection list
                {
                    ids.Add(room1Nodes[0].Id);
                    ids.Add(room2Nodes[0].Id);
                    connected.Add( new List<Node[]> { room1Nodes, room2Nodes });
                }
                CreatePathing(room1Nodes,room2Nodes);
            }
            //connects all unconnected rooms
            for (int x = 0; x < connected.Count-1; x++)
            {
                List<Node> candidate1 = new List<Node>();
                List<Node> candidate2 = new List<Node>();
                //adds all nodes of rooms to a big list that contains all connected rooms' nodes.
                for (int y = 0; y < connected[x].Count; y++)
                {
                    candidate1.AddRange(connected[x][y].ToList());
                }
                for (int y = 0; y < connected[x+1].Count; y++)
                {
                    candidate2.AddRange(connected[x+1][y].ToList());
                }
                CreatePathing(candidate1.ToArray(),candidate2.ToArray());
            }
        }
        /// <summary>
        /// finds the closest points between nodeset1 and 2
        /// returns from respective sets to places [0] and [1]
        /// doesn't check if nodelist 2 items are false if ignoreSecond is true;
        /// </summary>
        /// <param name="nodeSet1"></param>
        /// <param name="nodeSet2"></param>
        /// <returns></returns>
        public static Node[] FindClosest(Node[] nodeSet1, Node[] nodeSet2, bool ignoreSecond)
        {
            List<Node> nodes = new List<Node>();

            for (int y = 0; y < nodeSet2.Length; y++)
            {
                for (int x = 0; x < nodeSet1.Length; x++)
                {
                    if (nodeSet1[x].Enabled) //checks if node is usable
                    {
                        if (nodeSet2[y].Enabled || ignoreSecond) //checks if node is usable
                        {
                            nodes.Add(new Node(nodeSet1[x].Coordinates - nodeSet2[y].Coordinates, x + y * nodeSet1.Length)); //takes the distance between 2 nodes adds it to a list with identifier based on node locations in sets
                        }
                    }
                }
            }
            nodes.Sort();//sorts distances to eachother from the shortest to longest 
            Node start = nodeSet1[nodes[0].Id % nodeSet1.Length]; //selects node from the list that had the shortest distance to nodeset2
            Node end = nodeSet2[nodes[0].Id / nodeSet1.Length]; //same for for this set
            return new Node[] { start, end };
        }
        /// <summary>
        /// finds the closest 2 nodes of node sets and makes a path between them.
        /// </summary>
        /// <param name="nodeSet1"></param>
        /// <param name="nodeSet2"></param>
        public void CreatePathing(Node[] nodeSet1, Node[] nodeSet2)
        {
            Node[] set = FindClosest(nodeSet1, nodeSet2, false);
            nodeSet1[Array.IndexOf(nodeSet1, set[0])].Enabled = false;//disables node's ability to be found in FindClosest()
            nodeSet2[Array.IndexOf(nodeSet2, set[1])].Enabled = false;
            IntVector start = set[0].Coordinates;
            IntVector end =   set[1].Coordinates;
            DrawWalls(start, end);
            DrawFloors(start, end);
            Console.WriteLine("{0} connects to {1}", set);
            for (int x = 0; x < 4; x++)
            {

                if (set[0] == nodeSet1[x])
                {
                    Node[] set2 = FindClosest(nodeSet1, new Node[] { set[0] }, true);
                    nodeSet1[Array.IndexOf(nodeSet1, set2[0])].Enabled = false; 
                    start = set2[0].Coordinates;
                    end = set2[1].Coordinates;
                    DrawWalls(start, end);
                    DrawFloors(start, end);
                    Console.WriteLine("{0} connects to {1}", set2);
                    break;
                }
                if (set[1] == nodeSet2[x])
                {
                    Node[] set2 = FindClosest(nodeSet2, new Node[] { set[1] }, true);
                    nodeSet2[Array.IndexOf(nodeSet2, set2[0])].Enabled = false;
                    start = set2[0].Coordinates;
                    end = set2[1].Coordinates;
                    DrawWalls(start, end);
                    DrawFloors(start, end);
                    Console.WriteLine("{0} connects to {1}", set2);
                    break;
                }
            }

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
            Rooms.Add(new Node[2]{new Node((x, y),id),new Node((x + width, y + height),id)});

            //fills the rooms corners with walls
            Level.Write(Types.Wall((x + width,  y + height)));
            Level.Write(Types.Wall((x + width,  y)));
            Level.Write(Types.Wall((x,          y + height)));
            Level.Write(Types.Wall((x,          y)));

            //adds path finding nodes to the rooms corners and entrances
            Nodelist.Add(new Node[] 
            {
                new Node((x - 1,         y - 1         ),id){Enabled = true },
                new Node((width + x + 1, y - 1         ),id){Enabled = true},
                new Node((width + x + 1, height + y + 1),id){Enabled = true},
                new Node((x - 1,         height + y + 1),id){Enabled = true},
                new Node((x + width / 2, y - 1         ),id){Enabled = true},
                new Node((x + width + 1, y + height / 2),id){Enabled = true},
                new Node((x + width / 2, y + height + 1),id){Enabled = true},
                new Node((x - 1,         y + height / 2),id){Enabled = true}
            }); 

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
                        }
                        else
                        {
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
}
