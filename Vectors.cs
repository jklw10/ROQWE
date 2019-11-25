using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


using OpenTK;

namespace ROQWE
{

#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
    struct Vector : IComparable<Vector>
#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
    {
        public double X {get; set;}
        public double Y {get; set;}

        public Vector THIS { get { return this; } set { X = value.X; Y = value.Y; } }
        public double Magnitude 
        {
            get {return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));}
            private set { }
            
        }

        public double Angle
        {
            get {return VectorToRadians(this); }
            set{ THIS = RadiansToVector(value, Magnitude); }
        }

        /// <summary>
        /// (X, Y)
        /// </summary>
        /// <param name="coordinate"></param>
        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }

        // <summary>
        /// (X, Y)
        /// </summary>
        /// <param name="coordinate"></param>
        public Vector(ref double x, ref double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// creates a vector of (size, size)
        /// </summary>
        /// <param name="size"></param>
        public Vector(double size)
        {
            X = size;
            Y = size;
        }
        public override string ToString()
        {
            return "(" + X + "," + Y + ")";
        }

        /// <summary>
        /// rotate vector with radians
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Vector RotateVector(Vector vector, double angle)
        {
            return RadiansToVector((vector.Angle + angle), vector.Magnitude);
        }



        /// <summary>
        /// rotate vector with to an angle. 0 is on the Left.
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Vector SetRotation(Vector vector, double angle)
        {
           
            return RadiansToVector(angle, vector.Magnitude);
        }
        // your royal rectal region is kerosene.

       

        /// <summary>
        /// Set Magnitude to 1
        /// </summary>
        /// <returns></returns>
        public Vector Normalize()
        {
            return RadiansToVector(Angle , 1);
        }

        /// <summary>
        /// See how similar 2 vectors are in angle.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double CrossProduct(Vector a, Vector b)
        {
            Vector A = a.Normalize();
            Vector B = b.Normalize();
            return (A.X * B.X + A.Y * B.Y);
        }

        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.X + b.X, a.Y + b.Y);
        }

        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.X - b.X, a.Y - b.Y);
        }

        public static Vector operator *(Vector a, double b)
        {
            return new Vector(a.X *b, a.Y *b);
        }

        public static Vector operator *(Vector a, Vector b)
        {
            return new Vector( a.X*b.X , a.Y*b.Y );
        }

        public static Vector operator /(Vector a, double b)
        {
            return new Vector(a.X / b, a.Y / b);
        }

        public static Vector operator /(Vector a, Vector b)
        {
            return new Vector(a.X / b.X, a.Y / b.Y);
        }
        public static Vector operator %(Vector a, Vector b)
        {
            return a- Floor(a/b) * b ;
        }

        public static Vector Floor(Vector a)
        {
            return new Vector(Math.Floor(a.X), Math.Floor(a.Y));
        }

        public int CompareTo(Vector other)
        {
            return Magnitude.CompareTo(other.Magnitude);
        }
        public static bool operator >(Vector operand1, Vector operand2)
        {
            return operand1.CompareTo(operand2) == 1;
        }

        // Define the is less than operator.
        public static bool operator <(Vector operand1, Vector operand2)
        {
            return operand1.CompareTo(operand2) == -1;
        }

        // Define the is greater than or equal to operator.
        public static bool operator >=(Vector operand1, Vector operand2)
        {
            return operand1.CompareTo(operand2) >= 0;
        }

        // Define the is less than or equal to operator.
        public static bool operator <=(Vector operand1, Vector operand2)
        {
            return operand1.CompareTo(operand2) <= 0;
        }
        public static bool operator ==(Vector operand1, Vector operand2)
        {
            return (operand1.X - operand2.X == 0 && operand1.Y - operand2.Y == 0);
        }
        public static bool operator !=(Vector operand1, Vector operand2)
        {
            return !(operand1.X - operand2.X == 0 && operand1.Y - operand2.Y == 0);
        }


        /// <summary>
        /// returns true if angle in radians is between max and min angles
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        /// <returns></returns>
        public static bool IsBetween(double angle, double max, double min)
        {
            double difference = Math.Abs(min - max);
            return ((Math.Abs(angle- min) <= difference && Math.Abs(angle - max) <= difference));
        }

        /// <summary>
        /// returns true if vectorTarget is between high and low
        /// </summary>
        /// <param name="vectorTarget"></param>
        /// <param name="vectorHigh">higher</param>
        /// <param name="vectorLow">lower</param>
        /// <returns></returns>
        public static bool IsBetween(Vector vectorTarget, Vector vectorHigh, Vector vectorLow)
        {
            double Accuracy = CrossProduct(vectorHigh, vectorLow);
            Vector Median = (vectorHigh+vectorLow)/2;// = ThetaOf(vectorLow, vectorHigh);
            return ((1-CrossProduct(vectorTarget, Median) <= (1-Accuracy)/2 && 1- CrossProduct(vectorTarget, Median) <= (1 - Accuracy )/ 2));
            
        }

        /*public static double middleOf(double a,double b)
        {
            Vector A = RadiansToVector(a, 1);
            Vector B = RadiansToVector(b, 1);
            double middle = VectorToRadians((A+B)/2);
            return null;
        }//*/

        /// <summary>
        /// swaps the places of X and Y. Mostly because OpenTK
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector Swap()
        {
            return new Vector(Y, X);
        }

        // Frankenstein was here
        /*public static implicit operator Angle(Vector v)
        {
            return new Angle(v.Angle);
        }*/

        public static explicit operator Vector (GameWindow w)
        {
            return new Vector( w.Size.Width,w.Size.Width );
        }
        public static explicit operator Vector(Point w)
        {
            return new Vector(w.X, w.Y); 
        }
        public static implicit operator double[] (Vector v)
        {
            return new double[]{v.X, v.Y};
        }
        public static implicit operator Vector3 (Vector v)
        {
            return new Vector3((float)v.X, (float)v.Y, 0);
        }
        public static explicit operator Vector2(Vector v)
        {
            return new Vector2((float)v.X, (float)v.Y);
        }
        public static implicit operator Vector(Vector2 v)
        {
            return new Vector(v.X, v.Y);
        }
        public static implicit operator object[,](Vector v)
        {
            return new object[(int)v.X, (int)v.Y];
        }
        public static implicit operator (double, double)(Vector v)
        {
            return (v.X, v.Y);
        }
        public static implicit operator Vector(IntVector v)
        {
            return new Vector(v.X,v.Y);
        }
        public static implicit operator Vector((double x, double y) position)
        {
            return new Vector(position.x, position.y);
        }

        /*public static explicit operator Vector(Tuple<double, double> v)
        {
            return (v.X,v.Y);
        }//*/

        /// <summary>
        /// returns angle difference of 2 vectors
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double ThetaOf(Vector a, Vector b)
        {
            return (VectorToRadians(a) - VectorToRadians(b)); 
        }
        
        /// <summary>
        /// Converts from X and Y to degrees;
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double VectorToDegrees(double x, double y)
        {
            return 180 * (Math.Atan2(y, x) / Math.PI);
        }

        /// <summary>
        /// Converts from X and Y to degrees;
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public static double VectorToDegrees(Vector coordinate)
        {
            return 180 * (Math.Atan2(coordinate.Y, coordinate.X) / Math.PI);
        }

        /// <summary>
        /// converts from X and Y to radians;
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static double VectorToRadians(double x, double y)
        {
            return Math.Atan2(y, x);
        }

        /// <summary>
        /// converts from a Vector to radians;
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static double VectorToRadians(Vector v)
        {
            return  Math.Atan2(v.Y, v.X);
        }

        /// <summary>
        /// make a vector from angle and magnitude
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="magnitude"></param>
        /// <returns></returns>
        public static Vector RadiansToVector(double angle, double magnitude)
        {
            return new Vector(Math.Cos(angle),Math.Sin(angle))*magnitude;
        }
        
        public static double RadiansToDegrees(double radians)
        {
            return 180 * (radians / Math.PI);
        }

        /// <summary>
        /// Pieces the given angle into "segmenth" sized parts
        /// </summary>
        /// <param name="segment"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static double Segment(double segment, double angle)
        {
            return Math.Round(angle / segment) * segment;
        }

        

        
    }
    //
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
    struct IntVector
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
    {
        public int X { get; set; }
        public int Y { get; set; }
        public IntVector THIS { get { return this; } set { X = value.X; Y = value.Y; } }

        public double Magnitude
        {
            get { return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2)); }
            private set { }
        }

        public double Angle
        {
            get { return IntVectorToRadians(this); }
            set { }
        }
        /// <summary>
        /// (X, Y)
        /// </summary>
        /// <param name="coordinate"></param>
        public IntVector(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// creates a vector of (size, size)
        /// </summary>
        /// <param name="size"></param>
        public IntVector(int size)
        {
            X = size;
            Y = size;
        }
        public int CompareTo(IntVector other)
        {
            return Magnitude.CompareTo(other.Magnitude); 
        }
        public static bool operator >(IntVector operand1, IntVector operand2)
        {
            return operand1.CompareTo(operand2) == 1;
        }

        // Define the is less than operator.
        public static bool operator <(IntVector operand1, IntVector operand2)
        {
            return operand1.CompareTo(operand2) == -1;
        }

        // Define the is greater than or equal to operator.
        public static bool operator >=(IntVector operand1, IntVector operand2)
        {
            return operand1.CompareTo(operand2) >= 0;
        }

        // Define the is less than or equal to operator.
        public static bool operator <=(IntVector operand1, IntVector operand2)
        {
            return operand1.CompareTo(operand2) <= 0;
        }
        public static bool operator ==(IntVector operand1, IntVector operand2)
        {
            return (operand1.X - operand2.X == 0 && operand1.Y - operand2.Y == 0);
        }
        public static bool operator !=(IntVector operand1, IntVector operand2)
        {
            return !(operand1.X - operand2.X == 0 && operand1.Y - operand2.Y == 0);
        }

        public static IntVector operator +(IntVector a, IntVector b)
        {
            return new IntVector(a.X + b.X, a.Y + b.Y);
        }

        public static IntVector operator -(IntVector a, IntVector b)
        {
            return new IntVector(a.X - b.X, a.Y - b.Y);
        }

        public static IntVector operator *(IntVector a, double b)
        {
            return new IntVector((int)(a.X * b), (int)(a.Y * b));
        }

        public static IntVector operator *(IntVector a, IntVector b)
        {
            return new IntVector((int)(a.X * b.X), (int)(a.Y * b.Y));
        }

        public static IntVector operator /(IntVector a, double b)
        {
            return new IntVector((int)(Math.Round(a.X / b)), (int)(Math.Round(a.Y / b)));
        }

        public static IntVector operator /(IntVector a, IntVector b)
        {
            return new IntVector((int)(Math.Round((double)(a.X / b.X))), (int)(Math.Round((double)(a.Y / b.Y))));
        }

        public static IntVector operator %(IntVector a, IntVector b)
        {
            return ((Vector)a % (Vector)b);
        }

        /// <summary>
        /// converts from a Vector to radians;
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static double IntVectorToRadians(IntVector v)
        {
            return Math.Atan2(v.Y, v.X);
        }

        public static implicit operator (int, int) (IntVector v)
        {
            return (v.X, v.Y);
        }


        public static implicit operator IntVector((int x, int y) position)
        {
            return new IntVector(position.x, position.y);
        }//*/
        
        public static implicit operator IntVector(Vector v)
        {
            return new IntVector((int)Math.Round(v.X), (int)(Math.Round(v.Y)));
        }

    }//*/
    
}
