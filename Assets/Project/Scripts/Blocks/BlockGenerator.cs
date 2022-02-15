using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project.Scripts.Managers;
using Project.Scripts.SettingsObjects;
using Project.Scripts.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project.Scripts.Blocks
{
    public class BlockGenerator : MonoBehaviour
    {
        [Title("Block Prefabs")]
        [SerializeField] private List<GameObject> blockPrefabs;
        [SerializeField] private BlockCalculator blockCalculator;

        [Title("Block Generator Settings")]
        [SerializeField] private BlockGeneratorSettings blockGeneratorSettings;

        [Title("Game Settings")]
        [SerializeField] private GameSettings gameSettings;

        private const int MaxColorCount = 6;

        private List<GameObject> _gameBlockPrefabs;
        private Block[,] _gameBlocks;
        private readonly List<(int rowIndex, int columnIndex)> _destroyedBlockIndices =
            new List<(int rawIndex, int columnIndex)>();
        private readonly List<Block> _newGeneratedBlocks = new List<Block>();

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
            _destroyedBlockIndices.Add((rowIndex, columnIndex));
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
            _gameBlocks = new Block[gameSettings.rowCount, gameSettings.columnCount];
            _gameBlockPrefabs = new List<GameObject>(blockPrefabs);
            _gameBlockPrefabs.ShuffleList();
            for (int i = 0; i < MaxColorCount - gameSettings.colorCount; i++)
            {
                _gameBlockPrefabs.RemoveAt(0);
            }
        }

        private void InitializeGame()
        {
            for (int i = 0; i < gameSettings.rowCount; i++)
            {
                for (int j = 0; j < gameSettings.columnCount; j++)
                {
                    // initialize blocks around center (0,0) point
                    GameObject newBlockObject = GetRandomBlockObject();
                    newBlockObject.transform.position = InitVerticalPosition(i) + InitHorizontalPosition(j);

                    // give block an index according to (0,0) point is bottom left
                    int rowIndex = ReturnReferenceIndex(i, gameSettings.rowCount);
                    int columnIndex = ReturnReferenceIndex(j, gameSettings.columnCount);
                    Block newBlock = newBlockObject.GetComponent<Block>();
                    newBlock.SetBlockIndex(rowIndex, columnIndex);
                    _gameBlocks[rowIndex, columnIndex] = newBlock;
                }
            }
        }

        private async void GenerateNewBlocks()
        {
            var columnsOfBlocks = new List<List<(int rowIndex, int columnIndex)>>();
            var sameColumnBlockIndices = new List<(int rowIndex, int columnIndex)>();

            // group destroyed blocks by their column
            foreach (var destroyedBlockIndex in _destroyedBlockIndices.OrderBy(index => index.columnIndex))
            {
                if (sameColumnBlockIndices.Count != 0 &&
                    sameColumnBlockIndices[sameColumnBlockIndices.Count - 1].columnIndex !=
                    destroyedBlockIndex.columnIndex)
                {
                    columnsOfBlocks.Add(new List<(int rowIndex, int columnIndex)>(sameColumnBlockIndices));
                    sameColumnBlockIndices.Clear();
                }

                sameColumnBlockIndices.Add(destroyedBlockIndex);
            }

            if (sameColumnBlockIndices.Count != 0)
                columnsOfBlocks.Add(new List<(int rowIndex, int columnIndex)>(sameColumnBlockIndices));

            // and generate new ones according to the 
            // number of destroyed blocks on each column.
            // and drop current stacks
            foreach (var sameColumnBlocks in columnsOfBlocks)
            {
                List<int> destroyedBlockRowIndices = new List<int>();
                for (int i = 0; i < sameColumnBlocks.Count; i++)
                {
                    GameObject newGeneratedBlockObject = GetRandomBlockObject();
                    var verticalGeneratePosition = ((gameSettings.rowCount / 2) + (2 + i) - 0.5f) *
                                                   blockGeneratorSettings.verticalScaleFactor * Vector3.up;
                    var horizontalGeneratePosition = (-(gameSettings.columnCount / 2) + sameColumnBlocks[i].columnIndex + 0.5f) *
                                                     blockGeneratorSettings.horizontalScaleFactor * Vector3.right;
                    newGeneratedBlockObject.transform.position = verticalGeneratePosition + horizontalGeneratePosition;
                    destroyedBlockRowIndices.Add(sameColumnBlocks[i].rowIndex);
                    Block newGeneratedBlock = newGeneratedBlockObject.GetComponent<Block>();
                    newGeneratedBlock.IncreaseDropCount(sameColumnBlocks.Count);
                    newGeneratedBlock.SetBlockIndex(gameSettings.rowCount + i, sameColumnBlocks[0].columnIndex);
                    newGeneratedBlock.IsNewGenerated = true;
                    _newGeneratedBlocks.Add(newGeneratedBlock);
                }

                SetDropCounts(sameColumnBlocks[0].columnIndex, destroyedBlockRowIndices);
            }

            // wait for replacement to end and recalculate blocks
            Block.MoveTasks.Clear();
            EventBus.OnAfterBlockGeneration?.Invoke();
            await Task.WhenAll(Block.MoveTasks);
            ReassignBlocksAfterDrop();
            _destroyedBlockIndices.Clear();
        }

        private void ReassignBlocksAfterDrop()
        {
            for (int rowIndex = 0; rowIndex < gameSettings.rowCount; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < gameSettings.columnCount; columnIndex++)
                {
                    if (_gameBlocks[rowIndex, columnIndex] == null) continue;
                    int blockDropCount = _gameBlocks[rowIndex, columnIndex].DropCount;
                    if (blockDropCount == 0) continue;
                    _gameBlocks[rowIndex - blockDropCount, columnIndex] =
                        _gameBlocks[rowIndex, columnIndex];
                    _gameBlocks[rowIndex - blockDropCount, columnIndex]
                        .SetBlockIndex(rowIndex - blockDropCount, columnIndex);
                }
            }

            foreach (var newGeneratedBlock in _newGeneratedBlocks)
            {
                int blockDropCount = newGeneratedBlock.DropCount;
                (int rowIndex, int columnIndex) = newGeneratedBlock.GetIndex();
                _gameBlocks[rowIndex - blockDropCount, columnIndex] = newGeneratedBlock;
                newGeneratedBlock.SetBlockIndex(rowIndex - blockDropCount, columnIndex);
            }

            _newGeneratedBlocks.Clear();
            EventBus.OnAfterBlockReplacement?.Invoke();
            AssignBlocks();
        }

        private void SetDropCounts(int destroyedBlockColumnIndex, List<int> destroyedBlockRowIndices)
        {
            // for each block in this column, check if destroyed object is under this block
            // If it is, increase its drop count
            for (int i = 0; i < gameSettings.rowCount; i++)
            {
                if (_gameBlocks[i, destroyedBlockColumnIndex] != null)
                {
                    foreach (var destroyedBlockRowIndex in destroyedBlockRowIndices)
                    {
                        if (i > destroyedBlockRowIndex) _gameBlocks[i, destroyedBlockColumnIndex].IncreaseDropCount();
                    }
                }
            }
        }

        private void AssignBlocks()
        {
            blockCalculator.CalculateBlocks(_gameBlocks);
            GameManager.Instance.IsBlockInProcess = false;
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

        private Vector3 InitVerticalPosition(int index)
        {
            return ((index / 2 + index % 2) * Mathf.Pow(-1, index + 1) - 0.5f) *
                   blockGeneratorSettings.verticalScaleFactor * Vector3.up;
        }

        private Vector3 InitHorizontalPosition(int index)
        {
            return ((index / 2 + index % 2) * Mathf.Pow(-1, index + 1) - 0.5f) *
                   blockGeneratorSettings.horizontalScaleFactor * Vector3.right;
        }
    }
}