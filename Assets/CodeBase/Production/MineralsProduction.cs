using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Infrastructure.Factory;
using CodeBase.Minerals;
using CodeBase.Player;
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

    private IGameFactory _gameFactory;
    private List<MineralStates> _mineralStates = new();
    private int _consumptionMineralCount;
    private int _productionCount;
    private float _currentTime;
    private float _startTime;
    private MineralStates _currentMineral;
    private StoragePoint _currentTargetPoint;
    private int _storageCount;
    private float _testCooldown = 0.2f;
    private bool _isFirstAddedMineal;

    [Inject]
    public void Constructor(IGameFactory gameFactory)
    {
      _gameFactory = gameFactory;
    }

    private void Start()
    {
      _currentTargetPoint = _storagePoints.FirstOrDefault(point => !point.IsInsideStorage);
      // _currentMineral = _gameFactory.CreateMineral(_mineralPrefab, _currentTargetPoint.transform);
      _currentMineral = Instantiate(_mineralPrefab, _startPointPosition.position, Quaternion.identity);
      _currentMineral.CollectingZoneCollider.enabled = false;
      _currentMineral.transform.SetParent(_currentTargetPoint.transform);
    }

    private void Update()
    {
      _currentTime += Time.deltaTime;
    }
    
    private void FixedUpdate()
    {
      
      if (_productionCount <= 0)
      {
        _currentTime = 0f;
        return;
      }

      if (_currentTargetPoint == null)
      {
        Debug.Log("FixedUpdate _currentTargetPoint == null");
        _currentTime = 0f;
        CreateMineral();
        return;
      }
      
      if (_currentTime < _testCooldown)
      { 
        return;
      }

      float currentJourneyLength = Vector3.Distance(_currentMineral.transform.position, _currentTargetPoint.transform.position);
      float fractionOfJourney = CountPartOfJourney(_startTime, currentJourneyLength);

      _currentMineral.transform.position = Vector3.Lerp(_currentMineral.transform.position, _currentTargetPoint.transform.position, fractionOfJourney);
      
      if (_currentMineral.transform.position == _currentTargetPoint.transform.position)
      {
        _currentTargetPoint.IsInsideStorage = true;
        _productionCount -= 1;
        // _storageCount += 1;
        _currentMineral.CollectingZoneCollider.enabled = true;
        CreateMineral();
        
        // _tempPoints.Remove(_currentTempPoint);
        // _mineralStates.Remove(_currentMineral);
        // _journeyLengths.Remove(_currentJourneyLength);
        // _currentMineral = _mineralStates.LastOrDefault();
        // _currentTempPoint = _tempPoints.LastOrDefault();
        // _currentJourneyLength = _journeyLengths.LastOrDefault();



      }
    }

    public void AddMineralToProduction()
    {
      if (_isFirstAddedMineal)
      {
        _isFirstAddedMineal = false;
        _startTime = Time.time;
      }
      _productionCount += 1;
      // if (_mineralStates.Count <= 0)
      // {
      //   // _currentMineral = mineral;
      //   // _currentTempPoint = storagePoint;
      //   // _currentJourneyLength = Vector3.Distance(mineral.transform.position, _targetPoint.transform.position);
      // }
      // _mineralStates.Add(mineral);
      // _tempPoints.Add(storagePoint);
      // _startTime = Time.time;
      // _journeyLengths.Add(Vector3.Distance(mineral.transform.position, _targetPoint.transform.position));
    }

    private void CreateMineral()
    {
      _currentTargetPoint = _storagePoints.FirstOrDefault(point => !point.IsInsideStorage);
      if (_currentTargetPoint == null)
      {
        Debug.Log("CreateMineral _currentTargetPoint == null");
        return;
      }

      _startTime = Time.time;
      _currentMineral = Instantiate(_mineralPrefab, _startPointPosition.position, Quaternion.identity);
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
    
    // private float CountPartOfJourney(float startTime, float journeyLength)
    // {
    //   // Debug.Log($"Time.time = {Time.time}, startTime = {startTime}");
    //   float distanceCovered = (Time.time - startTime) * _mineralFlightSpeed;
    //   float fractionOfJourney = distanceCovered / journeyLength;
    //   // Debug.Log($"distanceCovered = {distanceCovered}, journeyLength = {journeyLength}, fractionOfJourney = {fractionOfJourney}");
    //   return fractionOfJourney;
    // }

    // private void OnMineralAdded(GameObject mineral)
    // {
    //   // int consumptionMineralCount = _mineralsConsumption.ConsumptionStorageMinerals.Count;
    //   // int consumptionMineralCount = _mineralsConsumption.ConsumptionStorageMinerals.Count;
    //   _consumptionMineralCount += 1;
    //   // if (_consumptionMineralCount >= _mineralsConsumption.MaxStorageCapacity) return;
    //   _mineralsConsumption.ConsumptionStorageMinerals.Remove(mineral);
    //   // Debug.Log($"Before Wait _mineralsConsumption.ConsumptionMineralCount = {_mineralsConsumption.ConsumptionMineralCount}");
    //   StartCoroutine(Wait(_consumptionMineralCount, mineral));
    //   
    // }
    
    // private IEnumerator LaunchMineral(Transform mineralTransform, float countTime = 0.02f)
    // {
    //   mineralTransform.transform.SetParent(transform);
    //   float time = _makeMineralTime;
    //   float startTime = Time.time;
    //   float journeyLength = Vector3.Distance(mineralTransform.position, _targetPoint.position);
    //
    //   while (time > 0f)
    //   {
    //     time -= countTime;
    //     float fractionOfJourney = CountPartOfJourney(startTime, journeyLength);
    //
    //     mineralTransform.position = Vector3.Lerp(mineralTransform.position, _targetPoint.position, fractionOfJourney);
    //     if (mineralTransform.position == _targetPoint.position) ExitCoroutine(mineralTransform);
    //
    //     yield return new WaitForSeconds(countTime);
    //   }
    // }

    // private Vector3 CountTargetPosition(int count)
    // {
    //   Vector3 targetPosition = _parentMineralObject.position;
    //   targetPosition.y = count * 0.25f;
    //   return targetPosition;
    // }

    // private void ExitCoroutine(Transform mineralTransform)
    // {
    //   mineralTransform.gameObject.SetActive(false);
    //   
    //   // Debug.Log("ExitCoroutine");
    //   
    //  
    //   
    //   // mineralTransform.SetParent(_parentMineralObject);
    //   // int num = _allMinerals.FindIndex(mineral => mineralTransform.gameObject == mineral);
    //   // StopCoroutine(_coroutines[num]);
    // }

    // private IEnumerator Wait(int consumptionMineralCount, GameObject mineral)
    // {
    //   // Debug.Log($"Middle Wait _consumptionMineralCount = {_consumptionMineralCount}");
    //   yield return new WaitForSeconds(consumptionMineralCount + _makeMineralTime);
    //   _mineralsConsumption.ConsumptionMineralCount -= 1;
    //   _consumptionMineralCount -= 1;
    //   consumptionMineralCount = _mineralsConsumption.ConsumptionStorageMinerals.Count;
    //   // Debug.Log($"After Wait _consumptionMineralCount = {_mineralsConsumption.ConsumptionMineralCount}");
    //   StartCoroutine(LaunchMineral(mineral.transform));
    // }
  }
}