using System.Collections.Generic;
using Project.Scripts.Managers;
using Project.Scripts.Utils;
using UnityEngine;

namespace Project.Scripts.Blocks
{
    public class BlockGenerator : MonoBehaviour
    {
        [SerializeField] private List<GameObject> blockPrefabs;
        
        private const float HorizontalScaleFactor = 2.2f; // horizontal ratio of unity units/unity scale for sprites
        private const float VerticalScaleFactor = 2.5f; // vertical ratio of unity units/unity scale for sprites
        private const int MaxColorCount = 6;
        
        private int _rowCount;
        private int _columnCount;
        private int _colorCount;
        private List<GameObject> _gameBlockPrefabs;

        private void Awake()
        {
            SetGameSettings();
            InitializeGame();
        }

        private void SetGameSettings()
        {
            _rowCount = GameManager.Instance.rowCount;
            _columnCount = GameManager.Instance.columnCount;
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
                    GameObject newBlock = Instantiate(_gameBlockPrefabs[Random.Range(0, _gameBlockPrefabs.Count)], transform);
                    var verticalPosition = ((i / 2 + i % 2) * Mathf.Pow(-1, i + 1) - 0.5f) * VerticalScaleFactor * Vector3.up;
                    var horizontalPosition = ((j / 2 + j % 2) * Mathf.Pow(-1, j + 1) - 0.5f) * HorizontalScaleFactor * Vector3.right;
                    newBlock.transform.position = verticalPosition + horizontalPosition;
                }
            }
        }
    }
}
