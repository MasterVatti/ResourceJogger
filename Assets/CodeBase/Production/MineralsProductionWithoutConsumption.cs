using System;
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
  public class MineralsProductionWithoutConsumption : MonoBehaviour
  {
    [SerializeField] private List<StoragePoint> _storagePoints;
    [SerializeField] private MineralStates _mineralPrefab;
    [SerializeField] private Transform _startPointPosition;
    [SerializeField] private float _mineralFlightSpeed;
    [SerializeField] private float _cooldown;
    
    private IGameFactory _gameFactory;
    private MineralStates _currentMineral;
    private StoragePoint _currentTargetPoint;
    private int _maxStorageCount;
    private float _currentCooldownTime;
    private float _startTime;
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
      _currentTargetPoint = _storagePoints.FirstOrDefault(point => !point.IsInsideStorage);
      _currentMineral = _gameFactory.CreateMineral(_mineralPrefab, _startPointPosition.position, Quaternion.identity);
      _currentMineral.CollectingZoneCollider.enabled = false;
      _currentMineral.transform.SetParent(_currentTargetPoint.transform);
    }
    
    private void Update()
    {
      _currentCooldownTime += Time.deltaTime;
    }

    private void FixedUpdate()
    {
      if (_currentCooldownTime < _cooldown)
      { 
        return;
      }

      if (_currentTargetPoint == null)
      {
        Debug.Log("ShowMessage");
        
        _currentCooldownTime = 0f;
        CreateMineral();
        return;
      }
      
      float currentJourneyLength = Vector3.Distance(_currentMineral.transform.position, _currentTargetPoint.transform.position);
      float fractionOfJourney = CountPartOfJourney(_startTime, currentJourneyLength);

      _currentMineral.transform.position = Vector3.Lerp(_currentMineral.transform.position, _currentTargetPoint.transform.position, fractionOfJourney);
      
      if (_currentMineral.transform.position == _currentTargetPoint.transform.position)
      {
        _currentTargetPoint.IsInsideStorage = true;
        _currentMineral.CollectingZoneCollider.enabled = true;
        CreateMineral();
      }
      
    }
    
    private void CreateMineral()
    {
      _currentTargetPoint = _storagePoints.FirstOrDefault(point => !point.IsInsideStorage);
      if (_currentTargetPoint == null)
      {
        if (!_isMessageShowed) _uiService.ShowMessage(Constants.WoodStorageProductionMessage);
        _isMessageShowed = true;
        return;
      }

      _isMessageShowed = false;
      _startTime = Time.time;
      _currentMineral = _gameFactory.CreateMineral(_mineralPrefab, _startPointPosition.position, Quaternion.identity);
      _currentMineral.transform.SetParent(_currentTargetPoint.transform);
      _currentMineral.CollectingZoneCollider.enabled = false;
    }
    
    private float CountPartOfJourney(float startTime, float journeyLength)
    {
      // Debug.Log($"Time.time = {Time.time}, startTime = {startTime}");
      float distanceCovered = (Time.time - startTime) * _mineralFlightSpeed;
      float fractionOfJourney = distanceCovered / journeyLength;
      // Debug.Log($"distanceCovered = {distanceCovered}, journeyLength = {journeyLength}, fractionOfJourney = {fractionOfJourney}");
      return fractionOfJourney;
    }
  }
}