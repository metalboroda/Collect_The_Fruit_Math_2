using __Game.Resources.Scripts.EventBus;
using Assets.__Game.Resources.Scripts.Game.States;
using Assets.__Game.Scripts.Infrastructure;
using UnityEngine;

namespace Assets.__Game.Resources.Scripts.LevelItem
{
  public class LevelTimer : MonoBehaviour
  {
    [SerializeField] private int _maxTime;

    private bool _allowTimer = true;
    private float _currentTime;

    private GameBootstrapper _gameBootstrapper;

    private EventBinding<EventStructs.StateChanged> _stateChangedEvent;
    private EventBinding<EventStructs.WinEvent> _winEvent;

    private void Awake()
    {
      _gameBootstrapper = GameBootstrapper.Instance;
    }

    private void OnEnable()
    {
      _stateChangedEvent = new EventBinding<EventStructs.StateChanged>(StopTimer);
      _winEvent = new EventBinding<EventStructs.WinEvent>(StopTimer);
    }

    private void OnDisable()
    {
      _stateChangedEvent.Remove(StopTimer);
      _winEvent.Remove(StopTimer);
    }

    void Start()
    {
      _currentTime = _maxTime;
    }

    void Update()
    {
      if (_allowTimer == false) return;
      if (_currentTime > 0)
      {
        _currentTime -= Time.deltaTime;
      }
      else
      {
        _currentTime = 0;

        TimerFinished();
      }

      EventBus<EventStructs.TimerEvent>.Raise(new EventStructs.TimerEvent { Time = (int)_currentTime });
    }

    private void TimerFinished()
    {
      _gameBootstrapper.StateMachine.ChangeState(new GameLoseState(_gameBootstrapper));
    }

    private void StopTimer(EventStructs.StateChanged stateChanged)
    {
      if (stateChanged.State is GameplayState)
      {
        _allowTimer = true;
      }
      else
      {
        _allowTimer = false;
      }
    }

    private void StopTimer(EventStructs.WinEvent winEvent)
    {
      _allowTimer = false;
    }
  }
}