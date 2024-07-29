using __Game.Resources.Scripts.EventBus;
using Assets.__Game.Resources.Scripts.Game.States;
using Assets.__Game.Resources.Scripts.LevelItem;
using Assets.__Game.Resources.Scripts.Settings;
using Assets.__Game.Resources.Scripts.SOs;
using Assets.__Game.Scripts.Enums;
using Assets.__Game.Scripts.Infrastructure;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.__Game.Resources.Scripts.Management
{
  public class GameUiManager : MonoBehaviour
  {
    [SerializeField] private AudioSource _audioSource;

    [Header("GlobalCanvas")]
    [SerializeField] private GameObject _globalCanvas;
    [Space]
    [SerializeField] private Button _globalAudioBtn;
    [SerializeField] private GameObject _globalAudioOnImage;
    [SerializeField] private GameObject _globalAudioOffImage;
    [Header("Quest Canvas")]
    [SerializeField] private GameObject _questCanvas;
    [Space]
    [SerializeField] private TextMeshProUGUI _questLevelCounterText;
    [SerializeField] private TextMeshProUGUI _questTaskText;
    [SerializeField] private TextMeshProUGUI _questCorrectNumbersTxt;
    [SerializeField] private Button _questPlayButton;
    [Header("Game Canvas")]
    [SerializeField] private GameObject _gameCanvas;
    [Space]
    [SerializeField] private TextMeshProUGUI _gameScoreCounterTxt;
    [SerializeField] private GameObject _gameStarImage;
    [Space]
    [SerializeField] private TextMeshProUGUI _gameLoseCounterTxt;
    [SerializeField] private GameObject _gameAngryFaceImage;
    [Space]
    [SerializeField] private Button _gamePauseButton;
    [Space]
    [SerializeField] private TextMeshProUGUI _gameTimerText;
    [Header("Game Canvas Animation")]
    [SerializeField] private float _gameImageeIn = 1.3f;
    [SerializeField] private float _gameImageAnimDuration = 0.15f;
    [Header("Win Canvas")]
    [SerializeField] private GameObject _winCanvas;
    [Space]
    [SerializeField] private Button _winNextLevelBtn;
    [SerializeField] private Button _winRewardButton;
    [SerializeField] private GameObject _winPerfectText;
    [SerializeField] private AudioClip _winPerfectSound;
    [Header("Lose Canvas")]
    [SerializeField] private GameObject _loseCanvas;
    [Space]
    [SerializeField] private Button _loseNextLevelBtn;
    [SerializeField] private Button _loseRestartBtn;
    [Header("Pause Canvas")]
    [SerializeField] private GameObject _pauseCanvas;
    [Space]
    [SerializeField] private TextMeshProUGUI _pauseLevelCounterText;
    [SerializeField] private TextMeshProUGUI _pauseTaskText;
    [SerializeField] private TextMeshProUGUI _pauseCorrectNumbersTxt;
    [SerializeField] private Button _pauseContinueBtn;
    [SerializeField] private Button _pauseRestartButton;

    private readonly List<GameObject> _canvases = new();
    private int _currentScore;
    private int _overallScore;
    private int _currentLoses;
    private CorrectValuesContainerSo _correctValuesContainerSo;
    private bool _lastLevel;

    private GameBootstrapper _gameBootstrapper;
    private Reward _reward;
    private GameSettings _gameSettings;

    private EventBinding<EventStructs.ComponentEvent<GameBootstrapper>> _componentEvent;
    private EventBinding<EventStructs.StateChanged> _stateChanged;
    private EventBinding<EventStructs.TimerEvent> _timerEvent;
    private EventBinding<EventStructs.SpawnedItemsEvent> _spawnedItemsEvent;
    private EventBinding<EventStructs.BasketPlacedItemEvent> _basketPlacedItemsEvent;
    private EventBinding<EventStructs.LastLevelEvent> _lastLevelEvent;

    private void Awake()
    {
      _reward = new Reward();
      _gameSettings = new GameSettings();

      LoadSettings();
    }

    private void OnEnable()
    {
      _componentEvent = new EventBinding<EventStructs.ComponentEvent<GameBootstrapper>>(SetBootstrapper);
      _stateChanged = new EventBinding<EventStructs.StateChanged>(SwitchCanvasesDependsOnState);
      _timerEvent = new EventBinding<EventStructs.TimerEvent>(DisplayTimer);
      _spawnedItemsEvent = new EventBinding<EventStructs.SpawnedItemsEvent>(SetOverallScore);
      _spawnedItemsEvent = new EventBinding<EventStructs.SpawnedItemsEvent>(DisplayCorrectValuesArray);
      _basketPlacedItemsEvent = new EventBinding<EventStructs.BasketPlacedItemEvent>(DisplayScore);
      _basketPlacedItemsEvent = new EventBinding<EventStructs.BasketPlacedItemEvent>(IconScaleAnimation);
      _lastLevelEvent = new EventBinding<EventStructs.LastLevelEvent>(OnLastLevel);
    }

    private void OnDisable()
    {
      _componentEvent.Remove(SetBootstrapper);
      _stateChanged.Remove(SwitchCanvasesDependsOnState);
      _timerEvent.Remove(DisplayTimer);
      _spawnedItemsEvent.Remove(SetOverallScore);
      _spawnedItemsEvent.Remove(DisplayCorrectValuesArray);
      _basketPlacedItemsEvent.Remove(DisplayScore);
      _basketPlacedItemsEvent.Remove(IconScaleAnimation);
      _lastLevelEvent.Remove(OnLastLevel);
    }

    private void Start()
    {
      SubscribeButtons();
      AddCanvasesToList();
      UpdateAudioButtonVisuals();
    }

    private void LoadSettings()
    {
      _gameSettings = SettingsManager.LoadSettings<GameSettings>();

      if (_gameSettings == null)
        _gameSettings = new GameSettings();
    }

    private void SubscribeButtons()
    {
      // Quest
      _questPlayButton.onClick.AddListener(() =>
      {
        EventBus<EventStructs.UiButtonEvent>.Raise(new EventStructs.UiButtonEvent
        {
          UiEnums = UiEnums.QuestPlayButton
        });
      });

      // Game
      _gamePauseButton.onClick.AddListener(() =>
      {
        EventBus<EventStructs.UiButtonEvent>.Raise(new EventStructs.UiButtonEvent
        {
          UiEnums = UiEnums.GamePauseButton
        });
      });

      // Win
      _winNextLevelBtn.onClick.AddListener(() =>
      {
        EventBus<EventStructs.UiButtonEvent>.Raise(new EventStructs.UiButtonEvent
        {
          UiEnums = UiEnums.WinNextLevelButton
        });

        _gameBootstrapper.RestartLevel();
      });
      _winRewardButton.onClick.AddListener(() =>
      {
        _winPerfectText.gameObject.SetActive(false);
        _winRewardButton.gameObject.SetActive(false);
        //_winPerfectSound.gameObject.SetActive(false);
      });

      // Lose
      _loseNextLevelBtn.onClick.AddListener(() =>
      {
        EventBus<EventStructs.UiButtonEvent>.Raise(new EventStructs.UiButtonEvent
        {
          UiEnums = UiEnums.WinNextLevelButton
        });

        _gameBootstrapper.RestartLevel();
      });
      _loseRestartBtn.onClick.AddListener(() =>
      {
        EventBus<EventStructs.UiButtonEvent>.Raise(new EventStructs.UiButtonEvent
        {
          UiEnums = UiEnums.LoseRestartLevelButton
        });

        _gameBootstrapper.RestartLevel();
      });

      // Pause
      _pauseContinueBtn.onClick.AddListener(() =>
      {
        EventBus<EventStructs.UiButtonEvent>.Raise(new EventStructs.UiButtonEvent
        {
          UiEnums = UiEnums.PauseContinueButton
        });
      });
      _pauseRestartButton.onClick.AddListener(() =>
      {
        _gameBootstrapper.RestartLevel();
      });
      _globalAudioBtn.onClick.AddListener(SwitchAudioVolumeButton);
    }

    private void AddCanvasesToList()
    {
      _canvases.Add(_questCanvas);
      _canvases.Add(_gameCanvas);
      _canvases.Add(_winCanvas);
      _canvases.Add(_loseCanvas);
      _canvases.Add(_pauseCanvas);
    }

    private void SetBootstrapper(EventStructs.ComponentEvent<GameBootstrapper> componentEvent)
    {
      _gameBootstrapper = componentEvent.Data;
    }

    private void SetOverallScore(EventStructs.SpawnedItemsEvent spawnedItemsEvent)
    {
      _overallScore += spawnedItemsEvent.CorrectItems.Count;
      _gameScoreCounterTxt.text = $"{_currentScore} / {_overallScore}";

      _gameLoseCounterTxt.text = _currentLoses.ToString();
    }

    private void DisplayScore(EventStructs.BasketPlacedItemEvent basketPlacedItemEvent)
    {
      if (basketPlacedItemEvent.Correct == true)
      {
        _currentScore += basketPlacedItemEvent.CorrectIncrement;
        _gameScoreCounterTxt.text = $"{_currentScore} / {_overallScore}";
      }
      else
      {
        _currentLoses += basketPlacedItemEvent.IncorrectIncrement;
        _gameLoseCounterTxt.text = $"{_currentLoses}";
      }
    }

    private void DisplayLevelCounter()
    {
      if (_gameSettings.OverallLevelIndex == 0)
        _questLevelCounterText.text = $"НАВЧАЛЬНИЙ РІВЕНЬ";
      else
        _questLevelCounterText.text = $"РІВЕНЬ {_gameSettings.OverallLevelIndex}";

      if (_gameSettings.OverallLevelIndex == 0)
        _pauseLevelCounterText.text = $"НАВЧАЛЬНИЙ РІВЕНЬ";
      else
        _pauseLevelCounterText.text = $"РІВЕНЬ {_gameSettings.OverallLevelIndex}";
    }

    private void DisplayCorrectValuesArray(EventStructs.SpawnedItemsEvent spawnedItemsEvent)
    {
      _correctValuesContainerSo = spawnedItemsEvent.CorrectValuesContainerSo;
      string arrayString = "";

      for (int i = 0; i < _correctValuesContainerSo.CorrectValues.Length; i++)
      {
        arrayString += _correctValuesContainerSo.CorrectValues[i];

        if (i < _correctValuesContainerSo.CorrectValues.Length - 1)
          arrayString += " ";
      }

      DisplayLevelCounter();

      _questTaskText.text = spawnedItemsEvent.CorrectValuesContainerSo.QuestText;
      _questCorrectNumbersTxt.text = arrayString;
      _pauseTaskText.text = spawnedItemsEvent.CorrectValuesContainerSo.QuestText;
      _pauseCorrectNumbersTxt.text = arrayString;
    }

    private void IconScaleAnimation(EventStructs.BasketPlacedItemEvent basketPlacedItemEvent)
    {
      Sequence seq = DOTween.Sequence();
      Transform icon;

      if (basketPlacedItemEvent.Correct == true)
        icon = _gameStarImage.transform;
      else
        icon = _gameAngryFaceImage.transform;

      seq.Append(icon.DOScale(_gameImageeIn, _gameImageAnimDuration));
      seq.Append(icon.DOScale(1f, _gameImageAnimDuration));
    }

    private void SwitchCanvasesDependsOnState(EventStructs.StateChanged state)
    {
      switch (state.State)
      {
        case GameQuestState:
          _globalCanvas.SetActive(true);
          SwitchCanvas(_questCanvas);
          break;
        case GameplayState:
          _globalCanvas.SetActive(false);
          SwitchCanvas(_gameCanvas);
          break;
        case GameWinState:
          _globalCanvas.SetActive(true);
          SwitchCanvas(_winCanvas);
          TryToEnableReward();

          if (_lastLevel == true)
          {
            _winNextLevelBtn.gameObject.SetActive(false);
            _loseNextLevelBtn.gameObject.SetActive(false);
          }
          break;
        case GameLoseState:
          _globalCanvas.SetActive(true);
          SwitchCanvas(_loseCanvas);

          if (_lastLevel == true)
            _loseNextLevelBtn.gameObject.SetActive(false);
          break;
        case GamePauseState:
          _globalCanvas.SetActive(true);
          SwitchCanvas(_pauseCanvas);
          break;
      }
    }

    private void SwitchCanvas(GameObject canvas, float delay = 0)
    {
      StartCoroutine(DoSwitchCanvas(canvas, delay));
    }

    private IEnumerator DoSwitchCanvas(GameObject canvas, float delay)
    {
      yield return new WaitForSeconds(delay);

      foreach (var canvasItem in _canvases)
      {
        if (canvasItem == canvas)
          canvas.SetActive(true);
        else
          canvasItem.SetActive(false);
      }
    }

    private void TryToEnableReward()
    {
      if (_currentLoses > 0) return;

      //_winRewardButton.gameObject.SetActive(true);
      _winPerfectText.gameObject.SetActive(true);
      //_winPerfectSound.gameObject.SetActive(true);
      _audioSource.PlayOneShot(_winPerfectSound);
    }

    private void SwitchAudioVolumeButton()
    {
      _gameSettings.IsMusicOn = !_gameSettings.IsMusicOn;

      UpdateAudioButtonVisuals();
      EventBus<EventStructs.AudioSwitchedEvent>.Raise();
      SettingsManager.SaveSettings(_gameSettings);
    }

    private void UpdateAudioButtonVisuals()
    {
      _globalAudioOnImage.SetActive(_gameSettings.IsMusicOn);
      _globalAudioOffImage.SetActive(!_gameSettings.IsMusicOn);
    }

    private void DisplayTimer(EventStructs.TimerEvent timerEvent)
    {
      _gameTimerText.text = $"Час: {timerEvent.Time}";
    }

    private void OnLastLevel(EventStructs.LastLevelEvent lastLevelEvent)
    {
      _lastLevel = lastLevelEvent.LastLevel;
    }
  }
}