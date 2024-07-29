using __Game.Resources.Scripts.EventBus;
using Assets.__Game.Resources.Scripts.SOs;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.__Game.Resources.Scripts.Logic
{
  public class Tree : MonoBehaviour
  {
    [SerializeField] private CorrectValuesContainerSo _correctValuesContainerSo;
    [Header("Spawn Param's")]
    [SerializeField] private float _minObjectSpawnDistance = 0.25f;
    [Space]
    [SerializeField] private TreeItemData[] _treeItemsDatas;

    private readonly List<Vector3> _spawnedPositions = new List<Vector3>();
    private readonly List<TreeItem> _correctItems = new List<TreeItem>();
    private readonly List<TreeItem> _incorrectItems = new List<TreeItem>();

    private CapsuleCollider _capsuleCollider;

    private void Awake()
    {
      _capsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void Start()
    {
      StartCoroutine(DoSpawnTreeItems());
    }

    private IEnumerator DoSpawnTreeItems()
    {
      foreach (var treeItemData in _treeItemsDatas)
      {
        for (int i = 0; i < treeItemData.Amount; i++)
        {
          SpawnTreeItem(treeItemData);
        }
      }

      yield return new WaitForEndOfFrame();

      _capsuleCollider.enabled = false;

      EventBus<EventStructs.SpawnedItemsEvent>.Raise(new EventStructs.SpawnedItemsEvent
      {
        CorrectValuesContainerSo = _correctValuesContainerSo,
        CorrectItems = _correctItems,
        IncorrectItems = _incorrectItems
      });
    }

    private void SpawnTreeItem(TreeItemData treeItemData)
    {
      Vector3 spawnPosition = GetRandomSpawnPosition();
      TreeItem itemToSpawn = treeItemData.ItemsToSpawn;
      TreeItem spawnedItem = Instantiate(itemToSpawn, spawnPosition, Quaternion.identity, transform);

      spawnedItem.SetRandomModelSize();
      spawnedItem.SetAnswer(treeItemData.AnswerText);
      spawnedItem.SetNarrationClip(treeItemData.NarrationCLip);
      _spawnedPositions.Add(spawnPosition);

      if (_correctValuesContainerSo.CorrectValues.Contains(treeItemData.AnswerText))
        _correctItems.Add(spawnedItem);
      else
        _incorrectItems.Add(spawnedItem);
    }

    private Vector3 GetRandomSpawnPosition()
    {
      Vector3 spawnPosition;
      int attempts = 0;

      do
      {
        float x = Random.Range(-_capsuleCollider.radius, _capsuleCollider.radius);
        float y = Random.Range(-_capsuleCollider.height, _capsuleCollider.height);

        spawnPosition = new Vector3(x, y, 0) + transform.position;
        attempts++;
      }
      while (IsPositionValid(spawnPosition) == false && attempts < 100);

      return spawnPosition;
    }

    private bool IsPositionValid(Vector3 position)
    {
      foreach (var spawnedPosition in _spawnedPositions)
      {
        if (Vector3.Distance(position, spawnedPosition) < _minObjectSpawnDistance)
          return false;
      }
      return true;
    }
  }
}