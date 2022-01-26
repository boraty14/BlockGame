using System;
using Project.Scripts.Managers;
using UnityEngine;

namespace Project.Scripts.Core
{
    public class InputHandler : MonoBehaviour
    {
        [SerializeField] private LayerMask blockLayer;
        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void FixedUpdate()
        {
            if (GameManager.Instance.IsBlasting) return;
            Ray cameraRay = _mainCamera.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(cameraRay, out RaycastHit hit, Mathf.Infinity,blockLayer.value))
            {
                
            }
        }
    }
}
