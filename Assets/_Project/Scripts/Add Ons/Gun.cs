using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;

namespace DaftAppleGames.RetroRacketRevolution.AddOns
{
    public abstract class Gun : AddOn
    {
        [BoxGroup("Settings")] [SerializeField] private GameObject projectilePrefab;
        [BoxGroup("Settings")] [SerializeField] private float delayBetweenShots = 1.0f;
        [BoxGroup("Settings")] [SerializeField] private Transform projectileSpawnTransform;
        [BoxGroup("Settings")] [SerializeField] private float projectileVelocity = 20.0f;
        [BoxGroup("Settings")] public GameObject projectileContainer;
        [BoxGroup("Audio")] public AudioClip fireClip;

        [BoxGroup("Deployment")] [SerializeField] private Quaternion deployedRotation;
        [BoxGroup("Deployment")] [SerializeField] private Quaternion retractedRotation;
        [BoxGroup("Deployment")] [SerializeField] private float deployTime;

        // Projective pool
        private ObjectPool<GameObject> _projectilePool;
        private float _lastShotCounter;

        private bool _isFiring;

        /// <summary>
        /// Create the object pool and initialise
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            _projectilePool = new ObjectPool<GameObject>(CreateProjectile, OnTakeProjectileFromPool,
                OnReturnProjectileToPool, OnDestroyProjectile, true, 20);
            _lastShotCounter = 0.0f;

            // Unparent to avoid moving with the player
            projectileContainer.transform.SetParent(null);
        }

        protected virtual void Update()
        {
            if (!IsDeployed)
            {
                return;
            }

            if (_isFiring)
            {
                DoFiring();
            }

            _lastShotCounter += Time.deltaTime;
        }

        /// <summary>
        /// Deploy the Gun
        /// </summary>
        protected internal override IEnumerator Deploy(bool immediate = false)
        {
            yield return RotateWeapon(retractedRotation, deployedRotation, immediate ? 0.0f : deployTime);
        }

        /// <summary>
        /// Retract the Gun
        /// </summary>
        protected internal override IEnumerator Retract(bool immediate = false)
        {
            yield return RotateWeapon(deployedRotation, retractedRotation, immediate ? 0.0f : deployTime);
        }

        private IEnumerator RotateWeapon(Quaternion startRotation, Quaternion endRotation, float rotateTime)
        {
            float elapsedTime = 0;
            while (elapsedTime < rotateTime)
            {
                transform.localRotation = Quaternion.Lerp(startRotation, endRotation, elapsedTime / rotateTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.localRotation = endRotation;
        }

        /// <summary>
        /// Create action for pool
        /// </summary>
        /// <returns></returns>
        private GameObject CreateProjectile()
        {
            GameObject bulletGameObject = Instantiate(projectilePrefab, projectileContainer.transform, true);
            BulletProjectile projectile = bulletGameObject.GetComponent<BulletProjectile>();
            projectile.WeaponAddOn = this;
            projectile.transform.position = projectileSpawnTransform.position;
            return bulletGameObject;
        }

        /// <summary>
        /// Take action for pool
        /// </summary>
        private void OnTakeProjectileFromPool(GameObject projectile)
        {
            projectile.transform.position = projectileSpawnTransform.position;
            projectile.SetActive(true);
        }

        /// <summary>
        /// Return action for pool
        /// </summary>
        private void OnReturnProjectileToPool(GameObject projectile)
        {
            projectile.GetComponent<Projectile>().projectileCollideEvent.RemoveAllListeners();
            projectile.transform.position = projectileSpawnTransform.position;
            projectile.SetActive(false);
        }

        /// <summary>
        /// Destroy action for pool
        /// </summary>
        private void OnDestroyProjectile(GameObject projectile)
        {
            Destroy(projectile);
        }

        /// <summary>
        /// Can the laser fire?
        /// </summary>
        /// <returns></returns>
        private bool CanFire()
        {
            return _lastShotCounter > delayBetweenShots;
        }

        internal override void Fire()
        {
            _isFiring = true;
        }

        internal override void StopFire()
        {
            _isFiring = false;
        }

        /// <summary>
        /// Fire the cannon
        /// </summary>
        protected virtual void DoFiring()
        {
            if (CanFire())
            {
                // Fire!
                if (AudioSource.enabled)
                {
                    AudioSource.PlayOneShot(fireClip);
                }

                _lastShotCounter = 0.0f;
                GameObject gunProjectile = _projectilePool.Get();
                Projectile projectile = gunProjectile.GetComponent<Projectile>();
                projectile.projectileCollideEvent.AddListener(OnReturnProjectileToPool);
                projectile.Fire(projectileVelocity);
                PostFiring();
            }
        }

        protected virtual void PostFiring()
        {
        }
    }
}