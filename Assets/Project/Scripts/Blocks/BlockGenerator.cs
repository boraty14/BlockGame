using System;
using System.Collections.Generic;
using System.Linq;
using Project.Scripts.Managers;
using Project.Scripts.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

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
        private readonly List<(int rowIndex, int columnIndex)> _destroyedBlockIndices = new List<(int rawIndex, int columnIndex)>();
        private float _generateHeightOffset;

        private void Awake()
        {
            SetGameSettings();
            InitializeGame();
        }

        private void OnEnable()
        {
            EventBus.OnBlockDestroy += OnBlockDestroy;
            EventBus.OnAfterBlockDestroy += OnAfterBlockDestroy;
        }

        private void OnDisable()
        {
            EventBus.OnBlockDestroy -= OnBlockDestroy;
            EventBus.OnAfterBlockDestroy -= OnAfterBlockDestroy;
        }

        private void OnBlockDestroy(int rowIndex, int columnIndex)
        {
            _destroyedBlockIndices.Add((rowIndex,columnIndex));
        }

        private void OnAfterBlockDestroy()
        {
            GenerateNewBlocks();
        }

        private void Start()
        {
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
                    GameObject newBlockObject = GetRandomBlockObject();
                    newBlockObject.transform.position = InitVerticalPosition(i) + InitHorizontalPosition(j);

                    // give block an index according to (0,0) point is bottom left
                    int rowIndex = ReturnReferenceIndex(i, _rowCount);
                    int columnIndex = ReturnReferenceIndex(j, _columnCount);
                    Block newBlock = newBlockObject.GetComponent<Block>();
                    newBlock.SetBlockIndex(rowIndex,columnIndex);
                    _gameBlocks[rowIndex, columnIndex] = newBlock;
                }
            }
        }

        private void GenerateNewBlocks()
        {
            //group destroyed blocks by their column and generate new ones according to the 
            //number of destroyed blocks on each column.
            
            var columnsOfBlocks = new List<List<(int rowIndex, int columnIndex)>>();
            var sameColumnBlockIndices = new List<(int rowIndex, int columnIndex)>();
            Debug.Log("destroyed total " + _destroyedBlockIndices.Count);
            foreach (var destroyedBlockIndex in _destroyedBlockIndices.OrderBy(index => index.columnIndex))
            {
                if (sameColumnBlockIndices.Count != 0 && 
                    sameColumnBlockIndices[sameColumnBlockIndices.Count - 1].columnIndex != destroyedBlockIndex.columnIndex)
                {
                    columnsOfBlocks.Add(new List<(int rowIndex, int columnIndex)>(sameColumnBlockIndices));
                    sameColumnBlockIndices.Clear();
                }
                sameColumnBlockIndices.Add(destroyedBlockIndex);
            }
            if(sameColumnBlockIndices.Count != 0) columnsOfBlocks.Add(new List<(int rowIndex, int columnIndex)>(sameColumnBlockIndices));
            
            Debug.Log("total columns " + columnsOfBlocks.Count);
            foreach (var sameColumnBlocks in columnsOfBlocks)
            {
                for (int i = 0; i < sameColumnBlocks.Count; i++)
                {
                    GameObject newBlockObject = GetRandomBlockObject();
                    var verticalGeneratePosition = ((_rowCount / 2) + (2 + i) - 0.5f) * VerticalScaleFactor * Vector3.up;
                    var horizontalGeneratePosition = (-(_columnCount / 2) + sameColumnBlocks[i].columnIndex + 0.5f) *
                                                     HorizontalScaleFactor * Vector3.right;
                    newBlockObject.transform.position = verticalGeneratePosition + horizontalGeneratePosition;
                }    
            }
            
            _destroyedBlockIndices.Clear();
        }
        
        private void AssignBlocks()
        {
            blockCalculator.CalculateBlocks(_gameBlocks);   
        }

        private GameObject GetRandomBlockObject()
        {
            return Instantiate(_gameBlockPrefabs[Random.Range(0, _gameBlockPrefabs.Count)], transform);
        }
        
        private static int ReturnReferenceIndex(int index, int totalLength)
        {
            int abstractIndex = (index / 2 + index % 2) * Mathf.RoundToInt(Mathf.Pow(-1, index + 1));
            int indexOffset = (totalLength - 1) / 2;
            return abstractIndex + indexOffset;
        }

        private static Vector3 InitVerticalPosition(int index)
        {
            return ((index / 2 + index % 2) * Mathf.Pow(-1, index + 1) - 0.5f) * VerticalScaleFactor * Vector3.up;
        }

        private static Vector3 InitHorizontalPosition(int index)
        {
            return ((index / 2 + index % 2) * Mathf.Pow(-1, index + 1) - 0.5f) * HorizontalScaleFactor * Vector3.right;
        }
    }
}
