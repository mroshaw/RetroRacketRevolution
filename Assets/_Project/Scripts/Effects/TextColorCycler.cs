using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;

namespace DaftAppleGames.RetroRacketRevolution.Effects
{
    public class TextColorCycler : MonoBehaviour
    {
        // Public serializable properties
        [BoxGroup("Settings")] [SerializeField] private bool cycleOnAwake = false;
        [BoxGroup("Settings")] [SerializeField] private bool cycleOnStart = false;
        [BoxGroup("Settings")] [SerializeField] private bool cycleOnEnable = true;

        private static readonly Color Orange = new Color(255, 165, 0, 1);
        private static readonly Color Indigo = new Color(29, 0, 51, 1);
        private static readonly Color Violet = new Color(127, 0, 255, 1);
        private static readonly Color Pink = new Color(255, 20, 147, 1);

        private static readonly Color[] Colors = { Color.red, Orange, Color.yellow, Color.green, Color.blue, Indigo };
        private static readonly Color[] Colors2 =
            { Color.red, Orange, Color.yellow, Color.green, Color.blue, Indigo, Violet };

        // Public properties

        // Private fields
        private TMP_Text _textComponent;

        /// <summary>
        /// Initialise this component
        /// </summary>   
        private void Awake()
        {
            _textComponent = GetComponent<TMP_Text>();

            if (cycleOnAwake)
            {
                StartColor();
            }
        }

        /// <summary>
        /// Start color cycle if selected
        /// </summary>
        private void OnEnable()
        {
            if (cycleOnEnable)
            {
                StartColor();
            }
        }

        /// <summary>
        /// Configure the component on Start
        /// </summary>
        private void Start()
        {
            if (cycleOnStart)
            {
                StartColor();
            }
        }

        /// <summary>
        /// Clean up on Destroy
        /// </summary>
        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        /// <summary>
        /// Stop all co-routines on disable
        /// </summary>
        private void OnDisable()
        {
            StopAllCoroutines();
        }

        /// <summary>
        /// Start the animation
        /// </summary>
        public void StartColor()
        {
            StartCoroutine(AnimateVertexColors());
        }

        /// <summary>
        /// Stop the animation
        /// </summary>
        public void StopColor()
        {
            StopCoroutine(AnimateVertexColors());
            // StopAllCoroutines();
        }

        /// <summary>
        /// Method to animate vertex colors of a TMP Text object.
        /// </summary>
        /// <returns></returns>
        private IEnumerator AnimateVertexColors()
        {
            // Force the text object to update right away so we can have geometry to modify right from the start.
            _textComponent.ForceMeshUpdate();

            TMP_TextInfo textInfo = _textComponent.textInfo;
            int currentCharacter = 0;

            Color32[] newVertexColors;
            Color32 c0 = _textComponent.color;

            int currColor = 0;

            Color[] newColors;

            while (true)
            {
                int characterCount = textInfo.characterCount;

                // If No Characters then just yield and wait for some text to be added
                if (characterCount == 0)
                {
                    yield return new WaitForSecondsRealtime(0.25f);
                    continue;
                }

                int charCount = _textComponent.text.Count(c => !Char.IsWhiteSpace(c));

                if (charCount % 7 == 0)
                {
                    newColors = Colors;
                }
                else
                {
                    newColors = Colors2;
                }

                // Get the index of the material used by the current character.
                int materialIndex = textInfo.characterInfo[currentCharacter].materialReferenceIndex;

                // Get the vertex colors of the mesh used by this text element (character or sprite).
                newVertexColors = textInfo.meshInfo[materialIndex].colors32;

                // Get the index of the first vertex used by this text element.
                int vertexIndex = textInfo.characterInfo[currentCharacter].vertexIndex;

                // Only change the vertex color if the text element is visible.
                if (textInfo.characterInfo[currentCharacter].isVisible)
                {
                    // c0 = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
                    // int randColor = (int)Random.Range(0, _colors.Length - 1);
                    // c0 = _colors[randColor];
                    c0 = newColors[currColor];
                    if (currColor == newColors.Length - 1)
                    {
                        currColor = 0;
                    }
                    else
                    {
                        currColor++;
                    }

                    newVertexColors[vertexIndex + 0] = c0;
                    newVertexColors[vertexIndex + 1] = c0;
                    newVertexColors[vertexIndex + 2] = c0;
                    newVertexColors[vertexIndex + 3] = c0;

                    // New function which pushes (all) updated vertex data to the appropriate meshes when using either the Mesh Renderer or CanvasRenderer.
                    _textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

                    // This last process could be done to only update the vertex data that has changed as opposed to all of the vertex data but it would require extra steps and knowing what type of renderer is used.
                    // These extra steps would be a performance optimization but it is unlikely that such optimization will be necessary.
                }

                currentCharacter = (currentCharacter + 1) % characterCount;

                yield return new WaitForSecondsRealtime(0.05f);
            }
        }
    }
}