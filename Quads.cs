using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Text;
//using VectorLib;
namespace ROQWE
{
    class Cube
    {
        private float X;
        private float Y;
        private float Z;
        private Vector3 Position
        {
            get { return new Vector3(X, Y, Z); }
            set { X = value.X; Y = value.Y; Z = value.Z; }
        }

        public double Angle { get; set; }
        private float Width;
        private float Length;
        private float Height;

        private static int VSID;
        private static int QID;
        private static int IBO;
        private static uint[] indices;

        private static float FoV = (float)Math.PI/3;
        public static float Zoom = 3;

        public static Vector Offset { get; set; }

        public static Vector ScreenSize = new Vector(Game.window.Width, Game.window.Height);

        private Color Color;
        private int Texture;
        private static int _default = Loader.LoadColor(Color.Black);
        public float highlight;

        public Cube(float x, float y, float z, float width, float length, float height, Color color)
        {
            X = x;
            Y = y;
            Z = z;
            Width = width;
            Length = length;
            Height = height;
            Texture = _default;
            Color = color;
            Angle = 0;
            highlight = 0;
        }
        public Cube()
        {
            X = 0;
            Y = 0;
            Z = 0;
            Width = 0;
            Length = 0;
            Height = 0;
            Texture = _default;
            Color = Color.Black;
            Angle = 0;
            highlight = 0;
        }
        public Cube(Vector3 position, float width, float length, float height, Color color)
        {
            X = position.X;
            Y = position.Y;
            Z = position.Z;
            Width = width;
            Length = length;
            Height = height;
            Texture = _default;
            Color = color;
            Angle = 0;
            highlight = 0;
        }
        public Cube(Vector3 position, float width, float length, float height, int texture)
        {
            X = position.X;
            Y = position.Y;
            Z = position.Z;
            Width = width;
            Length = length;
            Height = height;
            Texture = texture;
            Color = Color.Black;
            Angle = 0;
            highlight = 0;
        }
        
        public Cube(float x, float y, float z, float width, float length, float height, int texture)
        {
            X = x;
            Y = y;
            Z = z;
            Width = width;
            Length = length;
            Height = height;
            Texture = texture;
            Color = Color.Black;
            Angle = 0;
            highlight = 0;
        }
        public override string ToString()
        {
            return Color.ToString() + " cube with dimensions: (" + Width + ", " + Length + ", " + Height + ")";
        }
        public void MoveQuad(float x, float y)
        {
            X += x;
            Y += y;
        }
        public void MoveQuad(Vector v)
        {
            Position += v;
        }
        
        public static void SetFov(float fov)
        {
            if(fov >= 0.001 && fov <= Math.PI-0.01)
            {
                FoV = fov;
            }
        }

        public static void AddFOV(float fov)
        {
            if (FoV + fov >= 0.001 && FoV + fov <= Math.PI-0.01)
            {
                FoV += fov;
            }
        }


        /// <summary>
        /// rotates quad by Angle in Radians.
        /// </summary>
        /// <param name="angle"></param>
        public void RotateQuad(double angle)
        {
            Angle += angle;
        }
        /// <summary>
        /// angle in radians
        /// </summary>
        /// <param name="angle"></param>
        public void SetAngle(double angle)
        {
            Angle = angle;
        }
        public double GetAngle()
        {
            return Angle;
        }

        public void SetColor(Color color)
        {
            Color = color;
        }
        public Color GetColor()
        {
            return Color;
        }

        /// <summary>
        /// quad width in screen pixels
        /// </summary>
        /// <returns></returns>
        public double GetWidth()
        {
            return Width;
        }


        /// <summary>
        /// Returns specified quads position on the screen relative to top left corne
        /// r
        /// </summary>
        public Vector QuadPos()
        {
            return (Vector)Position.Xy + Offset;
        }

