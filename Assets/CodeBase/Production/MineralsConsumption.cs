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
    [SerializeField] private float _cooldown;
    [SerializeField] private MineralsProduction _mineralsProduction;

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
    private List<float> _startTimes = new();
    
    [SerializeField] private int _interpolationFramesCount = 45;
    private int _elapsedFrames = 0;
    
    private void Update()
    {
      _currentTime += Time.deltaTime;
    }

    private void FixedUpdate()
    {
      if (_mineralStates.Count <= 0)
      {
        return;
      }
      
      if (_currentMineral == null)
      {
        return;
      }
      if (_currentTime < _cooldown)
      { 
        return;
      }
      float fractionOfJourney = CountPartOfJourney(_startTime, _currentJourneyLength);
      _currentMineral.transform.position =
        Vector3.Lerp(_currentMineral.transform.position, _targetPoint.transform.position, fractionOfJourney);
      if (_currentMineral.transform.position == _targetPoint.transform.position)
      {

        _counter += 1;
        _currentTempPoint.IsInsideStorage = false;
        _mineralStates.Remove(_currentMineral);
        _tempPoints.Remove(_currentTempPoint);
        _journeyLengths.Remove(_currentJourneyLength);
        _mineralsProduction.AddMineralToProduction(_currentMineral);
        Destroy(_currentMineral.gameObject);
        _currentMineral = _mineralStates.FirstOrDefault();
        _currentTempPoint = _tempPoints.FirstOrDefault();
        _currentJourneyLength = _journeyLengths.FirstOrDefault();


        _currentTime = 0f;
        _startTime = Time.time;

      }

    }

    public void AddMineralToConsumption(MineralStates mineral, StoragePoint storagePoint)
    {
      float journeyLength = Vector3.Distance(mineral.transform.position, _targetPoint.transform.position);
      if (_mineralStates.Count <= 0)
      {
        _currentMineral = mineral;
        _currentTempPoint = storagePoint;
        _currentJourneyLength = journeyLength;
        _startTime = Time.time;
      }
      _currentTime = 0f;

      _mineralStates.Add(mineral);
      _tempPoints.Add(storagePoint);
      _journeyLengths.Add(journeyLength);
    }

    private float CountPartOfJourney(float startTime, float journeyLength)
    {
      float distanceCovered = (Time.time - startTime) * _mineralFlightSpeed;
      float fractionOfJourney = distanceCovered / journeyLength;
      return fractionOfJourney;
    }
  }
}