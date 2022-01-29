using System.Collections.Generic;
using Project.Scripts.Managers;
using UnityEngine;

namespace Project.Scripts.Blocks
{
    public class BlockCalculator : MonoBehaviour
    {
        private int _rowCount;
        private int _columnCount;
        private int _firstLimit;
        private int _secondLimit;
        private int _thirdLimit;

        private List<(int rowIndex, int columnIndex)> _calculatedBlockIndices = new List<(int rawIndex, int columnIndex)>();
        private List<List<Block>> _blockGroups = new List<List<Block>>();
        private Block[,] _gameBlocks;

        private void Awake()
        {
            SetGameSettings();
        }

        private void SetGameSettings()
        {
            _rowCount = GameManager.Instance.rowCount;
            _columnCount = GameManager.Instance.columnCount;
            _firstLimit = GameManager.Instance.firstLimit;
            _secondLimit = GameManager.Instance.secondLimit;
            _thirdLimit = GameManager.Instance.thirdLimit;
        }

        public void CalculateBlocks(Block[,] gameBlocks)
        {
            _gameBlocks = gameBlocks;
            _calculatedBlockIndices.Clear();
            _blockGroups.Clear();
            
            for (int rowIndex = 0; rowIndex < _rowCount; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < _columnCount; columnIndex++)
                {
                    if (IsIndexCalculated(rowIndex,columnIndex)) continue;
                    List<Block> newBlockGroup = new List<Block>();
                    _blockGroups.Add(newBlockGroup);
                    CheckAroundBlock(rowIndex, columnIndex,newBlockGroup);
                }
            }

            foreach (var blockGroup in _blockGroups)
            {
                Debug.Log(blockGroup.Count);
                foreach (var block in blockGroup)
                {
                    block.SetSprite(ReturnBlockState(blockGroup.Count));
                }
            }
        }

        private void CheckAroundBlock(int rowIndex,int columnIndex, List<Block> blockGroup)
        {
            if (IsIndexCalculated(rowIndex,columnIndex)) return;
            blockGroup.Add(_gameBlocks[rowIndex,columnIndex]);
            _gameBlocks[rowIndex,columnIndex].SetCurrentBlockGroup(blockGroup);
            _calculatedBlockIndices.Add((rowIndex,columnIndex));
            
            //check for four side of current block
            if(IsIndexValid(rowIndex,columnIndex + 1))
                CheckAroundBlock(rowIndex,columnIndex,blockGroup);
            if(IsIndexValid(rowIndex,columnIndex - 1))
                CheckAroundBlock(rowIndex,columnIndex,blockGroup);
            if(IsIndexValid(rowIndex + 1,columnIndex))
                CheckAroundBlock(rowIndex,columnIndex,blockGroup);
            if(IsIndexValid(rowIndex - 1,columnIndex))
                CheckAroundBlock(rowIndex,columnIndex,blockGroup);
        }

        private BlockState ReturnBlockState(int blockCount)
        {
            if (blockCount <= _firstLimit) return BlockState.Default;
            if (blockCount <= _secondLimit) return BlockState.A;
            if (blockCount <= _thirdLimit) return BlockState.B;
            return BlockState.C;
        }

        private bool IsIndexValid(int rowIndex, int columnIndex)
        {
            return rowIndex > -1 && rowIndex < _rowCount &&
                   columnIndex > -1 && columnIndex < _columnCount;
        }

        private bool IsIndexCalculated(int rowIndex, int columnIndex)
        {
            return _calculatedBlockIndices.Contains((rowIndex, columnIndex));
        }
        
    }
}
