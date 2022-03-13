using System;
using System.Threading.Tasks;
using UnityEngine;
using PWCDemo.Pooling;
using PWCDemo.Audio;
using PWCDemo.Scoring;
using PWCDemo.Particles;

namespace PWCDemo
{
    /// <summary>
    /// Class which manages the gameplay behavior of targets
    /// </summary>
    public class Target : MonoBehaviour
    {
        /// <summary>
        /// Event which fires when a <see cref="Target"/> is hit
        /// </summary>
        public Action<Target, int> OnTargetHitEvent { get; set; } = delegate { };
        /// <summary>
        /// Event which fires when a <see cref="Target"/> is cleaned up
        /// </summary>
        public Action<Target> OnTargetCleanupEvent { get; set; } = delegate { };

        /// <summary>
        /// <see cref="ParticlePlayer"/> prefab to spawn when the <see cref="Target"/> is despawned
        /// </summary>
        [Header("Despawn")]
        [SerializeField, Tooltip("Particle prefab to spawn when the target is despawned")]
        private ParticlePlayer _despawnParticlePrefab = null;
        /// <summary>
        /// Delay after which the <see cref="Target"/> should be despawned
        /// </summary>
        [SerializeField, Tooltip("Delay after which the target should be despawned")]
        private float _despawnDelay = 0.5f;
        /// <summary>
        /// Whether or not this <see cref="Target"/> should despawn itself once its hit
        /// </summary>
        [SerializeField, Tooltip("Whether or not this target should despawn itself once its hit")]
        private bool _autoDespawn = true;

        /// <summary>
        /// <see cref="Transform"/> representing the center of the <see cref="Target"/>
        /// </summary>
        [Header("Scoring")]
        [SerializeField, Tooltip("Transform representing the center of the target")]
        private Transform _targetCenter = null;
        /// <summary>
        /// <see cref="ScoreLabel"/> prefab to spawn to display score when a <see cref="Target"/> is hit
        /// </summary>
        [SerializeField, Tooltip("Label prefab to spawn to display score when a target is hit")]
        private ScoreLabel _scoreLabelPrefab = null;
        /// <summary>
        /// Configurable <see cref="ScoreThreshold"/> used to calculate user score
        /// </summary>
        [SerializeField, Tooltip("Configurable scoring thresholds used to calculate user score")]
        private ScoreThreshold[] _scoringThresholds = new ScoreThreshold[0];

        /// <summary>
        /// <see cref="AudioPlayer"/> prefab to spawn when playing a sound effect
        /// </summary>
        [Header("Audio")]
        [SerializeField, Tooltip("Prefab to spawn when playing a sound effect")]
        private AudioPlayer _audioPlayerPrefab = null;
        /// <summary>
        /// Sound to play when a <see cref="Target"/> is spawned
        /// </summary>
        [SerializeField, Tooltip("Sound to play when a target is spawned")]
        private AudioClip _spawnSound = null;
        /// <summary>
        /// Sound to play when a <see cref="Target"/> is hit
        /// </summary>
        [SerializeField, Tooltip("Sound to play when a target is hit")]
        private AudioClip _hitSound = null;
        /// <summary>
        /// Sound to play when a <see cref="Target"/> is despawned
        /// </summary>
        [SerializeField, Tooltip("Sound to play when a target is despawned")]
        private AudioClip _despawnSound = null;

        /// <summary>
        /// Default <see cref="Material"/> the <see cref="Target"/> should be rendered with
        /// </summary>
        [Header("Rendering")]
        [SerializeField, Tooltip("Default material the target should be rendered with")]
        private Material _defaultMaterial = null;
        /// <summary>
        /// <see cref="Material"/> the <see cref="Target"/> should be rendered with once hit
        /// </summary>
        [SerializeField, Tooltip("Material the target should be rendered with once hit")]
        private Material _hitMaterial = null;
        /// <summary>
        /// Reference to the <see cref="Target"/>'s <see cref="MeshRenderer"/>
        /// </summary>
        [SerializeField, Tooltip("Reference to the target's mesh renderer")]
        private MeshRenderer _meshRenderer = null;

        private bool _canHit = false;

        /// <summary>
        /// Handles initializing the <see cref="Target"/>
        /// </summary>
        public void Initialize()
        {
            PoolManager.Request(_audioPlayerPrefab, transform.position, Quaternion.identity).Play(_spawnSound);
            _meshRenderer.material = _defaultMaterial;
            _canHit = true;
        }

        /// <summary>
        /// Handles cleaning up the <see cref="Target"/> and returning it to the object pool
        /// </summary>
        public void Cleanup()
        {
            if (!gameObject.activeSelf) return;

            PoolManager.Request(_audioPlayerPrefab, transform.position, Quaternion.identity).Play(_despawnSound);
            PoolManager.Recycle(gameObject);
            ParticlePlayer despawnParticle = PoolManager.Request(_despawnParticlePrefab, transform.position, Quaternion.identity);
            despawnParticle.Play();

            OnTargetCleanupEvent(this);

            OnTargetHitEvent = delegate { };
            OnTargetCleanupEvent = delegate { };
        }

        /// <summary>
        /// Handles processing a hit from an <see cref="Arrow"/>
        /// </summary>
        /// <param name="arrow">The <see cref="Arrow"/> which impacted the target</param>
        /// <returns>An awaitable <see cref="Task"/></returns>
        public async Task ProcessHit(Collision collision = null, Arrow arrow = null)
        {
            if (!_canHit) return;

            int score = CalculateScore(collision, arrow);
            OnTargetHitEvent(this, score);

            PoolManager.Request(_audioPlayerPrefab, transform.position, Quaternion.identity).Play(_hitSound);

            if (!_autoDespawn) return;

            _canHit = false;
            _meshRenderer.material = _hitMaterial;

            float elapsedTime = 0f;
            while(elapsedTime < _despawnDelay)
            {
                await Task.Yield();
                elapsedTime += Time.deltaTime;
            }

            if (arrow) arrow.Cleanup();
            Cleanup();
        }

        /// <summary>
        /// Handles calculating the score of the shot based on the point of collision
        /// </summary>
        /// <param name="collision">The point of <see cref="Collision"/></param>
        /// <param name="arrow">The <see cref="Arrow"/> triggering the <see cref="Collision"/></param>
        private int CalculateScore(Collision collision, Arrow arrow)
        {
            if (collision == null || arrow == null) return 0;

            float distance = Vector3.Distance(collision.GetContact(0).point, _targetCenter.position);
            int pointValue = 0;

            for(int i = 0; i < _scoringThresholds.Length; i++)
            {
                if(distance < _scoringThresholds[i].DistanceFromCenter)
                {
                    pointValue = _scoringThresholds[i].PointValue;
                    break;
                }
            }
            
            if(pointValue > 0)
            {
                ScoreLabel scoreLabel = PoolManager.Request(_scoreLabelPrefab, arrow.transform.position, Quaternion.identity);
                scoreLabel.Initialize(pointValue);
            }

            return pointValue;
        }


        private void OnCollisionEnter(Collision collision)
        {
            if (!_canHit) return;

            Arrow arrow = collision.collider.GetComponent<Arrow>();
            if (arrow != null) _ = ProcessHit(collision, arrow);
        }

        private void Awake()
        {
            _canHit = true;
        }

#if UNITY_EDITOR
        [ContextMenu("Simulate Hit")]
        private void SimulateHit()
        {
            _ = ProcessHit();
        }
#endif
    }
}