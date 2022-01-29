using System;
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

        private readonly List<(int rowIndex, int columnIndex)> _calculatedBlockIndices = new List<(int rawIndex, int columnIndex)>();
        private readonly List<List<Block>> _blockGroups = new List<List<Block>>();
        private Block[,] _gameBlocks;

        private void Awake()
        {
            SetGameSettings();
        }
        
        private void SetGameSettings()
        {
            Debug.Log(GameManager.Instance.rowCount);
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
                    if (IsIndexCalculated(rowIndex, columnIndex)) continue;
                    CreateBlockGroup(rowIndex, columnIndex);
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

        private void CreateBlockGroup(int rowIndex,int columnIndex)
        {
            List<Block> newBlockGroup = new List<Block>();
            AddBlockToGroup(rowIndex,columnIndex,newBlockGroup);
        }

        private void AddBlockToGroup(int rowIndex, int columnIndex, List<Block> blockGroup)
        {
            Block newBlock = _gameBlocks[rowIndex, columnIndex];
            blockGroup.Add(newBlock);
            newBlock.SetCurrentBlockGroup(blockGroup);
            _calculatedBlockIndices.Add((rowIndex,columnIndex));
            _blockGroups.Add(blockGroup);
            CheckAroundBlock(rowIndex, columnIndex ,blockGroup,newBlock.GetBlockColor());
        }

        private void CheckAroundBlock(int rowIndex,int columnIndex, List<Block> blockGroup, BlockColor groupColor)
        {
            //check for four side of current block
            for (int i = -1; i < 2; i+=2)
            {
                if(IsIndexValid(rowIndex,columnIndex + i))
                    if(!IsIndexCalculated(rowIndex,columnIndex +i))
                        if (IsSameColor(rowIndex, columnIndex + i, groupColor))
                            AddBlockToGroup(rowIndex, columnIndex + i, blockGroup);
            }
            
            for (int i = -1; i < 2; i+=2)
            {
                if(IsIndexValid(rowIndex + i,columnIndex))
                    if(!IsIndexCalculated(rowIndex +i,columnIndex))
                        if(IsSameColor(rowIndex + i,columnIndex,groupColor))
                            AddBlockToGroup(rowIndex + i,columnIndex, blockGroup);
            }
        }

        private BlockState ReturnBlockState(int blockCount)
        {
            if (blockCount <= _firstLimit) return BlockState.Default;
            if (blockCount <= _secondLimit) return BlockState.A;
            if (blockCount <= _thirdLimit) return BlockState.B;
            return BlockState.C;
        }

        private bool IsSameColor(int rowIndex, int columnIndex, BlockColor groupColor)
        {
            return _gameBlocks[rowIndex, columnIndex].GetBlockColor() == groupColor;
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
