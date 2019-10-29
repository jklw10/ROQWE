using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OpenTK.Graphics.OpenGL;

namespace ROQWE
{
    class Shaders
    {

        public static int Load(string path, ShaderType type)
        {
            
            string Path = "Content/" + path;
            if (!File.Exists(Path))
            {
                throw new FileNotFoundException("file not found at Content/" + path);
            }
            string Text = File.ReadAllText(Path);

            int ID = GL.CreateShader(type);
            GL.ShaderSource(ID, Text);
            GL.CompileShader(ID);

            Console.WriteLine(GL.GetShaderInfoLog(ID));
            
            GL.GetShader(ID,ShaderParameter.CompileStatus,out int output);
            if (output == 0)
            {
                throw new Exception($"brok :( {GL.GetShaderInfoLog(ID)}");
            }
            return ID;

        }
    }
}
