using System;

namespace Rocket.API.Math
{
    /// <summary>
    ///     A three dimensional vector.
    /// </summary>
    [Serializable]
    public class Vector2
    {
        ///
        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        ///
        public Vector2()
        {
            X = 0;
            Y = 0;
        }

        /// <summary>
        ///     The X coordinate.
        /// </summary>
        public float X { get; set; }

        /// <summary>
        ///     The Y coordinate.
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        ///     The zero 2d vector (0, 0).
        /// </summary>
        public Vector2 Zero => new Vector2();

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2 operator *(Vector2 a, float b)
        {
            return new Vector2(a.X * b, a.Y * b);
        }

        public static Vector2 operator /(Vector2 a, float b)
        {
            return new Vector2(a.X / b, a.Y / b);
        }
    }
}