using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PWCDemo.Pooling;
using TMPro;

namespace PWCDemo
{
    public class GameController : MonoBehaviour
    {
        private const string START_ROUND_VALUE = "Shoot the target to begin the round";
        private const string ROUND_COUNTDOWN_VALUE = "Round begins in \n<b><color=#00F0CC>{0}s</color></b>";
        private const string ROUND_DURATION_VALUE = "Round ends in \n<b><color=#00F0CC>{0}s</color></b>";

        private const string CURRENT_SCORE_VALUE = "Score\n<b><color=#00F0CC>{0}</color></b>";
        private const string TARGETS_HIT_VALUE = "Targets hit\n<b><color=#00F0CC>{0}</color></b>";

        [SerializeField]
        private TextMeshProUGUI _startGameLabel = null;

        [SerializeField]
        private TextMeshProUGUI _currentScoreLabel = null;
        [SerializeField]
        private TextMeshProUGUI _targetsHitLabel = null;
        

        [SerializeField]
        private TargetSpawnVolume _targetSpawnVolume;
        
        [SerializeField]
        private Transform _startGameTargetParent = null;
        [SerializeField]
        private Target _targetPrefab = null;

        [SerializeField]
        private RoundData _roundData;

        private int _currentScore = 0;
        private int _targetsHit = 0;

        public int CurrentScore
        {
            get => _currentScore;
            set
            {
                _currentScore = value;
                _currentScoreLabel.text = string.Format(CURRENT_SCORE_VALUE, _currentScore);
            }
        }

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
            _currentScore = 0;
            _targetsHit = 0;
            StartCoroutine(RoundCoroutine());
        }

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

        private void OnStartGameTargetHit(Target target, int score)
        {
            target.OnTargetHitEvent -= OnStartGameTargetHit;
            StartRound();
        }

        private void OnGalleryTargetHit(Target target, int score)
        {
            target.OnTargetHitEvent -= OnGalleryTargetHit;

            CurrentScore += score;
            TargetsHit += 1;
        }

        private void Awake()
        {
            ReadyNewRound();
        }
    }
}