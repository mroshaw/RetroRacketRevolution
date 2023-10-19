using System;
using System.Collections.Generic;
using DaftApplesGames.RetroRacketRevolution.Players;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DaftApplesGames.RetroRacketRevolution.Menus
{
    public class CircleTextInput : WindowBase
    {
        [BoxGroup("Settings")] public Transform circleTransform;
        [BoxGroup("Settings")] public float circleRadius = 50.0f;
        [BoxGroup("Settings")] public GameObject buttonPrefab;
        [BoxGroup("Settings")] public string[] letterArray;
        [BoxGroup("Settings")] public int maxLetters = 3;
        [BoxGroup("UI")] public TextMeshProUGUI nameEntry;
        [BoxGroup("UI")] public TextMeshProUGUI headingText;
        [BoxGroup("UI")] public Transform buttonContainer;
        [BoxGroup("Buttons")] public Button doneButton;
        [BoxGroup("Buttons")] public Button delButton;
        [FoldoutGroup("Events")] public UnityEvent<Player, string> NameSubmittedEvent;

        private int _numLetters;

        private Player _player;

        /// <summary>
        /// Initialise this component
        /// </summary>
        public override void Awake()
        {
            _numLetters = letterArray.Length;
            nameEntry.text = "";
            base.Awake();
        }

        /// <summary>
        /// Setup other components
        /// </summary>
        private void Start()
        {
            CreateLetterButtons();
        }

        /// <summary>
        /// Show and update the heading
        /// </summary>
        /// <param name="heading"></param>
        public void Show(Player player)
        {
            headingText.text = $"{player.name}, new High Score!";
            _player = player;
            base.Show();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        /// <summary>
        /// Handle the done button click
        /// </summary>
        public void DoneButton()
        {
            NameSubmittedEvent.Invoke(_player, nameEntry.text);
        }

        /// <summary>
        /// Delete the last letter
        /// </summary>
        public void DelButton()
        {
            if(letterArray.Length < 1)
            {
                return;
            }
            nameEntry.text = nameEntry.text.Remove(nameEntry.text.Length - 1, 1);
        }

        /// <summary>
        /// Add to the name text when buttons are clicked
        /// </summary>
        private void ButtonClickEventHandler(string letter)
        {
            if (nameEntry.text.Length == maxLetters)
            {
                return;
            }
            
            nameEntry.text += letter;
        }

        /// <summary>
        /// Create a circle of buttons
        /// </summary>
        private void CreateLetterButtons()
        {
            List<Button> buttons = new List<Button>();

            // for (int currItem = _numLetters - 1; currItem >= 0; currItem--)
            for (int currItem = 0; currItem < _numLetters; currItem++)
            {
                Vector2 spawnPosition = CalculatePosition(currItem);
                GameObject buttonGameObject = Instantiate(buttonPrefab, buttonContainer);
                buttonGameObject.name = $"Index:{currItem}-Letter:{letterArray[currItem]}";
                Button button = buttonGameObject.GetComponent<Button>();
                buttonGameObject.transform.position = spawnPosition;
                buttonGameObject.GetComponentInChildren<TextMeshProUGUI>().text = letterArray[currItem];
                buttonGameObject.GetComponent<TextInputButton>().LetterButtonClickedEvent.AddListener(ButtonClickEventHandler);
                buttons.Add(button);
            }

            // Now set the Nav properties
            for (int currButtonIndex = 0; currButtonIndex < buttons.Count; currButtonIndex++)
            {
                // Create Nav
                Navigation customNav = new Navigation();
                customNav.mode = Navigation.Mode.Explicit;

                // First item
                if (currButtonIndex == 0)
                {
                    customNav.selectOnLeft = doneButton;
                    customNav.selectOnRight = buttons[currButtonIndex + 1];
                    buttons[currButtonIndex].navigation = customNav;
                    EventSystem.current.firstSelectedGameObject = buttons[currButtonIndex].gameObject;
                    continue;
                }

                // Last item
                if (currButtonIndex == buttons.Count - 1)
                {
                    customNav.selectOnLeft = buttons[currButtonIndex - 1];
                    customNav.selectOnRight = delButton;
                    buttons[currButtonIndex].navigation = customNav;
                    continue;
                }

                // Other buttons
                customNav.selectOnLeft = buttons[currButtonIndex - 1];
                customNav.selectOnRight = buttons[currButtonIndex + 1];
                buttons[currButtonIndex].navigation = customNav;
            }

            // Del button
            Navigation customNavDel = new Navigation();
            customNavDel.mode = Navigation.Mode.Explicit;
            customNavDel.selectOnLeft = buttons[^1];
            customNavDel.selectOnRight = doneButton;
            delButton.navigation = customNavDel;

            // Done button
            Navigation customNavDone = new Navigation();
            customNavDone.mode = Navigation.Mode.Explicit;
            customNavDone.selectOnLeft = delButton;
            customNavDone.selectOnRight = buttons[0];
            doneButton.navigation = customNavDone;
        }

        /// <summary>
        /// Determine the position of item "itemNumber" on the circle
        /// </summary>
        /// <param name="itemNumber"></param>
        private Vector2 CalculatePosition(int itemNumber)
        {
            float radians = 2 * MathF.PI / _numLetters * itemNumber;
            float vertical = MathF.Sin(radians);
            float horizontal = MathF.Cos(radians);
            Vector2 spawnDir = new Vector2(horizontal, -vertical);
            Vector2 spawnPos = (Vector2)circleTransform.position + spawnDir * circleRadius;
            return spawnPos;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.0f, 1.0f, 0.0f);
            DrawCircle(circleTransform.position, circleRadius);
        }

        /// <summary>
        /// Draw a circle
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        private void DrawCircle(Vector2 center, float radius)
        {
            Gizmos.DrawWireSphere(new Vector3(center.x, center.y, 0.01f), radius);
        }
#endif
    }
}
