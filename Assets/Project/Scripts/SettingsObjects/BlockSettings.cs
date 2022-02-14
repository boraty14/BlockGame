using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Project.Scripts.SettingsObjects
{
    [CreateAssetMenu(fileName = "BlockSettings", menuName = "Settings Objects/Create Block Settings")]
    public class BlockSettings : ScriptableObject
    {
        [Title("Block Destroy Settings")]
        public float blockDestroyDuration;
        public Ease blockDestroyEase;

        [Title("Block Movement Settings")]
        public float blockMovementDuration;
        public Ease blockMovementEase;
    }
}