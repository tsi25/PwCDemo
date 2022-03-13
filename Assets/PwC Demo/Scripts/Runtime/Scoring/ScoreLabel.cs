using System.Collections;
using UnityEngine;
using TMPro;
using PWCDemo.Pooling;

namespace PWCDemo.Scoring
{
    public class ScoreLabel : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _label;
        [SerializeField]
        private CanvasGroup _canvasGroup;
        [SerializeField]
        private Vector3 _targetOffset = Vector3.zero;
        [SerializeField]
        private float _floatDuration = 1f;
        [SerializeField]
        private AnimationCurve _curve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

        /// <summary>
        /// Handles initializing the score label based on the given score, and starting its float upwards
        /// </summary>
        /// <param name="score">The score to initialize the label with</param>
        public void Initialize(int score)
        {
            transform.LookAt(Camera.main.transform, Vector3.up);

            _label.text = score.ToString();
            _canvasGroup.alpha = 1;
            _label.rectTransform.anchoredPosition = Vector3.zero;

            StartCoroutine(FloatCoroutine());
        }

        /// <summary>
        /// Handles cleaning up the score label
        /// </summary>
        public void Cleanup()
        {
            PoolManager.Recycle(gameObject);
        }


        private IEnumerator FloatCoroutine()
        {
            float elapsedTime = 0f;
            float curveEvaluation = 0f;

            while(elapsedTime < _floatDuration)
            {
                curveEvaluation = _curve.Evaluate(elapsedTime / _floatDuration);
                _canvasGroup.alpha = Mathf.Lerp(1f, 0f, curveEvaluation);
                _label.rectTransform.anchoredPosition = Vector3.Lerp(Vector3.zero, _targetOffset, curveEvaluation);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _canvasGroup.alpha = 0f;
            _label.rectTransform.anchoredPosition = _targetOffset;
            Cleanup();
        }
    }
}