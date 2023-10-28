using System;
using DaftApplesGames.RetroRacketRevolution.Bonuses;
using DaftApplesGames.RetroRacketRevolution.Bricks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DaftApplesGames.RetroRacketRevolution.Menus
{
    public class HowToPlayWindow : WindowBase
    {
        [BoxGroup("Data")] public BonusData bonusData;
        [BoxGroup("Data")] public BrickTypeData brickTypeData;
        [BoxGroup("UI Settings")] public GameObject bonusContainer;
        [BoxGroup("UI Settings")] public GameObject bonusTemplateGameObject;
        [BoxGroup("UI Settings")] public GameObject brickContainer;
        [BoxGroup("UI Settings")] public GameObject brickTemplateGameObject;
        
        /// <summary>
        /// Update with the latest High Scores
        /// </summary>
        private void Start()
        {
            RefreshBonusData();
            RefreshBrickData();
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
            foreach (BonusData.BonusDef bonusDef in bonusData.Bonuses)
            {
                if (bonusDef.Type == BonusType.None)
                {
                    continue;
                }
                GameObject newEntry = Instantiate(bonusTemplateGameObject);
                newEntry.transform.SetParent(bonusContainer.transform);
                newEntry.transform.localScale = new Vector3(1, 1, 1);
                newEntry.transform.localPosition = new Vector3(0, 0, 0);
                newEntry.GetComponent<NameImageTemplate>().SetEntryContent(bonusDef.FriendlyName, bonusDef.Description, bonusDef.SpawnSprite);
                newEntry.SetActive(true);
            }
        }

        /// <summary>
        /// Refresh the Brick Data panel
        /// </summary>
        private void RefreshBrickData()
        {
            // Clear down brick data
            foreach (Transform childTransform in brickContainer.transform)
            {
                Destroy(childTransform.gameObject);
            }

            // Recreate them
            foreach (BrickTypeData.BrickDef brickDef in brickTypeData.BrickTypes)
            {
                GameObject newEntry = Instantiate(brickTemplateGameObject);
                newEntry.transform.SetParent(brickContainer.transform);
                newEntry.transform.localScale = new Vector3(1, 1, 1);
                newEntry.transform.localPosition = new Vector3(0, 0, 0);
                newEntry.GetComponent<NameImageTemplate>().SetEntryContent(brickDef.FriendlyName, brickDef.Description, brickDef.BrickSpawnSprite);
                newEntry.SetActive(true);
            }
        }
    }
}
