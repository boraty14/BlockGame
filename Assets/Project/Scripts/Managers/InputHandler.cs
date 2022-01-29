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
            InputPanel.Instance.OnPointerDownEvent.AddListener(OnPointerDown);
        }

        private void OnPointerDown()
        {
            if (GameManager.Instance.IsBlasting) return;
            
            var mousePosition = Input.mousePosition;
            mousePosition.z = Mathf.Abs(_mainCamera.transform.position.z);
            var screenPos = _mainCamera.ScreenToWorldPoint(mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(screenPos,Vector2.zero);
            if (hit)
            {
                Debug.Log(hit.transform.name);
            }
        }
    }
}
