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
    }
}