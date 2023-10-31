using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Minerals;
using CodeBase.Production;
using CodeBase.Services;
using UnityEngine;
using Zenject;

namespace CodeBase.Player
{
  public class UnloadingMinerals : MonoBehaviour
  {
    [SerializeField] private TriggerObserver _triggerObserver;
    [SerializeField] private CollectingMinerals _collectingMinerals;
    [SerializeField] private LayerMask _consumptionMask;
    [SerializeField] private float _mineralFlightSpeed;
    
    private Transform _consumptionStartingPoint;
    private MineralsConsumption _mineralsConsumption;
    private List<Coroutine> _coroutines = new();
    private List<GameObject> _minerals = new();
    private bool _isEnteredConsumptionZone;
    private bool _isFirstEnteredTrigger;
    private float _startTime;
    private int _counter;
    private List<float> _journeyLength = new();

    private void Start()
    {
      _triggerObserver.TriggerEnter += OnTriggerEnter;
      _triggerObserver.TriggerExit += OnTriggerExit;
    }

    private void OnTriggerEnter(Collider obj)
    {
      if ((_consumptionMask.value & (1 << obj.gameObject.layer)) != 0)
      {
        if (_isFirstEnteredTrigger) return;
        _isFirstEnteredTrigger = true;
        Debug.Log("OnTriggerEnter");
        _mineralsConsumption = obj.GetComponent<MineralsConsumption>();
        
        for (var i = _collectingMinerals.AllMinerals.Count - 1; i >= 0; i--)
        {
          // StoragePoint targetPoint = _mineralsConsumption.StoragePoints.First(point => !point.IsInsideStorage);
          
          // _journeyLength.Add(Vector3.Distance(_collectingMinerals.AllMinerals[i].transform.position,
          //   targetPoint.transform.position));
        }
        _startTime = Time.time;
        _isEnteredConsumptionZone = true;
      }
    }

    private void OnTriggerExit(Collider obj)
    {
      if ((_consumptionMask.value & (1 << obj.gameObject.layer)) != 0)
      {
        Debug.Log("OnTriggerExit");
        _isEnteredConsumptionZone = false;
        _isFirstEnteredTrigger = false;
      }


    }

    private void FixedUpdate()
    {
      if (!_isEnteredConsumptionZone) return;

      int mineralsCount = _collectingMinerals.AllMinerals.Count;
      int mineralsCounterBagPosition = 0;
      if (_mineralsConsumption.MineralStates.Count >= _mineralsConsumption.StoragePoints.Count) return;
      if (_mineralsConsumption.MineralStates.Count + mineralsCount >= _mineralsConsumption.StoragePoints.Count)
      {
        mineralsCount = Math.Abs(_mineralsConsumption.MineralStates.Count - _mineralsConsumption.StoragePoints.Count);
        mineralsCounterBagPosition = _collectingMinerals.AllMinerals.Count - mineralsCount;
      }
      
      for (int i = _collectingMinerals.AllMinerals.Count - 1; i >= mineralsCounterBagPosition; i--)
      {
        StoragePoint targetPoint = _mineralsConsumption.StoragePoints.FirstOrDefault(point => !point.IsInsideStorage);
        _collectingMinerals.AllMinerals[i].transform.SetParent(targetPoint.transform);
        
        // Debug.Log($"targetPoint.transform.position = {targetPoint.transform.position}");
        var journeyLength = (Vector3.Distance(_collectingMinerals.AllMinerals[i].transform.position,
        targetPoint.transform.position));
        
        float fractionOfJourney = CountPartOfJourney(_startTime, journeyLength);
        _collectingMinerals.AllMinerals[i].transform.position =
          Vector3.Lerp(_collectingMinerals.AllMinerals[i].transform.position, targetPoint.transform.position, fractionOfJourney);
        
        if (_collectingMinerals.AllMinerals[i].transform.position == targetPoint.transform.position)
        {
          // Debug.Log($"targetPoint.transform.position = {targetPoint.transform.position}");
          _mineralsConsumption.MineralStates.Add(_collectingMinerals.AllMinerals[i]);
          _collectingMinerals.DeleteMineralFromBag(_collectingMinerals.AllMinerals[i]);
          targetPoint.IsInsideStorage = true;
          // _counter += 1;
        }
      }
    }
    
    private float CountPartOfJourney(float startTime, float journeyLength)
    {
      float distanceCovered = (Time.time - startTime) * _mineralFlightSpeed;
      float fractionOfJourney = distanceCovered / journeyLength;
      return fractionOfJourney;
    }
    
