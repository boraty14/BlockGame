using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Project.Scripts.Utils
{
  [RequireComponent(typeof (Image))]
  public class InputPanel : MonoBehaviourSingleton<InputPanel>,
    IDragHandler,
    IEventSystemHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerExitHandler
  {
    [Space(16f)]
    [Title("Full Info Events")]
    public UnityEvent<PointerEventData> OnDragFullInfo = new UnityEvent<PointerEventData>();
    public UnityEvent<PointerEventData> OnPointerDownFullInfo = new UnityEvent<PointerEventData>();
    public UnityEvent<PointerEventData> OnPointerUpFullInfo = new UnityEvent<PointerEventData>();
    public UnityEvent<PointerEventData> OnPointerExitFullInfo = new UnityEvent<PointerEventData>();
    [Space(16f)]
    [Title("Delta Event")]
    public UnityEvent<Vector2> OnDragDelta = new UnityEvent<Vector2>();
    [Space(16f)]
    [Title("Position Events")]
    public UnityEvent<Vector2> OnDragPosition = new UnityEvent<Vector2>();
    public UnityEvent<Vector2> OnPointDownPosition = new UnityEvent<Vector2>();
    public UnityEvent<Vector2> OnPointerUpPosition = new UnityEvent<Vector2>();
    public UnityEvent<Vector2> OnPointerExitPosition = new UnityEvent<Vector2>();
    [Space(16f)]
    [Title("Pointer Events")]
    public UnityEvent OnPointerDownEvent = new UnityEvent();
    public UnityEvent OnPointerUpEvent = new UnityEvent();
    public UnityEvent OnPointerExitEvent = new UnityEvent();
    private Image inputImage;

    private void Awake() => this.inputImage = this.GetComponent<Image>();

    public void OnDrag(PointerEventData eventData)
    {
      this.OnDragDelta?.Invoke(eventData.delta * (1536f / (float) Screen.width));
      this.OnDragPosition?.Invoke(eventData.position * (1536f / (float) Screen.width));
      this.OnDragFullInfo?.Invoke(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
      this.OnPointDownPosition?.Invoke(eventData.position * (1536f / (float) Screen.width));
      this.OnPointerDownEvent?.Invoke();
      this.OnPointerDownFullInfo?.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
      this.OnPointerUpPosition?.Invoke(eventData.position * (1536f / (float) Screen.width));
      this.OnPointerUpEvent.Invoke();
      this.OnPointerUpFullInfo?.Invoke(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      this.OnPointerExitPosition?.Invoke(eventData.position * (1536f / (float) Screen.width));
      this.OnPointerExitEvent?.Invoke();
      this.OnPointerExitFullInfo?.Invoke(eventData);
    }

    public void Enable() => this.inputImage.enabled = true;

    public void Disable() => this.inputImage.enabled = false;
  }
}