using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;

namespace DaftAppleGames.RetroRacketRevolution.AddOns
{
    public class GatlingGun : Gun
    {
        [BoxGroup("Gatling")] [SerializeField] private Transform barrels;
        [BoxGroup("Gatling")] [SerializeField] private float barrelRotateSpeed;

        [BoxGroup("Casing")] [SerializeField] private Transform casingSpawnTransform;
        [BoxGroup("Casing")] [SerializeField] private GameObject casingPrefab;
        [BoxGroup("Casing")] [SerializeField] private GameObject casingContainer;
        [BoxGroup("Casing")] [SerializeField] private float directionRandomness = 0.2f;
        [BoxGroup("Casing")] [SerializeField] private float ejectionForce = 2.0f;
        [BoxGroup("Casing")] [SerializeField] private float ejectionTorqueRange = 1.0f;
        [BoxGroup("Casing")] [SerializeField] private float casingLifespan = 2.0f;

        private ObjectPool<GameObject> _casingPool;

        protected override void Awake()
        {
            base.Awake();
            _casingPool = new ObjectPool<GameObject>(CreateCasing, OnTakeCasingFromPool, OnReturnCasingToPool,
                OnDestroyCasing, true, 20);

            // Unparent to avoid moving with the player
            casingContainer.transform.SetParent(null);
        }

        /// <summary>
        /// Check when we're ready to fire
        /// </summary>
        protected override void Update()
        {
            base.Update();
            if (IsDeployed)
            {
                RotateBarrel();
            }
        }

        private void RotateBarrel()
        {
            barrels.Rotate(0, barrelRotateSpeed * Time.deltaTime, 0);
        }

        protected override void PostFiring()
        {
            EjectShell();
        }

        private void EjectShell()
        {
            GameObject casing = _casingPool.Get();

            // 2. Get the Rigidbody2D component
            Rigidbody rb = casing.GetComponent<Rigidbody>();

            Vector3 randomDirection = casingSpawnTransform.right +
                                      (Random.insideUnitSphere * directionRandomness);
            randomDirection.Normalize();
            rb.AddForce(randomDirection * ejectionForce, ForceMode.Impulse);

            // Apply random torque
            Vector3 randomTorque = new Vector3(
                Random.Range(-ejectionTorqueRange, ejectionTorqueRange),
                Random.Range(-ejectionTorqueRange, ejectionTorqueRange),
                Random.Range(-ejectionTorqueRange, ejectionTorqueRange)
            );
            rb.AddTorque(randomTorque, ForceMode.Impulse);

            Destroy(casing, casingLifespan);
        }

        /// <summary>
        /// Create action for pool
        /// </summary>
        private GameObject CreateCasing()
        {
            GameObject bulletGameObject = Instantiate(casingPrefab, projectileContainer.transform, true);
            BulletProjectile projectile = bulletGameObject.GetComponent<BulletProjectile>();
            projectile.WeaponAddOn = this;
            projectile.transform.SetParent(casingContainer.transform);
            projectile.transform.position = casingSpawnTransform.position;
            return bulletGameObject;
        }

        /// <summary>
        /// Take action for pool
        /// </summary>
        private void OnTakeCasingFromPool(GameObject casing)
        {
            casing.transform.position = casingSpawnTransform.position;
            casing.SetActive(true);
        }

        /// <summary>
        /// Return action for pool
        /// </summary>
        private void OnReturnCasingToPool(GameObject casing)
        {
            casing.transform.position = casingSpawnTransform.position;
            casing.SetActive(false);
        }

        /// <summary>
        /// Destroy action for pool
        /// </summary>
        private void OnDestroyCasing(GameObject casing)
        {
            Destroy(casing);
        }
    }
}