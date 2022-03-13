using UnityEngine;
using PWCDemo.Pooling;

namespace PWCDemo.Audio
{
    /// <summary>
    /// Convenience class which manages audio playback of individual sound effects
    /// </summary>
    public class AudioPlayer : MonoBehaviour
    {
        /// <summary>
        /// The <see cref="AudioSource"/> from which sound effects should be played
        /// </summary>
        [SerializeField, Tooltip("The audio source from which sound effects should be played")]
        private AudioSource _source;
        /// <summary>
        /// Configuration data managing how sound effects will be played
        /// </summary>
        [SerializeField, Tooltip("Configuration data managing how sound effects will be played")]
        private AudioData _data;

        private float _defaultPitch;

        /// <summary>
        /// Handles playing the given <see cref="AudioClip"/>. If randomPitch is true,
        /// it will also randomize the playback pitch
        /// </summary>
        /// <param name="clip">The <see cref="AudioClip"/> to play</param>
        /// <param name="randomPitch">Whether or not playback pitch should be randomized</param>
        public void Play(AudioClip clip, bool randomPitch = true)
        {
            if (randomPitch)
                _source.pitch = _data.RandomPitch;
            else
                _source.pitch = _defaultPitch;

            _source.PlayOneShot(clip);
            Invoke("Cleanup", clip.length);
        }

        /// <summary>
        /// Handles cleaning up this Audioplayer
        /// </summary>
        public void Cleanup()
        {
            PoolManager.Recycle(gameObject);
        }

        private void Start()
        {
            _defaultPitch = _source.pitch;
        }
    }
}
