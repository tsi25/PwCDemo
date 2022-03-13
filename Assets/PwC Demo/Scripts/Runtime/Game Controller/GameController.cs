using System.Collections;
using UnityEngine;
using PWCDemo.Pooling;
using TMPro;

namespace PWCDemo
{
    /// <summary>
    /// Class which handles the overarching game logic of the gallery minigame
    /// </summary>
    public class GameController : MonoBehaviour
    {
        private const string START_ROUND_VALUE = "Shoot the target to begin the round";
        private const string ROUND_COUNTDOWN_VALUE = "Round begins in \n<b><color=#00F0CC>{0}s</color></b>";
        private const string ROUND_DURATION_VALUE = "Round ends in \n<b><color=#00F0CC>{0}s</color></b>";

        private const string CURRENT_SCORE_VALUE = "Score\n<b><color=#00F0CC>{0}</color></b>";
        private const string TARGETS_HIT_VALUE = "Targets hit\n<b><color=#00F0CC>{0}</color></b>";

        /// <summary>
        /// <see cref="TextMeshProUGUI"/> which displays game prompts
        /// </summary>
        [Header("UI")]
        [SerializeField, Tooltip("Label which displays game prompts")]
        private TextMeshProUGUI _startGameLabel = null;
        /// <summary>
        /// <see cref="TextMeshProUGUI"/>  which dispalys the user's current score
        /// </summary>
        [SerializeField, Tooltip("Label which dispalys the user's current score")]
        private TextMeshProUGUI _currentScoreLabel = null;
        /// <summary>
        /// <see cref="TextMeshProUGUI"/> which displays the number of targets hit
        /// </summary>
        [SerializeField, Tooltip("Label which displays the number of targets hit")]
        private TextMeshProUGUI _targetsHitLabel = null;

        /// <summary>
        /// <see cref="TargetSpawnVolume"/> responsible for spawning gallery targets
        /// </summary>
        [Header("Gameplay")]
        [SerializeField, Tooltip("Target spawn volume responsible for spawning gallery targets")]
        private TargetSpawnVolume _targetSpawnVolume;
        /// <summary>
        /// The <see cref="Target"/> which can be shot to start the round
        /// </summary>
        [SerializeField, Tooltip("The target which can be shot to start the round")]
        private Transform _startGameTargetParent = null;
        /// <summary>
        /// Reference to the <see cref="Target"/> prefab
        /// </summary>
        [SerializeField, Tooltip("Reference to the target prefab")]
        private Target _targetPrefab = null;
        /// <summary>
        /// Configurable <see cref="RoundData"/> used to control the round behavior
        /// </summary>
        [SerializeField, Tooltip("Configurable round data used to control the round behavior")]
        private RoundData _roundData;

        private int _currentScore = 0;
        private int _targetsHit = 0;

        /// <summary>
        /// The user's current score
        /// </summary>
        public int CurrentScore
        {
            get => _currentScore;
            set
            {
                _currentScore = value;
                _currentScoreLabel.text = string.Format(CURRENT_SCORE_VALUE, _currentScore);
            }
        }

        /// <summary>
        /// The current number of <see cref="Target"/>s the user has hit
        /// </summary>
        public int TargetsHit
        {
            get => _targetsHit;
            set
            {
                _targetsHit = value;
                _targetsHitLabel.text = string.Format(TARGETS_HIT_VALUE, _targetsHit);
            }
        }

        /// <summary>
        /// Handles readying the next round without starting it
        /// </summary>
        public void ReadyNewRound()
        {
            Target startGameTarget = PoolManager.Request(_targetPrefab, _startGameTargetParent);
            startGameTarget.OnTargetHitEvent += OnStartGameTargetHit;
            startGameTarget.Initialize();

            _startGameLabel.text = START_ROUND_VALUE;
        }

        /// <summary>
        /// Handles beginning the round
        /// </summary>
        public void StartRound()
        {
            ResetTrackers();
            StartCoroutine(RoundCoroutine());
        }

        /// <summary>
        /// Handles resetting visual trackers IE current score and targets hit
        /// </summary>
        public void ResetTrackers()
        {
            CurrentScore = 0;
            TargetsHit = 0;
        }

        /// <summary>
        /// Coroutine which handles the gallery gameplay logic of a round
        /// </summary>
        private IEnumerator RoundCoroutine()
        {
            //process initial countdown delay as the round begins
            float elapsedTime = 0f;
            while(elapsedTime < _roundData.CountdownDelay)
            {
                _startGameLabel.text = string.Format(
                    ROUND_COUNTDOWN_VALUE,
                    Mathf.RoundToInt(_roundData.CountdownDelay - elapsedTime));

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            //process the full round
            elapsedTime = 0f;
            float nextSpawnTime = 0f;
            while(elapsedTime < _roundData.Duration)
            {
                if (elapsedTime >= nextSpawnTime)
                {
                    Target target = _targetSpawnVolume.GenerateTarget();
                    if (target != null) target.OnTargetHitEvent += OnGalleryTargetHit;

                    nextSpawnTime += Mathf.Lerp(
                        _roundData.SpawnFrequency.x, 
                        _roundData.SpawnFrequency.y, 
                        _roundData.SpawnCurve.Evaluate(elapsedTime / _roundData.Duration));
                }

                _startGameLabel.text = string.Format(
                    ROUND_DURATION_VALUE,
                    Mathf.RoundToInt(_roundData.Duration - elapsedTime));

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            //Cleanup the round and prep the next one
            _targetSpawnVolume.CleanupTargets();
            yield return new WaitForSeconds(_roundData.CountdownDelay);
            ReadyNewRound();
        }

        /// <summary>
        /// Handles intercepting the OnHitEvent from the start game <see cref="Target"/>
        /// </summary>
        /// <param name="target">The <see cref="Target"/> thast was hit</param>
        /// <param name="score">The score generated from the hit</param>
        private void OnStartGameTargetHit(Target target, int score)
        {
            target.OnTargetHitEvent -= OnStartGameTargetHit;
            StartRound();
        }

        /// <summary>
        /// Handles intercepting the OnHitEvent from a gallery <see cref="Target"/>
        /// </summary>
        /// <param name="target">The <see cref="Target"/> thast was hit</param>
        /// <param name="score">The score generated from the hit</param>
        private void OnGalleryTargetHit(Target target, int score)
        {
            target.OnTargetHitEvent -= OnGalleryTargetHit;

            CurrentScore += score;
            TargetsHit += 1;
        }

        private void Awake()
        {
            ReadyNewRound();
            ResetTrackers();
        }
    }
}