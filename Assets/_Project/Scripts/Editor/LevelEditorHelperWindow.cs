using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

namespace DaftApplesGames.RetroRacketRevolution.Editor
{


    public class LevelEditorHelperWindow : OdinEditorWindow
    {
        [MenuItem("Level Editor/Helper Window")]
        private static void OpenWindow()
        {
            GetWindow<LevelEditorHelperWindow>().Show();
        }

        [BoxGroup("Grid Settings")] public int columns = 15;
        [BoxGroup("Grid Settings")] public int rows = 12;

        [BoxGroup("Settings")] public GameObject buttonPrefab;


        [Button("Update Grid", ButtonSizes.Large)]
        public void UpdateGridButton()
        {
            // Find the Canvas Panel
            Canvas canvas = FindAnyObjectByType<Canvas>();
            GameObject panel = canvas.gameObject.transform.Find("Grid Panel").gameObject;

            Transform gridTransform = panel.transform.Find("Grid");
            if (gridTransform)
            {
                DestroyImmediate(gridTransform.gameObject);
            }

            GameObject newGrid = new GameObject("Grid", typeof(RectTransform));
            newGrid.transform.SetParent(panel.transform, true);
            SetAndStretchToParentSize(newGrid.GetComponent<RectTransform>(), panel.gameObject.GetComponent<RectTransform>());

            VerticalLayoutGroup vertical = newGrid.AddComponent<VerticalLayoutGroup>();
            vertical.childForceExpandHeight = false;
            vertical.childForceExpandWidth = false;

            for (int currRow = 0; currRow < rows; currRow++)
            {
                // Create row
                GameObject rowGameObject = new GameObject($"Row{currRow}");
                rowGameObject.transform.SetParent(newGrid.transform);
                HorizontalLayoutGroup horizontal = rowGameObject.AddComponent<HorizontalLayoutGroup>();
                horizontal.childForceExpandHeight = false;
                horizontal.childForceExpandWidth = false;

                // Iterate and create buttons
                for (int currCol = 0; currCol < columns; currCol++)
                {
                    GameObject newButtonGameObject = PrefabUtility.InstantiatePrefab(buttonPrefab) as GameObject;
                    newButtonGameObject.transform.SetParent(rowGameObject.transform);
                    newButtonGameObject.name = $"Brick{currRow}{currCol}";
                    newButtonGameObject.GetComponentInChildren<TextMeshProUGUI>().text = $"{currRow},{currCol}\n";
                }
            }

            newGrid.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            
        }

        public void SetAndStretchToParentSize(RectTransform _mRect, RectTransform _parent)
        {
            _mRect.anchoredPosition = _parent.position;
            _mRect.anchorMin = new Vector2(1, 0);
            _mRect.anchorMax = new Vector2(0, 1);
            _mRect.pivot = new Vector2(0.5f, 0.5f);
            _mRect.sizeDelta = _parent.rect.size;
            _mRect.transform.SetParent(_parent);
        }

    }
}