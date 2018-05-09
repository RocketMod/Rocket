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
        /// <summary>
        ///     The Z coordinate.
        /// </summary>
        public float Z { get; set; }
    }
}