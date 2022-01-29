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
            _currentBlockGroup = null;
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
            _currentBlockGroup = null;
        }

        public void SetCurrentBlockGroup(List<Block> blockGroup)
        {
            _currentBlockGroup = blockGroup;
        }

        public void OnHit()
        {
            if (_currentBlockGroup == null) return;
            foreach (var currentBlock in _currentBlockGroup)
            {
                currentBlock.BlastBlock();
            }
        }

        public void BlastBlock()
        {
            
        }

        public BlockColor GetBlockColor() => blockColor;

        public void SetSprite(int statusIndex)
        {
            switch (statusIndex)
            {
                case 0:
                    _spriteRenderer.sprite = defaultSprite;
                    break;
                case 1:
                    _spriteRenderer.sprite = spriteA;
                    break;
                case 2:
                    _spriteRenderer.sprite = spriteB;
                    break;
                case 3:
                    _spriteRenderer.sprite = spriteC;
                    break;
            }
        }
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
