using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Project.Scripts.Managers;
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
        private int _rowIndex;
        private int _columnIndex;
        private float _blockDestroyDuration;
        private Ease _blockDestroyEase;
        private float _blockMovementDuration;
        private Ease _blockMovementEase;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _blockDestroyDuration = GameManager.Instance.blockDestroyDuration;
            _blockDestroyEase = GameManager.Instance.blockDestroyEase;
            _blockMovementDuration = GameManager.Instance.blockMovementDuration;
            _blockMovementEase = GameManager.Instance.blockMovementEase;
        }
        
        public void SetCurrentBlockGroup(List<Block> blockGroup)
        {
            _currentBlockGroup = blockGroup;
        }

        public void SetBlockIndex(int rowIndex, int columnIndex)
        {
            _rowIndex = rowIndex;
            _columnIndex = columnIndex;
        }

        public async void OnHit()
        {
            if (_currentBlockGroup.Count == 1) return;
            GameManager.Instance.IsBlockInProcess = true;
            var destroyProcesses = new Task[_currentBlockGroup.Count];
            for (int i = 0; i < _currentBlockGroup.Count; i++)
            {
                destroyProcesses[i] = _currentBlockGroup[i].DestroyBlockRoutine();
            }
            await Task.WhenAll(destroyProcesses);
            EventBus.OnAfterBlockDestroy?.Invoke();
        }

        private async Task DestroyBlockRoutine()
        {
            EventBus.OnBlockDestroy?.Invoke(_rowIndex,_columnIndex);
            await transform.DOScale(Vector3.zero, _blockDestroyDuration).
                SetEase(_blockDestroyEase).AsyncWaitForCompletion();
            Destroy(gameObject);
        }

        public void MoveBlock(Vector3 targetPosition)
        {
            transform.DOMove(targetPosition, _blockMovementDuration).SetEase(_blockMovementEase);
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
