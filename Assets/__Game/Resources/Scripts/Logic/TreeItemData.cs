using System;
using UnityEngine;

namespace Assets.__Game.Resources.Scripts.Logic
{
  [Serializable]
  public class TreeItemData
  {
    [field: SerializeField] public TreeItem ItemsToSpawn { get; private set; }
    [field: SerializeField] public string AnswerText { get; private set; }
    [field: SerializeField] public int Amount { get; private set; }
    [field: Space]
    [field: SerializeField] public AudioClip NarrationCLip { get; private set; }
  }
}