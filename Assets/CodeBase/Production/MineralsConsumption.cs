using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Production
{
  public class MineralsConsumption : MonoBehaviour
  {
    public Transform StartingPoint => _startingPoint;
    public int MaxStorageCapacity => _maxStorageCapacity;
    public int MaxStorageLines => _maxStorageLines;
    public List<GameObject> ConsumptionStorageMinerals => _consumptionStorageMinerals;

    [SerializeField] private Transform _startingPoint;
    [SerializeField] private int _maxStorageCapacity;
    [SerializeField] private int _maxStorageLines;

    private List<GameObject> _consumptionStorageMinerals = new();
  }
}