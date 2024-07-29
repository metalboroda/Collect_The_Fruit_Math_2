using Assets.__Game.Resources.Scripts.Logic;
using UnityEngine;

namespace Assets.__Game.Resources.Scripts.SOs
{
  [CreateAssetMenu(fileName = "TreeItemsContainer", menuName = "SOs/Containers/TreeItemsContainer")]
  public class TreeItemsContainerSo : ScriptableObject
  {
    public TreeItem[] ItemsToSpawn;

    public TreeItem GetRandomItem()
    {
      return ItemsToSpawn[Random.Range(0, ItemsToSpawn.Length)];
    }
  }
}