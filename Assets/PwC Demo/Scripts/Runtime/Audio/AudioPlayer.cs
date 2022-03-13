using UnityEngine;
using PWCDemo.Pooling;

namespace PWCDemo.Audio
{
    public class AudioPlayer : MonoBehaviour
    {
        [SerializeField]
        private AudioSource _source;
        [SerializeField]
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
