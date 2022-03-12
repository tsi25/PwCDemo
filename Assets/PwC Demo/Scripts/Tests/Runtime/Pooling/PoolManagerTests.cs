using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PWCDemo.Pooling;

namespace PWCDemo.UnitTests.Pooling
{
    public class PoolManagerTests
    {
        /// <summary>
        /// Tests whether or not <see cref="GameObject"/>s can be requested as expected from the <see cref="PoolManager"/>
        /// </summary>
        [Test]
        public void PoolManagerRequestGameObject()
        {
            GameObject prefab = PoolManagerUtility.Instance.GameObjectPrefab;

            GameObject instance = PoolManager.Request(prefab);

            Assert.IsTrue(instance.activeSelf);
            Assert.IsNotNull(instance.GetComponent<PoolId>());
        }

        /// <summary>
        /// Tests whether or not objects with generic components can be requested as expected from the <see cref="PoolManager"/>
        /// </summary>
        [Test]
        public void PoolManagerRequestComponent()
        {
            Transform prefab = PoolManagerUtility.Instance.ComponentPrefab;

            Transform instance = PoolManager.Request(prefab);

            Assert.IsTrue(instance.gameObject.activeSelf);
            Assert.IsNotNull(instance.GetComponent<PoolId>());
        }

        /// <summary>
        /// Tests whether or not objects can be requested as expected to a given position and rotation
        /// </summary>
        [Test]
        public void PoolManagerRequestPositionAndRotation()
        {
            Transform prefab = PoolManagerUtility.Instance.ComponentPrefab;

            Vector3 position = new Vector3(10f, 1.5f, -2f);
            Quaternion rotation = new Quaternion(1f, 0.2f, -1f, 0.5f);

            Transform instance = PoolManager.Request(prefab, position, rotation);

            Assert.IsTrue(instance.gameObject.activeSelf);
            Assert.IsNotNull(instance.GetComponent<PoolId>());
            Assert.IsTrue(instance.position == position);
            Assert.IsTrue(instance.rotation == rotation);
        }

        /// <summary>
        /// Tests whether or not objects can be requested as expected to a given transform
        /// </summary>
        [Test]
        public void PoolManagerRequestTransform()
        {
            Transform prefab = PoolManagerUtility.Instance.ComponentPrefab;

            Transform parent = new GameObject("Parent").transform;

            Vector3 position = new Vector3(10f, 1.5f, -2f);
            Quaternion rotation = new Quaternion(1f, 0.2f, -1f, 0.5f);
            parent.SetPositionAndRotation(position, rotation);

            Transform instance = PoolManager.Request(prefab, parent);

            Assert.IsTrue(instance.gameObject.activeSelf);
            Assert.IsNotNull(instance.GetComponent<PoolId>());
            Assert.IsTrue(instance.parent == parent);
            Assert.IsTrue(instance.localPosition == Vector3.zero);
            Assert.IsTrue(instance.localRotation == Quaternion.identity);
        }

        /// <summary>
        /// Tests whether or not objects can be recycled as expected
        /// </summary>
        [Test]
        public void PoolManagerRecycle()
        {
            GameObject prefab = PoolManagerUtility.Instance.GameObjectPrefab;

            GameObject instance = PoolManager.Request(prefab);

            Assert.IsTrue(instance.activeSelf);
            Assert.IsNotNull(instance.GetComponent<PoolId>());

            PoolManager.Recycle(instance);
            Assert.IsFalse(instance.activeSelf);
            Assert.IsTrue(instance.transform.parent == PoolManager.Instance.transform);
        }
    }
}