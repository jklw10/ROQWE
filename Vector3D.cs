using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using OpenTK;

namespace ROQWE
{
    
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
    struct IntVector3D: IComparable<IntVector3D>
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
    
        public IntVector XY
        {
            get { return (X, Y); }
            set { X = value.X; Y = value.Y; }
        }
        public IntVector XZ
        {
            get { return (X, Z); }
            set { X = value.X; Z = value.Y; }
        }
        public IntVector YZ
        {
            get { return (Y, Z); }
            set { Y = value.X; Z = value.Y; }
        }
    
    
        public double Magnitude
        {
            get { return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2)); }
            private set { }
        }
    
        public Vector Angle
        {
            get { return IntVectorToRadians(this); }
            set {  }
        }

        public override string ToString()
        {
            return "(" + X + "," + Y + "," + Z + ")";
        }

        /// <summary>
        /// (X, Y)
        /// </summary>
        /// <param name="coordinate"></param>
        public IntVector3D(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    
        /// <summary>
        /// creates a vector of (size, size, size)
        /// </summary>
        /// <param name="size"></param>
        public IntVector3D(int size)
        {
            X = size;
            Y = size;
            Z = size;
        }
        public int CompareTo(IntVector3D other)
        {
           // The magnitude comparison depends on the comparison of 
           // the underlying Double values. 
           return Magnitude.CompareTo(other.Magnitude);
        }
        public static bool operator >(IntVector3D operand1, IntVector3D operand2)
        {
            return operand1.CompareTo(operand2) == 1;
        }
    
        // Define the is less than operator.
        public static bool operator <(IntVector3D operand1, IntVector3D operand2)
        {
            return operand1.CompareTo(operand2) == -1;
        }
    
        // Define the is greater than or equal to operator.
        public static bool operator >=(IntVector3D operand1, IntVector3D operand2)
        {
            return operand1.CompareTo(operand2) >= 0;
        }
    
        // Define the is less than or equal to operator.
        public static bool operator <=(IntVector3D operand1, IntVector3D operand2)
        {
            return operand1.CompareTo(operand2) <= 0;
        }
        public static bool operator ==(IntVector3D operand1, IntVector3D operand2)
        {
            return operand1.CompareTo(operand2) == 0;
        }
        public static bool operator !=(IntVector3D operand1, IntVector3D operand2)
        {
            return operand1.CompareTo(operand2) != 0;
        }
        public static IntVector3D operator +(IntVector3D a, IntVector b)
        {
            return new IntVector3D(a.X + b.X, a.Y + b.Y, a.Z);
        }

        public static IntVector3D operator +(IntVector3D a, IntVector3D b)
        {
            return new IntVector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }
    
        public static IntVector3D operator -(IntVector3D a, IntVector3D b)
        {
            return new IntVector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }
    
        public static IntVector3D operator *(IntVector3D a, double b)
        {
            return new IntVector3D((int)(a.X * b), (int)(a.Y * b), (int)(a.Z * b));
        }
    
        public static IntVector3D operator *(IntVector3D a, IntVector3D b)
        {
            return new IntVector3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }
    
        public static IntVector3D operator /(IntVector3D a, double b)
        {
            return new IntVector3D((int)Math.Round(a.X / b), (int)Math.Round(a.Y / b), (int)Math.Round(a.Z / b));
        }
    
        public static IntVector3D operator /(IntVector3D a, IntVector3D b)
        {
            return new IntVector3D(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        }
    
        public static IntVector3D operator %(IntVector3D a, IntVector3D b)
        {
            return a - Floor(a / b) * b;
        }
    
        public static IntVector3D Floor(Vector3 a)
        {
            return new IntVector3D((int)Math.Floor(a.X), (int)Math.Floor(a.Y), (int)Math.Floor(a.Z));
        }
        /// <summary>
        /// converts from a Vector to radians;
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector IntVectorToRadians(IntVector3D v)
        {
            return new Vector(Math.Atan2(v.Y,v.X), Math.Atan2(v.Z,v.XY.Magnitude));
        }

        public static explicit operator string (IntVector3D v)
        {
            return "("+v.X+", "+ v.Y+", "+v.Z+")";
        }

        public static implicit operator (int, int, int) (IntVector3D v)
        {
            return (v.X, v.Y, v.Z);
        }
    
        public static implicit operator IntVector3D((int x, int y, int z) position)
        {
            return new IntVector3D(position.x, position.y, position.z);
        }//*/
        public static implicit operator IntVector3D((IntVector v, int z) position)
        {
            return new IntVector3D(position.v.X, position.v.Y, position.z);
        }

        public static implicit operator IntVector3D(Vector3 v)
        {
            return new IntVector3D((int)Math.Round(v.X), (int)Math.Round(v.Y), (int)Math.Round(v.Z));
        }
        public static implicit operator Vector3(IntVector3D v)
        {
            return new Vector3(v.X,v.Y,v.Z);
        }
    }
    class Vector3D
    {
        public float X, Y, Z;
        public Vector3D(float x,float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public static explicit operator Vector3D((float x, float y, float z) position)
        {
            return new Vector3D(position.x, position.y, position.z);
        }//*/
        public static implicit operator Vector3(Vector3D v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }
    }
}
