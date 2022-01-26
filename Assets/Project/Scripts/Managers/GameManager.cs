using System;
using Project.Scripts.Utils;
using UnityEngine;

namespace Project.Scripts.Managers
{
    public class GameManager : MonoBehaviourSingleton<GameManager>
    {
        public bool IsBlasting { get; set; }
        private void Awake()
        {
            Application.targetFrameRate = 60;
            IsBlasting = false;
        }
    }
}
