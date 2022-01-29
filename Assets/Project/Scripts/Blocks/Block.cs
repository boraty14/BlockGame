using System;
using System.Collections.Generic;
using Project.Scripts.Utils;
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

        private SpriteRenderer _spriteRenderer;
        private List<Block> _currentBlockGroup;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            EventBus.OnBlockCalculate += OnBlockCalculate;
        }

        private void OnDisable()
        {
            EventBus.OnBlockCalculate -= OnBlockCalculate;
        }

        private void OnBlockCalculate()
        {
            
        }

        public void SetCurrentBlockGroup(List<Block> blockGroup)
        {
            _currentBlockGroup = blockGroup;
        }

        public void OnHit()
        {
            foreach (var currentBlock in _currentBlockGroup)
            {
                currentBlock.BlastBlock();
            }
        }

        public void BlastBlock()
        {
            
        }

        public BlockColor GetBlockColor() => blockColor;

        public void SetSprite(BlockState blockState)
        {
            switch (blockState)
            {
                case BlockState.Default:
                    _spriteRenderer.sprite = defaultSprite;
                    break;
                case BlockState.A:
                    _spriteRenderer.sprite = spriteA;
                    break;
                case BlockState.B:
                    _spriteRenderer.sprite = spriteB;
                    break;
                case BlockState.C:
                    _spriteRenderer.sprite = spriteC;
                    break;
            }
        }
    }

    public enum BlockState
    {
        Default,
        A,
        B,
        C
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
