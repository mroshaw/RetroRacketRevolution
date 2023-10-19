using DaftApplesGames.RetroRacketRevolution.Bonuses;
using System;
using Sirenix.OdinInspector;
using UnityEngine;
using static DaftApplesGames.RetroRacketRevolution.Bonuses.BonusManager;
using System.Collections.Generic;
using System.Linq;

namespace DaftApplesGames.RetroRacketRevolution
{
    public enum AddOnType { None, LaserCannon, Catcher, LevelBooster }

    public class AddOnManager : MonoBehaviour
    {
        [BoxGroup("Settings")] public AddOnItem[] addonItemArray;

        [SerializeField]
        private Dictionary<AddOnType, AddOnItem> _addOnDict = new Dictionary<AddOnType, AddOnItem>();

        /// <summary>
        /// Initialise the AddOn Manager
        /// </summary>
        private void Awake()
        {
            _addOnDict = addonItemArray.ToDictionary(x => x.AddOnType);
        }

        /// <summary>
        /// Spawn an AddOn
        /// </summary>
        /// <returns></returns>
        public AddOn SpawnAddOn(AddOnType addOnType)
        {
            GameObject addOnGameObject = Instantiate(_addOnDict[addOnType].Prefab);
            AddOn addOn = addOnGameObject.GetComponent<AddOn>();
            return addOn;
        }

        /// <summary>
        /// Bonus type and prefab pairs
        /// </summary>
        [Serializable]
        public class AddOnItem
        {
            public AddOnType AddOnType;
            public GameObject Prefab;
            public GameObject ProjectileContainer;
        }
    }
}
