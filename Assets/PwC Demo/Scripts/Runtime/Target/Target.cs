using System;
using System.Threading.Tasks;
using UnityEngine;
using PWCDemo.Pooling;
using PWCDemo.Audio;
using PWCDemo.Scoring;
using PWCDemo.Particles;

namespace PWCDemo
{
    public class Target : MonoBehaviour
    {
        public Action<Target, int> OnTargetHitEvent { get; set; } = delegate { };
        public Action<Target> OnTargetCleanupEvent { get; set; } = delegate { };

        [Header("Despawn")]
        [SerializeField]
        private ParticlePlayer _despawnParticlePrefab = null;
        [SerializeField]
        private float _despawnDelay = 0.5f;
        [SerializeField]
        private bool _autoDespawn = true;

        [Header("Scoring")]
        [SerializeField]
        private Transform _targetCenter = null;
        [SerializeField]
        private ScoreLabel _scoreLabelPrefab = null;
        [SerializeField]
        private ScoreThreshold[] _scoringThresholds = new ScoreThreshold[0];


        [Header("Audio")]
        [SerializeField]
        private AudioPlayer _audioPlayerPrefab;
        [SerializeField]
        private AudioClip _spawnSound = null;
        [SerializeField]
        private AudioClip _hitSound = null;
        [SerializeField]
        private AudioClip _despawnSound = null;

        [Header("Rendering")]
        [SerializeField]
        private Material _defaultMaterial = null;
        [SerializeField]
        private Material _hitMaterial = null;

        [SerializeField]
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

            if (arrow) PoolManager.Recycle(arrow.gameObject);
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

        [ContextMenu("Simulate Hit")]
        private void SimulateHit()
        {
            _ = ProcessHit();
        }

        private void Awake()
        {
            _canHit = true;
        }
    }
}