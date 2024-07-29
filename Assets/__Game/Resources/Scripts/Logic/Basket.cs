using __Game.Resources.Scripts.EventBus;
using Assets.__Game.Resources.Scripts.Game.States;
using Assets.__Game.Resources.Scripts.SOs;
using Assets.__Game.Scripts.Infrastructure;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.__Game.Resources.Scripts.Logic
{
  public class Basket : MonoBehaviour
  {
    [SerializeField] private CorrectValuesContainerSo[] _correctValuesContainersSo;
    [field: Header("Tutorial")]
    [field: SerializeField] public bool Tutorial { get; private set; }
    [Header("Stupor param's")]
    [SerializeField] private float _stuporTimeoutSeconds = 10f;

    public bool Completed { get; private set; }
    public bool Corrupted { get; private set; }
    public int CorrectItemsCount => _correctItems.Count;

    private List<TreeItem> _correctItems = new List<TreeItem>();
    private List<TreeItem> _incorrectItems = new List<TreeItem>();
    private int _correctItemsAmount;
    private int _incorrectItemsAmount;
    private bool _canReceiveItems = true;
    private TreeItem _placedTreeItem;
    private int _correctCounter;
    private int _incorrectCounter;
    private Coroutine _stuporTimeoutRoutine;

    private GameBootstrapper _gameBootstrapper;

    private EventBinding<EventStructs.SpawnedItemsEvent> _spawnedItemsEvent;
    private EventBinding<EventStructs.StateChanged> _stateChangedEvent;

    private void Awake()
    {
      _gameBootstrapper = GameBootstrapper.Instance;
    }

    private void OnEnable()
    {
      _spawnedItemsEvent = new EventBinding<EventStructs.SpawnedItemsEvent>(ReceiveSpawnedItems);
      _stateChangedEvent = new EventBinding<EventStructs.StateChanged>(StuporTimerDependsOnState);
    }

    private void OnDisable()
    {
      _spawnedItemsEvent.Remove(ReceiveSpawnedItems);
      _stateChangedEvent.Remove(StuporTimerDependsOnState);
    }

    private void Start()
    {
      EventBus<EventStructs.ComponentEvent<Basket>>.Raise(new EventStructs.ComponentEvent<Basket> { Data = this });
    }

    private void ReceiveSpawnedItems(EventStructs.SpawnedItemsEvent spawnedItemsEvent)
    {
      _correctItems.Clear();
      _incorrectItems.Clear();

      foreach (var item in spawnedItemsEvent.CorrectItems)
      {
        foreach (var container in _correctValuesContainersSo)
        {
          if (container.CorrectValues.Contains(item.Answer))
          {
            _correctItems.Add(item);
            break;
          }
        }
      }

      foreach (var item in spawnedItemsEvent.IncorrectItems)
      {
        bool isCorrect = false;

        foreach (var container in _correctValuesContainersSo)
        {
          if (container.CorrectValues.Contains(item.Answer))
          {
            isCorrect = true;
            break;
          }
        }

        if (isCorrect == false)
          _incorrectItems.Add(item);
      }

      _correctItemsAmount = _correctItems.Count;
      _incorrectItemsAmount = _incorrectItems.Count;
    }

    public void PlaceItem(TreeItem treeItem)
    {
      if (_canReceiveItems == false) return;
      if (_gameBootstrapper.StateMachine.CurrentState is GameplayState == false) return;

      _placedTreeItem = treeItem;

      treeItem.transform.SetParent(transform);
      treeItem.transform.DOMove(transform.position, 0.25f).OnComplete(() =>
      {
        CheckForCorrectItem();

        _canReceiveItems = true;
      });

      CheckIfItemBelongsToAnotherBasket(treeItem);
    }

    private void CheckForCorrectItem()
    {
      ResetAndStartStuporTimer();

      bool isCorrect = false;

      foreach (var container in _correctValuesContainersSo)
      {
        if (container.CorrectValues.Contains(_placedTreeItem.Answer))
        {
          isCorrect = true;
          break;
        }
      }

      if (isCorrect == true)
      {
        _correctCounter++;
        _correctItems.Remove(_placedTreeItem);
        _placedTreeItem.SpawnParticles(true);

        Destroy(_placedTreeItem.gameObject);

        if (_correctCounter >= _correctItemsAmount)
          Completed = true;

        if (Tutorial == false)
          EventBus<EventStructs.LevelPointEvent>.Raise(new EventStructs.LevelPointEvent { LevelPoint = 1 });

        EventBus<EventStructs.BasketPlacedItemEvent>.Raise(new EventStructs.BasketPlacedItemEvent
        {
          Correct = true,
          CorrectIncrement = 1
        });
      }
      else
      {
        _incorrectCounter++;
        _incorrectItems.Remove(_placedTreeItem);
        _placedTreeItem.SpawnParticles(false);

        Destroy(_placedTreeItem.gameObject);

        if (_incorrectCounter >= _incorrectItemsAmount)
        {
          Corrupted = true;
        }

        EventBus<EventStructs.BasketPlacedItemEvent>.Raise(new EventStructs.BasketPlacedItemEvent
        {
          Correct = false,
          IncorrectIncrement = 1
        });
      }

      EventBus<EventStructs.BasketReceivedItemEvent>.Raise(new EventStructs.BasketReceivedItemEvent { Id = transform.GetInstanceID() });

      if (_correctItems.Count == 0 && Completed == false)
        EventBus<EventStructs.OutOfCorrectItemsEvent>.Raise(new EventStructs.OutOfCorrectItemsEvent { Id = transform.GetInstanceID() });
    }

    private void CheckIfItemBelongsToAnotherBasket(TreeItem treeItem)
    {
      foreach (var container in _correctValuesContainersSo)
      {
        if (container.CorrectValues.Contains(treeItem.Answer) == false)
        {
          EventBus<EventStructs.WrongBasketEvent>.Raise(new EventStructs.WrongBasketEvent { Id = transform.GetInstanceID(), TreeItem = treeItem });
          break;
        }
      }
    }

    private void StuporTimerDependsOnState(EventStructs.StateChanged stateChanged)
    {
      if (stateChanged.State is GameplayState)
        ResetAndStartStuporTimer();
      else
      {
        if (_stuporTimeoutRoutine != null)
          StopCoroutine(_stuporTimeoutRoutine);
      }
    }

    private void ResetAndStartStuporTimer()
    {
      if (_stuporTimeoutRoutine != null)
        StopCoroutine(_stuporTimeoutRoutine);

      _stuporTimeoutRoutine = StartCoroutine(DoStuporTimerCoroutine());
    }

    private IEnumerator DoStuporTimerCoroutine()
    {
      yield return new WaitForSeconds(_stuporTimeoutSeconds);

      EventBus<EventStructs.StuporEvent>.Raise(new EventStructs.StuporEvent());

      ResetAndStartStuporTimer();
    }
  }
}