    // private void TriggerEnter(Collider obj)
    // {
    //   if ((_consumptionMask.value & (1 << obj.gameObject.layer)) != 0)
    //   {
    //     _mineralsConsumption = obj.GetComponent<MineralsConsumption>();
    //     _consumptionStartingPoint = _mineralsConsumption.StartingPoint;
    //     // _collectingMinerals.ClearCoroutineList();
    //     for (var i = _collectingMinerals.AllMinerals.Count - 1; i >= 0; i--)
    //     {
    //       // Debug.Log($"ConsumptionStorageMinerals.Count = {_mineralsConsumption.ConsumptionStorageMinerals.Count}");
    //       if (_mineralsConsumption.ConsumptionStorageMinerals.Count >= _mineralsConsumption.MaxStorageCapacity)
    //       {
    //         return;
    //       }
    //       _mineralsConsumption.ConsumptionMineralCount += 1;
    //       // Debug.Log($"ConsumptionMineralCount = {_mineralsConsumption.ConsumptionMineralCount}");
    //       if (_mineralsConsumption.ConsumptionMineralCount >= _mineralsConsumption.MaxStorageCapacity) return;
    //       _collectingMinerals.MineralCount -= 1;
    //       MineralStates lastMineral = _collectingMinerals.AllMinerals[i];
    //       // _mineralsConsumption.ConsumptionStorageMinerals.Push(lastMineral);
    //       _mineralsConsumption.ConsumptionStorageMinerals.Add(lastMineral.gameObject);
    //       
    //       _collectingMinerals.AllMinerals.Remove(lastMineral);
    //       _coroutines.Add(StartCoroutine(LaunchMineral(lastMineral.gameObject, _consumptionStartingPoint, i)));
    //       _minerals.Add(lastMineral.gameObject);
    //     }
    //   }
    // }

    private void TriggerExit(Collider obj)
    {
    }

    // private IEnumerator LaunchMineral(GameObject mineralInBag, Transform storagePoint, int count, float countTime = 0.02f)
    // {
    //   yield return new WaitForSeconds(0.2f);
    //   float coroutineTime = 3f;
    //   float startTime = Time.time;
    //   
    //   mineralInBag.transform.SetParent(_mineralsConsumption.transform);
    //   
    //   
    //   while (coroutineTime > 0)
    //   {
    //     coroutineTime -= countTime;
    //     Vector3 targetPosition = CountTargetPosition(storagePoint, count);
    //     float journeyLength = Vector3.Distance(mineralInBag.transform.position,targetPosition);
    //     float fractionOfJourney = CountPartOfJourney(startTime, journeyLength);
    //
    //     mineralInBag.transform.position = Vector3.Lerp(mineralInBag.transform.position, targetPosition, fractionOfJourney);
    //     if (mineralInBag.transform.position == targetPosition) ExitCoroutine(mineralInBag);
    //
    //     yield return new WaitForSeconds(countTime);
    //   }
    // }
    //
    // private float CountPartOfJourney(float startTime, float journeyLength)
    // {
    //   float distanceCovered = (Time.time - startTime) * _collectingMinerals.MineralFlightSpeed;
    //   float fractionOfJourney = distanceCovered / journeyLength;
    //   return fractionOfJourney;
    // }
    //
    // private Vector3 CountTargetPosition(Transform storagePoint, float bagMineralCount)
    // {
    //   Vector3 targetPosition = storagePoint.position;
    //   targetPosition.x -= bagMineralCount + 0.1f;
    //   // Debug.Log($"bagMineralCount = {bagMineralCount}, targetPosition = {targetPosition}");
    //   // Debug.Log($"{_mineralsConsumption.MaxStorageCapacity / _mineralsConsumption.MaxStorageLines}_mineralsConsumption.MaxStorageCapacity / _mineralsConsumption.MaxStorageLines");
    //   if (_mineralsConsumption.ConsumptionStorageMinerals.Count > _mineralsConsumption.MaxStorageCapacity / _mineralsConsumption.MaxStorageLines)
    //   {
    //     // Debug.Log("if");
    //     targetPosition.z -= (float)_mineralsConsumption.MaxStorageLines / 10 + 0.2f;
    //   }
    //   
    //   // Debug.Log($"count = {count}, targetPosition = {targetPosition}");
    //   return targetPosition;
    // }
    //
    // private void ExitCoroutine(GameObject mineralInBack)
    // {
    //   // _mineralsConsumption.ConsumptionStorageMinerals.Add(mineralInBack);
    //   
    //
    //   // _mineralsConsumption.AddMineralToStorage(mineralInBack);
    //
    //   int num = _minerals.FindIndex(mineral => mineralInBack.gameObject == mineral);
    //   // Debug.Log($"ExitCoroutine num = {num}");
    //   StopCoroutine(_coroutines[num]);
    //   _coroutines.Remove(_coroutines[num]);
    //   _minerals.Remove(_minerals[num]);
    //   
    //
    // }

  }
}