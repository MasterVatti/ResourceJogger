using System;
using System.Collections.Generic;
using CodeBase.Minerals;
using CodeBase.Player;
using CodeBase.Services;
using UnityEngine;
using Zenject;

namespace CodeBase.Production
{
  public class MineralsConsumption : MonoBehaviour
  {
    public event Action<GameObject> MineralAdded;
    public Transform StartingPoint => _startingPoint;
    public int MaxStorageCapacity => _maxStorageCapacity;
    public int MaxStorageLines => _maxStorageLines;
    public int ConsumptionMineralCount { get; set; }
    public List<GameObject> ConsumptionStorageMinerals => _consumptionStorageMinerals;
    public List<MineralStates> MineralStates => _mineralStates;
    public List<StoragePoint> StoragePoints => _storagePoints;

    [SerializeField] private Transform _startingPoint;
    [SerializeField] private int _maxStorageCapacity;
    [SerializeField] private int _maxStorageLines;
    [SerializeField] private List<StoragePoint> _storagePoints;

    private List<GameObject> _consumptionStorageMinerals = new();
    private List<MineralStates> _mineralStates = new();

    private void FixedUpdate()
    {
      if (_mineralStates.Count != 0)
      {
        
      }
    }
    // public void AddMineralToStorage(GameObject lastMineral)
    // {
    //   MineralAdded?.Invoke(lastMineral);
    // }
  }
}