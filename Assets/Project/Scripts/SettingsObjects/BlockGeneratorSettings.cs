using UnityEngine;

namespace Project.Scripts.SettingsObjects
{
    [CreateAssetMenu(fileName = "BlockGeneratorSettings", menuName = "Settings Objects/Create Block Generator Settings")]
    public class BlockGeneratorSettings : ScriptableObject
    {
        public float verticalScaleFactor; // vertical ratio of unity units/unity scale for sprites
        public float horizontalScaleFactor; // horizontal ratio of unity units/unity scale for sprites
    }
}
