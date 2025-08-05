using Sirenix.OdinInspector;
using UnityEngine;

namespace DaftAppleGames.RetroRacketRevolution.Utils
{
    /// <summary>
    /// Component to control the custom LitShader properties
    /// </summary>
    public class MaterialTools : MonoBehaviour
    {
        private static readonly int ColorProperty = Shader.PropertyToID("_Color");

        [BoxGroup("Settings")] [SerializeField] private Color color;
        [BoxGroup("Settings")] [SerializeField] private MeshRenderer[] renderers;

        private void Awake()
        {
            SetColor();
        }

        [Button("Refresh Renderers")]
        private void RefreshRenderers()
        {
            renderers = GetComponentsInChildren<MeshRenderer>(true);
        }

        internal void SetColor()
        {
            foreach (Renderer currRenderer in renderers)
            {
                currRenderer.material.SetColor(ColorProperty, color);
            }
        }

        internal void SetColor(Color newColor)
        {
            color = newColor;
            SetColor();
        }
    }
}