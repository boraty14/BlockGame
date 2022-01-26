using Project.Scripts.Utils;
using UnityEngine;

namespace Project.Scripts.Managers
{
    public class InputHandler : MonoBehaviour
    {
        [SerializeField] private LayerMask blockLayer;
        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void OnEnable()
        {
            InputPanel.Instance.OnPointerDownEvent.AddListener(OnPointerDown);
        }

        private void OnDisable()
        {
            InputPanel.Instance.OnPointerDownEvent.RemoveListener(OnPointerDown);
        }

        private void OnPointerDown()
        {
            if (GameManager.Instance.IsBlasting) return;
            Ray cameraRay = _mainCamera.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(cameraRay, out RaycastHit hit, Mathf.Infinity,blockLayer.value))
            {
                
            }
        }
    }
}
