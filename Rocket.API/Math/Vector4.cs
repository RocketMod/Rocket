using System;

namespace Rocket.API.Math
{
    /// <summary>
    ///     A four dimensional vector.
    /// </summary>
    [Serializable]
    public class Vector4 : Vector3
    {
        ///
        public Vector4(float x, float y, float z, float w) : base(x, y, z)
        {
            W = w;
        }

        ///
        public Vector4(float x, float y, float z) : base(x, y, z)
        {
            W = 0f;
        }

        ///
        public Vector4(float x, float y) : base(x, y)
        {
            W = 0f;
        }

        ///
        public Vector4() : base()
        {
            W = 0f;
        }

        ///
        public Vector4(Vector3 vector)
        {
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
            W = 0f;
        }

        ///
        public Vector4(Vector2 vector) : base(vector)
        {
            W = 0f;
        }


        /// <summary>
        ///     The W coordinate.
        /// </summary>
        public float W { get; set; }

        /// <summary>
        ///     The zero 4d vector (0, 0, 0, 0).
        /// </summary>
        public new static Vector4 Zero => new Vector4();

        public static Vector4 operator +(Vector4 a, Vector4 b)
        {
            return new Vector4(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
        }

        public static Vector4 operator -(Vector4 a, Vector4 b)
        {
            return new Vector4(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
        }

        public static Vector4 operator *(Vector4 a, float b)
        {
            return new Vector4(a.X * b, a.Y * b, a.Z * b, a.W * b);
        }

        public static Vector4 operator /(Vector4 a, float b)
        {
            return new Vector4(a.X / b, a.Y / b, a.Z / b, a.W / b);
        }
    }
}