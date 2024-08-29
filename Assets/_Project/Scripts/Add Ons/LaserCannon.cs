using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;

namespace DaftAppleGames.RetroRacketRevolution
{
    public class LaserCannon : AddOn
    {
        [BoxGroup("Laser Settings")] public GameObject laserBoltPrefab;
        [BoxGroup("Laser Settings")] public float delayBetweenShots = 1.0f;
        [BoxGroup("Laser Settings")] public Transform barrelEndTransform;
        [BoxGroup("Settings")] public GameObject projectileContainer;
        [BoxGroup("Audio")] public AudioClip fireClip;

        public HardPoint AttachedHardPoint => _hardPoint;

        // Laser bolt pool
        private ObjectPool<GameObject> _laserBoltPool;
        private float _lastShotCounter;
        private Sprite[] _laserBoltSprites;
        private AudioSource _audioSource;
        private HardPoint _hardPoint;

        /// <summary>
        /// Create the object pool and initialise
        /// </summary>
        public override void Awake()
        {
            base.Awake();
            _laserBoltPool = new ObjectPool<GameObject>(CreateLaserBolt, OnTakeLaserBoltFromPool, OnReturnLaserBoltToPool, OnDestroyLaserBolt, true, 20);
            _lastShotCounter = 0.0f;
            _audioSource = GetComponent<AudioSource>();
            _hardPoint = GetComponentInParent<HardPoint>();
        }

        /// <summary>
        /// Configure components
        /// </summary>
        private void Start()
        {
            // Un-parent the bolt container, to prevent it following the player
            projectileContainer.transform.SetParent(null);

        }

        /// <summary>
        /// Check when we're ready to fire
        /// </summary>
        private void Update()
        {
            _lastShotCounter += Time.deltaTime;
        }

        /// <summary>
        /// Create action for pool
        /// </summary>
        /// <returns></returns>
        private GameObject CreateLaserBolt()
        {
            GameObject laserBoltGameObject = Instantiate(laserBoltPrefab, projectileContainer.transform, true);
            LaserBolt laserBolt = laserBoltGameObject.GetComponent<LaserBolt>();
            laserBolt.LaserCannon = this;
            return laserBoltGameObject;
        }

        /// <summary>
        /// Take action for pool
        /// </summary>
        /// <param name="laserBolt"></param>
        private static void OnTakeLaserBoltFromPool(GameObject laserBolt)
        {
            laserBolt.SetActive(true);
        }

        /// <summary>
        /// Return action for pool
        /// </summary>
        /// <param name="laserBolt"></param>
        private static void OnReturnLaserBoltToPool(GameObject laserBolt)
        {
            laserBolt.SetActive(false);
        }

        /// <summary>
        /// Destroy action for pool
        /// </summary>
        /// <param name="laserBolt"></param>
        private static void OnDestroyLaserBolt(GameObject laserBolt)
        {
            Destroy(laserBolt);
        }

        /// <summary>
        /// Can the laser fire?
        /// </summary>
        /// <returns></returns>
        private bool CanFire()
        {
            return _lastShotCounter > delayBetweenShots;
        }

        /// <summary>
        /// Fire the cannon
        /// </summary>
        public void Fire()
        {
            if (CanFire())
            {
                // Fire!
                if (_audioSource.enabled)
                {
                    _audioSource.PlayOneShot(fireClip);
                    _lastShotCounter = 0.0f;
                    GameObject laserBoltObject = _laserBoltPool.Get();
                    LaserBolt laserBolt = laserBoltObject.GetComponent<LaserBolt>();
                    laserBolt.LaserBoltCollideEvent.AddListener(OnReturnLaserBoltToPool);
                    laserBolt.LaserCannon = this;
                    laserBoltObject.gameObject.transform.position = barrelEndTransform.position;
                    laserBoltObject.gameObject.transform.localScale = new Vector2(40, 80);
                }
            }
        }
    }
}