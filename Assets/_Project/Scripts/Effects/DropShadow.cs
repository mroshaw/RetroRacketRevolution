using Sirenix.OdinInspector;
using UnityEngine;

namespace DaftAppleGames.RetroRacketRevolution.Effects
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class DropShadow : MonoBehaviour
    {
        [BoxGroup("Settings")] public Vector2 ShadowOffset;
        [BoxGroup("Settings")] public Material ShadowMaterial;
        [BoxGroup("Settings")] public bool updateShadow = false;
        [BoxGroup("Settings")] public Transform matchTransform;
        private SpriteRenderer _spriteRenderer;
        private GameObject _shadowGameObject;

        private Transform _matchTransform;

        /// <summary>
        /// Initialise this component
        /// </summary>
        private void Awake()
        {
            _matchTransform = matchTransform == null ? transform : matchTransform;
        }

        /// <summary>
        /// Apply the shadow
        /// </summary>
        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();

            //create a new gameobject to be used as drop shadow
            _shadowGameObject = new GameObject("Shadow");
            _shadowGameObject.transform.SetParent(transform);
            _shadowGameObject.transform.localPosition = transform.localPosition + (Vector3)ShadowOffset;
            _shadowGameObject.transform.localScale = new Vector3(1, 1, 1);
            //create a new SpriteRenderer for Shadow gameobject
            SpriteRenderer shadowSpriteRenderer = _shadowGameObject.AddComponent<SpriteRenderer>();

            //set the shadow gameobject's sprite to the original sprite
            shadowSpriteRenderer.sprite = _spriteRenderer.sprite;
            //set the shadow gameobject's material to the shadow material we created
            shadowSpriteRenderer.material = ShadowMaterial;

            //update the sorting layer of the shadow to always lie behind the sprite
            shadowSpriteRenderer.sortingLayerName = _spriteRenderer.sortingLayerName;
            shadowSpriteRenderer.sortingOrder = _spriteRenderer.sortingOrder - 1;
        }
        
        /// <summary>
        /// Update position
        /// </summary>
        private void LateUpdate()
        {
            if (!updateShadow)
            {
                return;
            }
            //update the position and rotation of the sprite's shadow with moving sprite
            _shadowGameObject.transform.position = _matchTransform.position + (Vector3)ShadowOffset;
            _shadowGameObject.transform.rotation = _matchTransform.rotation;
        }
    }
}