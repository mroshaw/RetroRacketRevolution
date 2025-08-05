using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SpaceGraphicsToolkit.Backdrop;
using SpaceGraphicsToolkit.Ring;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DaftAppleGames.RetroRacketRevolution.Levels
{
    public class BackdropGenerator : MonoBehaviour
    {
        [BoxGroup("Settings")] [SerializeField] private GameObject backdropGameObject;
        [BoxGroup("Stars")] [SerializeField] private StarsBackdropElement starBackdrop1;
        [BoxGroup("Stars")] [SerializeField] private StarsBackdropElement starBackdrop2;
        [BoxGroup("Accretion Disk")] [SerializeField] private AccretionDiskBackdropElement accretionDisk;

        [Serializable] private abstract class BackdropElement
        {
            [BoxGroup("Settings")] [SerializeField] private float likelihoodToAppear;
            [BoxGroup("Settings")] [SerializeField] private List<Color> possibleColors;
        }

        [Serializable] private class StarsBackdropElement : BackdropElement
        {
            [BoxGroup("Settings")] [SerializeField] private SgtBackdrop backdrop;
            [BoxGroup("Settings")] [SerializeField] private int minimumStars;
            [BoxGroup("Settings")] [SerializeField] private int maximumStars;
        }

        [Serializable] private class AccretionDiskBackdropElement : BackdropElement
        {
            [BoxGroup("Settings")] [SerializeField] private SgtRing ring;
        }

        private int GetRandomSeed()
        {
            return Random.Range(-999999999, 999999999);
        }
    }
}