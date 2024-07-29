using __Game.Resources.Scripts.EventBus;
using Assets.__Game.Resources.Scripts.Tutorial;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.__Game.Resources.Scripts.Logic
{
  public class BasketTutorial : MonoBehaviour
  {
    [SerializeField] private TutorialFinger _tutorialFinger;

    private List<TreeItem> _correctItems = new List<TreeItem>();

    private Basket _basket;

    private EventBinding<EventStructs.ComponentEvent<Basket>> _basketComponentEvent;
    private EventBinding<EventStructs.SpawnedItemsEvent> _spawnedItemsEvent;
    private EventBinding<EventStructs.BasketReceivedItemEvent> _basketReceivedItemEvent;

    private void OnEnable()
    {
      _basketComponentEvent = new EventBinding<EventStructs.ComponentEvent<Basket>>(ReceiveBasketComponent);
      _spawnedItemsEvent = new EventBinding<EventStructs.SpawnedItemsEvent>(ReceiveCorrectItems);
      _basketReceivedItemEvent = new EventBinding<EventStructs.BasketReceivedItemEvent>(DisableTutorialFInger);
    }

    private void OnDisable()
    {
      _basketComponentEvent.Remove(ReceiveBasketComponent);
      _spawnedItemsEvent.Remove(ReceiveCorrectItems);
      _basketReceivedItemEvent.Remove(DisableTutorialFInger);
    }

    private void Start()
    {
      StartCoroutine(DoEnableTutorialFinger());
    }

    private void ReceiveBasketComponent(EventStructs.ComponentEvent<Basket> basketComponentEvent)
    {
      _basket = basketComponentEvent.Data;
    }

    private void ReceiveCorrectItems(EventStructs.SpawnedItemsEvent spawnedItemsEvent)
    {
      _correctItems = spawnedItemsEvent.CorrectItems;
    }

    private IEnumerator DoEnableTutorialFinger()
    {
      if (_basket.Tutorial == false) yield break;

      yield return new WaitForEndOfFrame();
      yield return new WaitForEndOfFrame();

      if (_correctItems.Count == 0) yield break;

      //_tutorialFinger.gameObject.SetActive(true);
      _tutorialFinger.EnableFinger();
      _tutorialFinger.SetMovement(_correctItems.First().transform, transform);
    }

    private void DisableTutorialFInger(EventStructs.BasketReceivedItemEvent basketReceivedItemEvent)
    {
      if (_tutorialFinger.gameObject.activeSelf == false) return;

      _tutorialFinger.StopAndDisableFinger();
    }
  }
}