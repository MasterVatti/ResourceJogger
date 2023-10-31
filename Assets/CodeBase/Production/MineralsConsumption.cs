using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private Transform _targetPoint;
    [SerializeField] private int _maxStorageCapacity;
    [SerializeField] private int _maxStorageLines;
    [SerializeField] private List<StoragePoint> _storagePoints;
    [SerializeField] private float _mineralFlightSpeed;
    [SerializeField] private float _cooldown = 1f;

    private List<GameObject> _consumptionStorageMinerals = new();
    private List<MineralStates> _mineralStates = new();
    // private List<float> _startTime = new();
    private float _startTime;
    // private List<float> _journeyLengths = new();
    private List<float> _journeyLengths = new();
    private float _currentTime;
    private int _counter;
    private int _secondCounter;
    private int _i;
    private List<StoragePoint> _tempPoints = new();
    private MineralStates _currentMineral;
    private StoragePoint _currentTempPoint;
    private float _currentJourneyLength;
    
    private void Update()
    {
      _currentTime += Time.deltaTime;
    }

    private void FixedUpdate()
    {
      if (_mineralStates.Count <= 0)
      {
        _currentTime = 0f;
        return;
      }
      
      if (_currentTime < _cooldown)
      { 
        return;
      }

      if (_currentMineral == null)
      {
        return;
      }
      // Debug.Log($"_counter = {_counter}");
      // _currentMineral = _mineralStates.Last();
      // float startTime = _startTime.Last();
      // float journeyLength = _journeyLengths.Last();


      // float journeyLength = Vector3.Distance(mineral.transform.position, _targetPoint.transform.position);
        
      float fractionOfJourney = CountPartOfJourney(_startTime, _currentJourneyLength);
      _currentMineral.transform.position =
        Vector3.Lerp(_currentMineral.transform.position, _targetPoint.transform.position, fractionOfJourney);
        
      if (_currentMineral.transform.position == _targetPoint.transform.position)
      {
        // Debug.Log($"_targetPoint.transform.position = {_targetPoint.transform.position}");
        // _storagePoints.Last(point => point.IsInsideStorage).IsInsideStorage = false;
        // StoragePoint point = _tempPoints.Last();
        // point.IsInsideStorage = false;
        _currentTempPoint.IsInsideStorage = false;
        _tempPoints.Remove(_currentTempPoint);
        _mineralStates.Remove(_currentMineral);
        _journeyLengths.Remove(_currentJourneyLength);
        _currentMineral = _mineralStates.LastOrDefault();
        _currentTempPoint = _tempPoints.LastOrDefault();
        _currentJourneyLength = _journeyLengths.LastOrDefault();
        // _secondCounter -= 1;
        // Destroy(mineral);
        // _currentTime = 0f;
        _startTime = Time.time;
        // _collectingMinerals.DeleteMineralFromBag(_collectingMinerals.AllMinerals[i]);
      }

    }
    private float CountPartOfJourney(float startTime, float journeyLength)
    {
      // Debug.Log($"Time.time = {Time.time}, startTime = {startTime}");
      float distanceCovered = (Time.time - startTime) * _mineralFlightSpeed;
      float fractionOfJourney = distanceCovered / journeyLength;
      // Debug.Log($"distanceCovered = {distanceCovered}, journeyLength = {journeyLength}, fractionOfJourney = {fractionOfJourney}");
      return fractionOfJourney;
    }
    public void SetValues(MineralStates mineral, StoragePoint storagePoint)
    {
      if (_mineralStates.Count <= 0)
      {
        _currentMineral = mineral;
        _currentTempPoint = storagePoint;
        _currentJourneyLength = Vector3.Distance(mineral.transform.position, _targetPoint.transform.position);
      }
      _mineralStates.Add(mineral);
      _tempPoints.Add(storagePoint);
      _startTime = Time.time;
      _journeyLengths.Add(Vector3.Distance(mineral.transform.position, _targetPoint.transform.position));
    }
  }
}