using System;

namespace Rocket.API.Math {
    /// <summary>
    ///     A three dimensional vector.
    /// </summary>
    [Serializable]
    public class Vector2
    {
        /// <summary>
        ///     The X coordinate. 
        /// </summary>
        public float X { get; set; }

        /// <summary>
        ///     The Y coordinate. 
        /// </summary>
        public float Y { get; set; }
    }
}