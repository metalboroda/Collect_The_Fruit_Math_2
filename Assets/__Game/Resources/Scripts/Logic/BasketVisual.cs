using __Game.Resources.Scripts.EventBus;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Assets.__Game.Resources.Scripts.Logic
{
  public class BasketVisual : MonoBehaviour
  {
    [Header("UI")]
    [SerializeField] private string _basketNumberText;
    [SerializeField] private TextMeshProUGUI _basketNumberTextMesh;
    [Header("Effects")]
    [SerializeField] private DOTweenAnimation _doTweenAnimation;

    private EventBinding<EventStructs.BasketReceivedItemEvent> _basketReceivedItemEvent;

    private void OnEnable()
    {
      _basketReceivedItemEvent = new EventBinding<EventStructs.BasketReceivedItemEvent>(PlayPunchANimation);
    }

    private void OnDisable()
    {
      _basketReceivedItemEvent.Remove(PlayPunchANimation);
    }

    private void Start()
    {
      _basketNumberTextMesh.text = _basketNumberText;
    }

    private void PlayPunchANimation(EventStructs.BasketReceivedItemEvent basketReceivedItemEvent)
    {
      if (basketReceivedItemEvent.Id != transform.GetInstanceID()) return;

      _doTweenAnimation.DORestart();
    }
  }
}