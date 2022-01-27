using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project.Scripts.Utils
{
  [RequireComponent(typeof(Image))]
    public class InputPanel : MonoBehaviourSingleton<InputPanel>, IDragHandler, IPointerDownHandler, IPointerUpHandler,
        IPointerExitHandler
    {
        /// <summary>
        /// Called when OnDrag callback fires
        /// Includes PointerEventData object
        /// </summary>
        [Space(16), Title("Full Info Events")]
        public PointerEventDataEvent OnDragFullInfo = new PointerEventDataEvent();

        /// <summary>
        /// Called when OnPointerDown callback fires
        /// Includes PointerEventData object
        /// </summary>
        public PointerEventDataEvent OnPointerDownFullInfo = new PointerEventDataEvent();

        /// <summary>
        /// Called when OnPointerUp callback fires
        /// Includes PointerEventData object
        /// </summary>
        public PointerEventDataEvent OnPointerUpFullInfo = new PointerEventDataEvent();

        /// <summary>
        /// Called when OnPointerExit callback fires
        /// Includes PointerEventData object
        /// </summary>
        public PointerEventDataEvent OnPointerExitFullInfo = new PointerEventDataEvent();

        /// <summary>
        /// Called when OnDrag callback fires
        /// Includes Vector2 object as pointer delta 
        /// </summary>
        [Space(16), Title("Delta Event")]
        public PositionEvent OnDragDelta = new PositionEvent();

        /// <summary>
        /// Called when OnDrag callback fires
        /// Includes Vector2 object as pointer's current position 
        /// </summary>
        [Space(16), Title("Position Events")]
        public PositionEvent OnDragPosition = new PositionEvent();

        /// <summary>
        /// Called when OnPointerDown callback fires
        /// Includes Vector2 object as pointer's current position 
        /// </summary>
        public PositionEvent OnPointDownPosition = new PositionEvent();

        /// <summary>
        /// Called when OnPointerUp callback fires
        /// Includes Vector2 object as pointer's current position 
        /// </summary>
        public PositionEvent OnPointerUpPosition = new PositionEvent();

        /// <summary>
        /// Called when OnPointerExit callback fires
        /// include Vector2 object as pointer's current position 
        /// </summary>
        public PositionEvent OnPointerExitPosition = new PositionEvent();

        /// <summary>
        /// Called when OnPointerDown callback fires
        /// </summary>
        [Space(16), Title("Pointer Events")]
        public EmptyEvent OnPointerDownEvent = new EmptyEvent();

        /// <summary>
        /// Called when OnPointerUp callback fires
        /// </summary>
        public EmptyEvent OnPointerUpEvent = new EmptyEvent();

        /// <summary>
        /// Called when OnPointerExit callback fires
        /// </summary>
        public EmptyEvent OnPointerExitEvent = new EmptyEvent();

        private Image inputImage = null;

        private void Awake()
        {
            inputImage = GetComponent<Image>();
        }

        /// <summary>
        /// UnityEngine-Event callback method DO NOT call Manually
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData)
        {
            OnDragDelta?.Invoke(eventData.delta * (1536.0f / Screen.width));
            OnDragPosition?.Invoke(eventData.position * (1536.0f / Screen.width));
            OnDragFullInfo?.Invoke(eventData);
        }

        /// <summary>
        /// UnityEngine-Event callback method DO NOT call Manually
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerDown(PointerEventData eventData)
        {
            OnPointDownPosition?.Invoke(eventData.position * (1536.0f / Screen.width));
            OnPointerDownEvent?.Invoke();
            OnPointerDownFullInfo?.Invoke(eventData);
        }

        /// <summary>
        /// UnityEngine-Event callback method DO NOT call Manually
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerUp(PointerEventData eventData)
        {
            OnPointerUpPosition?.Invoke(eventData.position * (1536.0f / Screen.width));
            OnPointerUpEvent.Invoke();
            OnPointerUpFullInfo?.Invoke(eventData);
        }

        /// <summary>
        /// UnityEngine-Event callback method DO NOT call Manually
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerExit(PointerEventData eventData)
        {
            OnPointerExitPosition?.Invoke(eventData.position * (1536.0f / Screen.width));
            OnPointerExitEvent?.Invoke();
            OnPointerExitFullInfo?.Invoke(eventData);
        }

        public void Enable()
        {
            inputImage.enabled = true;
        }

        public void Disable()
        {
            inputImage.enabled = false;
        }
    }

    [System.Serializable]
    public class PositionEvent : UnityEvent<Vector2>
    {
    }

    [System.Serializable]
    public class PointerEventDataEvent : UnityEvent<PointerEventData>
    {
    }

    [System.Serializable]
    public class EmptyEvent : UnityEvent
    {
    }
}