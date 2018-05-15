using System;

namespace Rocket.API.Math
{
    //todo: add extension methods to Rocket.UnityEngine so we can easily convert this to UnityEngine vectors

    /// <summary>
    ///     A three dimensional vector.
    /// </summary>
    [Serializable]
    public class Vector3 : Vector2
    {
        ///
        public Vector3(float x, float y, float z) : base(x, y)
        {
            Z = z;
        }

        ///
        public Vector3(float x, float y) : base(x, y)
        {
            Z = 0f;
        }

        ///
        public Vector3() : base()
        {
            Z = 0f;
        }

        ///
        public Vector3(Vector2 vector)
        {
            X = vector.X;
            Y = vector.Y;
            Z = 0f;
        }


        /// <summary>
        ///     The Z coordinate.
        /// </summary>
        public float Z { get; set; }

        /// <summary>
        ///     The zero 3d vector (0, 0, 0).
        /// </summary>
        public new Vector3 Zero => new Vector3();

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Vector3 operator *(Vector3 a, float b)
        {
            return new Vector3(a.X * b, a.Y * b, a.Z * b);
        }

        public static Vector3 operator /(Vector3 a, float b)
        {
            return new Vector3(a.X / b, a.Y / b, a.Z / b);
        }
    }
}