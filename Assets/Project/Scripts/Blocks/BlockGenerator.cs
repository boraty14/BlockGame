using System.Collections.Generic;
using Project.Scripts.Managers;
using Project.Scripts.Utils;
using UnityEngine;

namespace Project.Scripts.Blocks
{
    public class BlockGenerator : MonoBehaviour
    {
        [SerializeField] private List<GameObject> blockPrefabs;
        [SerializeField] private BlockCalculator blockCalculator;
        
        private const float HorizontalScaleFactor = 2.26f; // horizontal ratio of unity units/unity scale for sprites
        private const float VerticalScaleFactor = 2.56f; // vertical ratio of unity units/unity scale for sprites
        private const int MaxColorCount = 6;
        
        private int _rowCount;
        private int _columnCount;
        private int _colorCount;
        private List<GameObject> _gameBlockPrefabs;
        private Block[,] _gameBlocks;

        private void Awake()
        {
            SetGameSettings();
            InitializeGame();
            AssignBlocks();
        }
        
        private void SetGameSettings()
        {
            _rowCount = GameManager.Instance.rowCount;
            _columnCount = GameManager.Instance.columnCount;
            _gameBlocks = new Block[_rowCount,_columnCount];
            
            _colorCount = GameManager.Instance.colorCount;
            _gameBlockPrefabs = new List<GameObject>(blockPrefabs);
            _gameBlockPrefabs.ShuffleList();
            for (int i = 0; i < MaxColorCount-_colorCount; i++)
            {
                _gameBlockPrefabs.RemoveAt(0);
            }
        }

        private void InitializeGame()
        {
            for (int i = 0; i < _rowCount; i++)
            {
                for (int j = 0; j < _columnCount; j++)
                {
                    // initialize blocks around center (0,0) point
                    GameObject newBlock = Instantiate(_gameBlockPrefabs[Random.Range(0, _gameBlockPrefabs.Count)], transform);
                    var verticalPosition = ((i / 2 + i % 2) * Mathf.Pow(-1, i + 1) - 0.5f) * VerticalScaleFactor * Vector3.up;
                    var horizontalPosition = ((j / 2 + j % 2) * Mathf.Pow(-1, j + 1) - 0.5f) * HorizontalScaleFactor * Vector3.right;
                    newBlock.transform.position = verticalPosition + horizontalPosition;
                    
                    // give block an index according to (0,0) point is bottom left
                    int rowIndex = ReturnReferenceIndex(i, _rowCount);
                    int columnIndex = ReturnReferenceIndex(j, _columnCount);
                    _gameBlocks[rowIndex, columnIndex] = newBlock.GetComponent<Block>();
                }
            }
        }

        private int ReturnReferenceIndex(int index, int totalLength)
        {
            int abstractIndex = (index / 2 + index % 2) * Mathf.RoundToInt(Mathf.Pow(-1, index + 1));
            int indexOffset = (totalLength - 1) / 2;
            return abstractIndex + indexOffset;

        }

        private void AssignBlocks()
        {
            blockCalculator.CalculateBlocks();   
        }
    }
}
