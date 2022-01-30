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

        public static readonly List<Task> MoveTasks = new List<Task>();

        public bool IsNewGenerated { get; set; }
        
        public int DropCount { get; private set; }

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _blockDestroyDuration = GameManager.Instance.blockDestroyDuration;
            _blockDestroyEase = GameManager.Instance.blockDestroyEase;
            _blockMovementDuration = GameManager.Instance.blockMovementDuration;
            _blockMovementEase = GameManager.Instance.blockMovementEase;
            DropCount = 0;
            IsNewGenerated = false;
        }

        private void OnEnable()
        {
            EventBus.OnAfterBlockGeneration += OnAfterBlockGeneration;
            EventBus.OnAfterBlockReplacement += OnAfterBlockReplacement;
        }
        
        private void OnDisable()
        {
            EventBus.OnAfterBlockGeneration -= OnAfterBlockGeneration;
            EventBus.OnAfterBlockReplacement -= OnAfterBlockReplacement;
        }

        private void OnAfterBlockGeneration()
        {
            if (DropCount == 0) return;
            Vector3 targetPosition =
                transform.position + (DropCount * BlockGenerator.VerticalScaleFactor * Vector3.down);
            if (IsNewGenerated)
            {
                targetPosition += BlockGenerator.VerticalScaleFactor * Vector3.down;
                IsNewGenerated = false;
            }
            MoveTasks.Add(MoveBlock(targetPosition));
        }

        private void OnAfterBlockReplacement()
        {
            DropCount = 0;
        }

        public void IncreaseDropCount(int increaseAmount = 1) => DropCount += increaseAmount;

        public (int rowIndex, int columnIndex) GetIndex() => (_rowIndex, _columnIndex);

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

        private async Task MoveBlock(Vector3 targetPosition)
        {
            await transform.DOMove(targetPosition, _blockMovementDuration).SetEase(_blockMovementEase).AsyncWaitForCompletion();
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
}
