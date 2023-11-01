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
      if (_collectingMinerals.AllMinerals.Count <= 0) return;

      MineralStates mineral = _collectingMinerals.AllMinerals.Last();
      StoragePoint targetPoint = _mineralsConsumption.StoragePoints.FirstOrDefault(point => !point.IsInsideStorage);
      if (targetPoint == null)
      {
        //todo: Need test
        _startTime = Time.time;
        return;
      }
      
      mineral.transform.SetParent(targetPoint.transform);
        
      // Debug.Log($"targetPoint.transform.position = {targetPoint.transform.position}");
      float journeyLength = Vector3.Distance(mineral.transform.position,
        targetPoint.transform.position);
        
      float fractionOfJourney = CountPartOfJourney(_startTime, journeyLength);
      mineral.transform.position =
        Vector3.Lerp(mineral.transform.position, targetPoint.transform.position, fractionOfJourney);
        
      if (mineral.transform.position == targetPoint.transform.position)
      {
        // Debug.Log($"targetPoint.transform.position = {targetPoint.transform.position}");
        _collectingMinerals.DeleteMineralFromBag(mineral);
        targetPoint.IsInsideStorage = true;
        _mineralsConsumption.AddMineralToConsumption(mineral, targetPoint);
      }
    }
    private float CountPartOfJourney(float startTime, float journeyLength)
    {
      float distanceCovered = (Time.time - startTime) * _mineralFlightSpeed;
      float fractionOfJourney = distanceCovered / journeyLength;
      return fractionOfJourney;
    }
  }
}