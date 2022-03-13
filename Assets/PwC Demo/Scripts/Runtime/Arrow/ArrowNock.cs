using UnityEngine;

namespace PWCDemo
{
    /// <summary>
    /// Convenience class used to handle tracking which <see cref="Arrow"/> is associated
    /// with this Nock
    /// </summary>
    public class ArrowNock : MonoBehaviour
    {
        /// <summary>
        /// The <see cref="Arrow"/> associated with this Nock
        /// </summary>
        [field: SerializeField, Tooltip("The arrow associated with this nock")]
        public Arrow CachedArrow { get; set; } = null;
    }
}
