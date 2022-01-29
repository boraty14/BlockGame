using System.Collections.Generic;
using Project.Scripts.Managers;
using UnityEngine;

namespace Project.Scripts.Blocks
{
    public class BlockCalculator : MonoBehaviour
    {
        private int _firstLimit;
        private int _secondLimit;
        private int _thirdLimit;

        private List<(int rowIndex, int columnIndex)> _calculatedBlocks; 

        private void Awake()
        {
            SetGameSettings();
        }

        private void SetGameSettings()
        {
            _firstLimit = GameManager.Instance.firstLimit;
            _secondLimit = GameManager.Instance.secondLimit;
            _thirdLimit = GameManager.Instance.thirdLimit;
            _calculatedBlocks = new List<(int rawIndex, int columnIndex)>();
        }

        public void CalculateBlocks()
        {
            _calculatedBlocks.Clear();
        }

        private bool CheckIfIndexIsUsed(int rowIndex, int columnIndex)
        {
            return false;
        }
        
    }
}
