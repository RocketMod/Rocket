using System;

namespace Rocket.API.Math {
    /// <summary>
    ///     A four dimensional vector.
    /// </summary>
    [Serializable]
    public class RVector4 : RVector3
    {
        /// <summary>
        ///     The W coordinate. 
        /// </summary>
        public float W { get; set; }
    }
}