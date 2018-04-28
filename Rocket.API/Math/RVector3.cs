namespace Rocket.API.Math
{
    //todo: add extension methods to Rocket.UnityEngine so we can easily convert this to UnityEngine vectors

    /// <summary>
    ///     A three dimensional vector.
    /// </summary>
    public struct RVector3
    {
        /// <summary>
        ///     The X coordinate. 
        /// </summary>
        public float X { get; set; }
        
        /// <summary>
        ///     The Y coordinate. 
        /// </summary>
        public float Y { get; set; }
        
        /// <summary>
        ///     The Z coordinate. 
        /// </summary>
        public float Z { get; set; }
    }
}