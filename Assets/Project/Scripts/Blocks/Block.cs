using UnityEngine;

namespace Project.Scripts.Blocks
{
    public class Block : MonoBehaviour
    {
        [SerializeField] private Sprite defaultSprite;
        [SerializeField] private Sprite spriteA;
        [SerializeField] private Sprite spriteB;
        [SerializeField] private Sprite spriteC;
        [SerializeField] private BlockColor blockColor;

        public BlockColor GetBlockColor() => blockColor;
    }

    public enum BlockColor
    {
        Green,
        Red,
        Pink,
        Purple,
        Yellow,
        Blue
    }
}
