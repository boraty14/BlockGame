using System;
using Project.Scripts.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Scripts.Managers
{
    public class GameManager : MonoBehaviourSingleton<GameManager>
    {
        [LabelText("Total number of rows (M)")][Range(2,10)] public int rowCount;
        [LabelText("Total number of columns (N)")][Range(2,10)] public int columnCount;
        [LabelText("Total number of colors (K)")][Range(1,6)] public int colorCount;
        [LabelText("First Limit (A)")] public int firstLimit;
        [LabelText("Second Limit (B)")] public int secondLimit;
        [LabelText("Third Limit (C)")] public int thirdLimit;
        
        public bool IsBlasting { get; set; }
        private void Awake()
        {
            Application.targetFrameRate = 60;
            IsBlasting = false;
        }
    }
}
