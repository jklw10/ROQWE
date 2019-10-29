using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;


namespace ROQWE
{
    class Visuals
    {
        public static int Create(params int[] Shaders)
        {                                               
            int ID = GL.CreateProgram();                
            foreach (var Unit in Shaders)               
            {                                           
                GL.AttachShader(ID, Unit);              
                                                        
                                                        
            }                                           
            GL.LinkProgram(ID);                         
                                                        
            return ID;                                  

        }
    }
}
