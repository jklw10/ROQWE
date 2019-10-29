using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using OpenTK;

namespace ROQWE
{
    class Program
    {
        static void Main(string[] args)
        {
            
            
            Game window = new Game(640, 480);
            window.Run();
            window.Dispose();

        }
    }

}
