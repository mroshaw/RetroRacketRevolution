using DaftAppleGames.RetroRacketRevolution.Bonuses;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DaftAppleGames.RetroRacketRevolution.Menus
{
    public class BonusSummaryWindow : WindowBase
    {
        [BoxGroup("Data")] public BonusData bonusData;
        [BoxGroup("UI Settings")] public GameObject bonusContainer;
        [BoxGroup("UI Settings")] public GameObject bonusTemplateGameObject;

        /// <summary>
        /// Update with the latest High Scores
        /// </summary>
        public override void Start()
        {
            base.Start();
            RefreshBonusData();
        }

        /// <summary>
        /// Refresh scores before showing
        /// </summary>
        public override void Show()
        {
            base.Show();
        }

        /// <summary>
        /// Refresh the Bonus Data panel
        /// </summary>
        private void RefreshBonusData()
        {
            // Clear down bonus data
            foreach (Transform childTransform in bonusContainer.transform)
            {
                Destroy(childTransform.gameObject);
            }

            // Recreate them
            foreach (BonusData.BonusDef bonusDef in bonusData.bonuses)
            {
                if (bonusDef.type == BonusType.None)
                {
                    continue;
                }

                GameObject newEntry = Instantiate(bonusTemplateGameObject);
                newEntry.transform.SetParent(bonusContainer.transform);
                newEntry.transform.localScale = new Vector3(1, 1, 1);
                newEntry.transform.localPosition = new Vector3(0, 0, 0);
                newEntry.GetComponent<NameImageTemplate>().SetEntryContent(bonusDef.friendlyName, bonusDef.description,
                    bonusDef.levelEditorSprite);
                newEntry.SetActive(true);
            }
        }
    }
}