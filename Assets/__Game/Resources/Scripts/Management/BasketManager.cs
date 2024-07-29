using __Game.Resources.Scripts.EventBus;
using Assets.__Game.Resources.Scripts.Game.States;
using Assets.__Game.Resources.Scripts.Logic;
using Assets.__Game.Scripts.Infrastructure;
using UnityEngine;

namespace Assets.__Game.Resources.Scripts.Management
{
  public class BasketManager : MonoBehaviour
  {
    [SerializeField] private Basket[] _baskets;

    private EventBinding<EventStructs.BasketReceivedItemEvent> _basketReceivedItemEvent;
    private EventBinding<EventStructs.OutOfCorrectItemsEvent> _outOfCorrectItemsEvent;
    private EventBinding<EventStructs.WrongBasketEvent> _wrongBasketEvent;

    private GameBootstrapper _gameBootstrapper;

    private void Awake()
    {
      _gameBootstrapper = GameBootstrapper.Instance;
    }

    private void OnEnable()
    {
      _basketReceivedItemEvent = new EventBinding<EventStructs.BasketReceivedItemEvent>(OnBasketReceivedItem);
      _outOfCorrectItemsEvent = new EventBinding<EventStructs.OutOfCorrectItemsEvent>(OnOutOfCorrectItems);
      _wrongBasketEvent = new EventBinding<EventStructs.WrongBasketEvent>(OnWrongBasketItem);
    }

    private void OnDisable()
    {
      _basketReceivedItemEvent.Remove(OnBasketReceivedItem);
      _outOfCorrectItemsEvent.Remove(OnOutOfCorrectItems);
      _wrongBasketEvent.Remove(OnWrongBasketItem);
    }

    private void OnWrongBasketItem(EventStructs.WrongBasketEvent wrongBasketEvent)
    {
      _gameBootstrapper.StateMachine.ChangeState(new GameLoseState(_gameBootstrapper));

      CheckAllBasketsForCompletionOrOutOfItems();
    }

    private void OnOutOfCorrectItems(EventStructs.OutOfCorrectItemsEvent outOfCorrectItemsEvent)
    {
      CheckAllBasketsForCompletionOrOutOfItems();
    }

    private void CheckAllBasketsForCompletionOrOutOfItems()
    {
      bool allCompletedOrOutOfCorrectItems = true;

      foreach (var basket in _baskets)
      {
        if (basket.Completed == false && basket.CorrectItemsCount > 0)
        {
          allCompletedOrOutOfCorrectItems = false;
          break;
        }
      }

      if (allCompletedOrOutOfCorrectItems == true)
        _gameBootstrapper.StateMachine.ChangeState(new GameLoseState(_gameBootstrapper));
    }

    private void OnBasketReceivedItem(EventStructs.BasketReceivedItemEvent basketReceivedItemEvent)
    {
      bool allCompleted = true;
      bool allCorrupted = true;

      foreach (var basket in _baskets)
      {
        if (basket.Completed == false)
          allCompleted = false;

        if (basket.Corrupted == false)
          allCorrupted = false;
      }

      if (allCompleted == true)
        AllBasketsCompletedAction();

      if (allCorrupted == true)
        AllBasketsCorruptedAction();
    }

    private void AllBasketsCompletedAction()
    {
      _gameBootstrapper.StateMachine.ChangeState(new GameWinState(_gameBootstrapper));
    }

    private void AllBasketsCorruptedAction()
    {
      _gameBootstrapper.StateMachine.ChangeState(new GameLoseState(_gameBootstrapper));
    }
  }
}