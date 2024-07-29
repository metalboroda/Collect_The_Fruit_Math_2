using Assets.__Game.Resources.Scripts.Logic;
using Assets.__Game.Resources.Scripts.SOs;
using Assets.__Game.Scripts.Enums;
using System.Collections.Generic;
using UnityEngine;

namespace __Game.Resources.Scripts.EventBus
{
  public class EventStructs
  {
    #region FiniteStateMachine
    public struct StateChanged : IEvent
    {
      public State State;
    }
    #endregion

    #region LevelManager
    public struct LastLevelEvent : IEvent
    {
      public bool LastLevel;
    }
    #endregion

    #region Game
    public struct StuporEvent : IEvent { }
    #endregion

    #region Tree
    public struct SpawnedItemsEvent : IEvent
    {
      public CorrectValuesContainerSo CorrectValuesContainerSo;
      public List<TreeItem> CorrectItems;
      public List<TreeItem> IncorrectItems;
    }
    #endregion

    #region TreeItem
    public struct BasketPlacedItemEvent : IEvent
    {
      public bool Correct;
      public int CorrectIncrement;
      public int IncorrectIncrement;
    }

    public struct TreeItemClickEvent : IEvent
    {
      public AudioClip ItemAudioClip;
    }
    #endregion

    #region Basket
    public struct BasketReceivedItemEvent : IEvent
    {
      public int Id;
    }

    public struct OutOfCorrectItemsEvent : IEvent
    {
      public int Id;
    }

    public struct WrongBasketEvent : IEvent
    {
      public int Id;
      public TreeItem TreeItem;
    }
    #endregion

    #region Variants&Answers
    public struct VariantsAssignedEvent : IEvent { }
    public struct CorrectAnswerEvent : IEvent { }
    public struct IncorrectCancelEvent : IEvent { }
    #endregion

    #region Game
    public struct WinEvent : IEvent { }
    public struct LoseEvent : IEvent { }
    #endregion

    #region ScoreManager
    public struct LevelPointEvent : IEvent
    {
      public int LevelPoint;
    }
    #endregion

    #region Ui
    public struct LevelCounterEvent : IEvent
    {
      public int OverallLevelIndex;
    }
    public struct UiButtonEvent : IEvent
    {
      public UiEnums UiEnums;
    }
    #endregion

    #region Audio
    public struct AudioSwitchedEvent : IEvent { }
    #endregion

    #region Components
    public struct ComponentEvent<T> : IEvent
    {
      public T Data { get; set; }
    }
    #endregion

    #region Timer
    public struct TimerEvent : IEvent
    {
      public int Time;
    }
    #endregion
  }
}