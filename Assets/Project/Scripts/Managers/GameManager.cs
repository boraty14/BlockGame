using DG.Tweening;
using Project.Scripts.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Scripts.Managers
{
    [DefaultExecutionOrder(-1)]
    public class GameManager : MonoBehaviourSingleton<GameManager>
    {
        

        
        public bool IsBlockInProcess { get; set; }
        private void Awake()
        {
            Application.targetFrameRate = 60;
            IsBlockInProcess = false;
        }
    }
}
