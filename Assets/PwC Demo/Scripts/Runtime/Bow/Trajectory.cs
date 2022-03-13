using UnityEngine;
using UnityEngine.SceneManagement;

namespace PWCDemo
{
    /// <summary>
    /// Class which handles rendering a preview trajectory line
    /// </summary>
    public class Trajectory : MonoBehaviour
    {
        /// <summary>
        /// <see cref="Arrow"/> used to simulate how a released <see cref="Arrow"/> would fly
        /// </summary>
        [SerializeField, Tooltip("Arrow used to simulate how a released arrow would fly")]
        private Arrow _ghostArrow = null;
        /// <summary>
        /// Reference to the <see cref="LineRenderer"/> used to show the trajectory
        /// </summary>
        [SerializeField, Tooltip("Reference to the line renderer used to show the trajectory")]
        private LineRenderer _lineRenderer = null;
        /// <summary>
        /// Number of physics iterations to perform when calculating the trajectory
        /// </summary>
        [SerializeField, Tooltip("Number of physics iterations to perform when calculating the trajectory")]
        private int _iterations = 100;

        private Scene _simulationScene = default;
        private PhysicsScene _physicsScene = default;

        /// <summary>
        /// Handles updating the simulated trajectory line of the arrow based on the given position, rotation, and force
        /// </summary>
        /// <param name="position">The origin of the <see cref="Arrow"/> being simulated</param>
        /// <param name="rotation">The rotation of the <see cref="Arrow"/> being simulated</param>
        /// <param name="force">The force applied to the <see cref="Arrow"/> being simulated</param>
        public void SimulateTrajectory(Vector3 position, Quaternion rotation, Vector3 force)
        {
            if (!_ghostArrow.gameObject.activeSelf) _ghostArrow.gameObject.SetActive(true);
            _ghostArrow.transform.SetPositionAndRotation(position, rotation);
            _ghostArrow.CachedRigidbody.velocity = force;

            _lineRenderer.positionCount = _iterations;
            for (int i = 0; i < _iterations; i++)
            {
                _physicsScene.Simulate(Time.fixedDeltaTime);
                _lineRenderer.SetPosition(i, _ghostArrow.transform.position);
            }
        }

        /// <summary>
        /// Handles clearing the simulated trajectory line
        /// </summary>
        public void Clear()
        {
            _ghostArrow.gameObject.SetActive(false);
            _lineRenderer.positionCount = 0;
        }

        /// <summary>
        /// Generates a physics simulation scene where the trajectory can be calculated
        /// </summary>
        private void CreatePhysicsScene()
        {
            _simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
            _physicsScene = _simulationScene.GetPhysicsScene();
            _ghostArrow.transform.SetParent(null);
            SceneManager.MoveGameObjectToScene(_ghostArrow.gameObject, _simulationScene);
        }

        private void Awake()
        {
            CreatePhysicsScene();
        }
    }
}

