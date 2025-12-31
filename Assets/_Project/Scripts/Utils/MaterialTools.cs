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
        private static readonly int EmissiveColorProperty = Shader.PropertyToID("_Emissive_Color");
        private static readonly int EmissiveProperty = Shader.PropertyToID("_Emission");

        [BoxGroup("Settings")] [SerializeField] private MeshRenderer[] renderers;

        [Button("Refresh Renderers")]
        private void RefreshRenderers()
        {
            renderers = GetComponentsInChildren<MeshRenderer>(true);
        }

        internal void SetColor(Color newColor)
        {
            SetColorProperty(ColorProperty, newColor);
        }

        internal void SetEmissiveColor(Color newEmissiveColor)
        {
            SetColorProperty(EmissiveColorProperty, newEmissiveColor);
        }

        internal void SetEmission(float newEmission)
        {
            SetFloatProperty(EmissiveProperty, newEmission);
        }


        private void SetFloatProperty(int propertyId, float newValue)
        {
            foreach (MeshRenderer currRenderer in renderers)
            {
                currRenderer.material.SetFloat(propertyId, newValue);
            }
        }

        private void SetColorProperty(int propertyId, Color newColor)
        {
            foreach (MeshRenderer currRenderer in renderers)
            {
                currRenderer.material.SetColor(propertyId, newColor);
            }
        }
    }
}