        /// <summary>
        /// Returns specified quads position on the screen relative to center of screen
        /// </summary>
        public Vector CenterPos()
        {
            return (Vector)Position.Xy + Offset + ScreenSize / 2;
        }
        /// <summary>
        /// highlights the selected cube by width
        /// </summary>
        public void Highlight(float width)
        {
            highlight = width;
        }

        /// <summary>
        /// draws cubes on screen without perspective.
        /// sides vector: x=left,y=right,z=bottom,w=top;
        /// </summary>
        /// <param name="cameraAngle"></param>
        /// <param name="sides">x=left,y=right,z=bottom,w=top</param>
        public void DrawOnScreen(Vector3 cameraAngle, Vector3 cameraLookAt,Vector4 sides)
        {

            GL.UseProgram(VSID);
            Vector3 ObjectPosition = new Vector3((new Vector(1) * (Position.Xy)))
            {
                Z = Position.Z 
            };

            Matrix4 Scale = Matrix4.CreateScale(Width, Length, Height);
            Matrix4 Translation = Matrix4.CreateTranslation(ObjectPosition);
            Matrix4 Rotation = Matrix4.CreateRotationZ((float)Angle);

            Matrix4 CamRotX = Matrix4.CreateRotationX(cameraAngle.X);
            Matrix4 CamRotY = Matrix4.CreateRotationY(cameraAngle.Y);
            Matrix4 CamRotZ = Matrix4.CreateRotationZ(cameraAngle.Z);
            Matrix4 CamTrans = Matrix4.CreateTranslation(new Vector3(1,0,0));

            Vector3 CamPos = (CamTrans * CamRotX * CamRotY * CamRotZ).ExtractTranslation();

            Matrix4 ModelMatrix = Scale * Rotation * Translation;

            Matrix4 ViewMatrix = Matrix4.LookAt(cameraLookAt + CamPos, cameraLookAt, Vector3.UnitZ);

            //everything works from here up
            Matrix4 ProjectionMatrix = Matrix4.CreateOrthographicOffCenter(sides.Y, sides.X, sides.W, sides.Z, 0.01f,100);

            Matrix4 Combined = ModelMatrix * ViewMatrix * ProjectionMatrix;

            GL.ProgramUniformMatrix4(VSID, GL.GetUniformLocation(VSID, "QuadMatrix"), false, ref Combined);
            GL.ProgramUniform4(VSID, GL.GetUniformLocation(VSID, "ColorIn"), Color);
            GL.ProgramUniform1(VSID, GL.GetUniformLocation(VSID, "SS"), 0);
            Vector2 resolution = sides.Zw-sides.Xy; //gets the window size by taking the bottom left corner from top right one
            GL.ProgramUniform2(VSID, GL.GetUniformLocation(VSID, "Resolution"), ref resolution);
            Vector2 pos = sides.Xy;
            GL.ProgramUniform2(VSID, GL.GetUniformLocation(VSID, "ScreenPos"), ref pos);

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.IndexArray);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.Fog);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Texture);
            GL.BindVertexArray(QID);

            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
        }
        /// <summary>
        /// Draws the specified Cube in world.
        /// </summary>
        public void DrawInWorld(Vector3 cameraAngle, Vector3 cameraLookAt)
        {
            
            GL.UseProgram(VSID);
            Vector3 ObjectPosition = new Vector3((new Vector2(1) * (Position.Xy)))
            {
                Z = Position.Z - (Height*0.5f)
            };

            Matrix4 Scale            = Matrix4.CreateScale(Width, Length,Height); 
            Matrix4 Translation      = Matrix4.CreateTranslation(ObjectPosition);
            Matrix4 Rotation         = Matrix4.CreateRotationZ((float)Angle);


            Matrix4 CamRotZ          = Matrix4.CreateRotationZ(cameraAngle.Z);
            Matrix4 CamRotX          = Matrix4.CreateRotationX(cameraAngle.X);
            Matrix4 CamRotY          = Matrix4.CreateRotationY(cameraAngle.Y);
            Matrix4 CamTrans         = Matrix4.CreateTranslation(new Vector3(Zoom,0,0));

            Vector3 CamPos           = (CamTrans * CamRotX * CamRotY  * CamRotZ).ExtractTranslation();

            Matrix4 ModelMatrix      = Scale * Rotation * Translation;

            Matrix4 ViewMatrix       = Matrix4.LookAt(cameraLookAt + CamPos, cameraLookAt, Vector3.UnitZ);

            Matrix4 ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI-FoV, (float)(ScreenSize.X/ScreenSize.Y),0.1f,1000);
            
            Matrix4 Combined         = ModelMatrix * ViewMatrix * ProjectionMatrix;

            GL.ProgramUniformMatrix4(VSID, GL.GetUniformLocation(VSID, "QuadMatrix"), false, ref Combined);
            GL.ProgramUniform4(VSID, GL.GetUniformLocation(VSID, "ColorIn"), Color);
            GL.ProgramUniform1(VSID, GL.GetUniformLocation(VSID, "SS"), 0);
            Vector2 resolution = (Vector2)(Vector)Game.window;
            GL.ProgramUniform2(VSID, GL.GetUniformLocation(VSID, "Resolution"), ref resolution);
            Vector2 pos = new Vector2(0,0);
            GL.ProgramUniform2(VSID, GL.GetUniformLocation(VSID, "ScreenPos"), ref pos);

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.IndexArray);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.Fog);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Texture);
            GL.BindVertexArray(QID);

            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
        }
        public static void CreateVisuals()
        {
            int VS = Shaders.Load("Shaders.VertS", ShaderType.VertexShader);
            int FS = Shaders.Load("Shaders.FragS", ShaderType.FragmentShader);

            VSID = Visuals.Create(VS, FS);
        }

        public static void CreateCube()
        {
            QID = GL.GenVertexArray();
            GL.BindVertexArray(QID);
            //magic numbers for cube vertices (xyz)
            int VID =GL.GenBuffer();
            float[] Verticles = 
            {
                   -0.5f,  0.5f,  0.5f,
                    0.5f,  0.5f,  0.5f,
                   -0.5f, -0.5f,  0.5f,
                    0.5f, -0.5f,  0.5f,
                   -0.5f, -0.5f, -0.5f,
                    0.5f, -0.5f, -0.5f,
                   -0.5f,  0.5f, -0.5f,
                    0.5f,  0.5f, -0.5f,
                   -0.5f,  0.5f,  0.5f,
                   -0.5f,  0.5f, -0.5f,
                    0.5f,  0.5f,  0.5f,
                    0.5f,  0.5f, -0.5f
            };
            GL.BindBuffer(BufferTarget.ArrayBuffer, VID);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float)*Verticles.Length, Verticles,BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);

            //magic numbers for cube UV ids (XY)
            int UVID = GL.GenBuffer();
            float[] UVs =
            {
                0,  0,
                1,  0,
                0,  1,
                1,  1,
                0,  0,
                1,  0,
                0,  1,
                1,  1,
                1,  1,
                1,  0,
                0,  1,
                0,  0,
            };
            GL.BindBuffer(BufferTarget.ArrayBuffer, UVID);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * UVs.Length, UVs, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(1);

            //magic numbers for cube vertex indices ids (XYZ)
            indices = new uint[]
            {
                0, 2, 1,
                2, 3, 1,
                8, 9, 2,
                9, 4, 2,
                2, 4, 3,
                4, 5, 3,
                3, 5,10,
                5,11,10,
                4, 6, 5,
                6, 7, 5,
                6, 0, 7,
                0, 1, 7
            };

            IBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(sizeof(uint) * indices.Length), indices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(IBO,1,VertexAttribPointerType.UnsignedInt,false,0,0);
            GL.EnableVertexAttribArray(2);

        }

    }
}
