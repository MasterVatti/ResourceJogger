using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Infrastructure.Factory;
using CodeBase.Minerals;
using CodeBase.Player;
using CodeBase.Services.UIService;
using UnityEngine;
using Zenject;

namespace CodeBase.Production
{
  public class MineralsProduction : MonoBehaviour
  {
    [SerializeField] private int _maxStorage;
    [SerializeField] private List<StoragePoint> _storagePoints;
    [SerializeField] private MineralsConsumption _mineralsConsumption;
    [SerializeField] private float _makeMineralTime;
    [SerializeField] private float _mineralFlightSpeed;
    [SerializeField] private float _cooldown;
    [SerializeField] private MineralStates _mineralPrefab;
    [SerializeField] private Transform _startPointPosition;
    [SerializeField] private List<MineralType> _consumptionMineralTypes;
    [SerializeField] private MineralType _productionMineralType;

    private IGameFactory _gameFactory;
    private List<MineralStates> _mineralStates = new();
    private int _consumptionMineralCount;
    private int[] _productionCount;

    private float _currentTime;
    private float _startTime;
    private MineralStates _currentMineral;
    private StoragePoint _currentTargetPoint;
    private int _storageCount;
    private bool _isFirstAddedMineral = true;
    private int _counter;
    private IUIService _uiService;
    private bool _isMessageShowed;
    
    [Inject]
    public void Constructor(IGameFactory gameFactory, IUIService uiService)
    {
      _gameFactory = gameFactory;
      _uiService = uiService;
    }

    private void Start()
    {
      _productionCount = new int[_consumptionMineralTypes.Count];
      _currentTargetPoint = _storagePoints.FirstOrDefault(point => !point.IsInsideStorage);
      // _currentMineral = _gameFactory.CreateMineral(_mineralPrefab, _currentTargetPoint.transform);
      _currentMineral = _gameFactory.CreateMineral(_mineralPrefab, _startPointPosition.position, Quaternion.identity);
      _currentMineral.CollectingZoneCollider.enabled = false;
      _currentMineral.transform.SetParent(_currentTargetPoint.transform);
    }

    private void Update()
    {
      _currentTime += Time.deltaTime;
    }
    
    private void FixedUpdate()
    {

      if (_productionCount.Any(counter => counter <= 0))
      {
        
        _currentTime = 0f;
        return;
      }

      if (_currentTargetPoint == null)
      {
        // Debug.Log("FixedUpdate _currentTargetPoint == null");
        // _currentTime = 0f;
        CreateMineral();
        return;
      }
      
      if (_currentTime < _cooldown)
      { 
        return;
      }
      // Debug.Log($"_counter = {_counter}");
      
      float currentJourneyLength = Vector3.Distance(_currentMineral.transform.position, _currentTargetPoint.transform.position);
      float fractionOfJourney = CountPartOfJourney(_startTime, currentJourneyLength);

      _currentMineral.transform.position = Vector3.Lerp(_currentMineral.transform.position, _currentTargetPoint.transform.position, fractionOfJourney);
      
      if (_currentMineral.transform.position == _currentTargetPoint.transform.position)
      {
        _counter += 1;
        _currentTargetPoint.IsInsideStorage = true;

        // _storageCount += 1;
        _currentMineral.CollectingZoneCollider.enabled = true;
        CreateMineral();
        

      }
    }

    public void AddMineralToProduction(MineralStates mineral)
    {
      if (_isFirstAddedMineral)
      {
        _isFirstAddedMineral = false;
        _startTime = Time.time;
      }
      for (var i = 0; i < _consumptionMineralTypes.Count; i++)
      {
        if (mineral.MineralType == _consumptionMineralTypes[i])
        {
          _productionCount[i] += 1;
        }
      }

    }

    private void CreateMineral()
    {
      _currentTargetPoint = _storagePoints.FirstOrDefault(point => !point.IsInsideStorage);
      if (_currentTargetPoint == null)
      {
        if (!_isMessageShowed) SelectUIMessage();
        _isMessageShowed = true;
        // Debug.Log("CreateMineral _currentTargetPoint == null");
        return;
      }
      for (var i = 0; i < _consumptionMineralTypes.Count; i++)
      {
        _productionCount[i] -= 1;
      }

      _isMessageShowed = false;
      _startTime = Time.time;
      _currentMineral = _gameFactory.CreateMineral(_mineralPrefab, _startPointPosition.position, Quaternion.identity);
      _currentMineral.transform.SetParent(_currentTargetPoint.transform);
      _currentMineral.CollectingZoneCollider.enabled = false;
      _currentTime = 0f;
    }

    private float CountPartOfJourney(float startTime, float journeyLength)
    {
      // Debug.Log($"Time.time = {Time.time}, startTime = {startTime}");
      float distanceCovered = _mineralFlightSpeed;
      float fractionOfJourney = distanceCovered / journeyLength;
      // Debug.Log($"distanceCovered = {distanceCovered}, journeyLength = {journeyLength}, fractionOfJourney = {fractionOfJourney}");
      return fractionOfJourney;
    }

    private void SelectUIMessage()
    {
      _uiService.ShowMessage(_productionMineralType == MineralType.Charcoal
        ? Constants.CharcoalStorageProductionMessage
        : Constants.PlankStorageProductionMessage);
    }
  }
}