using System.Collections;
using System.Collections.Generic;
using CodeBase.Production;
using UnityEngine;

namespace CodeBase.Player
{
  public class UnloadingMinerals : MonoBehaviour
  {
    [SerializeField] private TriggerObserver _triggerObserver;
    [SerializeField] private CollectingMinerals _collectingMinerals;
    [SerializeField] private LayerMask _consumptionMask;
    
    private Transform _consumptionStartingPoint;
    private MineralsConsumption _mineralsConsumption;
    private List<Coroutine> _coroutines = new();
    
    private void Start()
    {
      _triggerObserver.TriggerEnter += TriggerEnter;
      _triggerObserver.TriggerExit += TriggerExit;
    }

    private void TriggerEnter(Collider obj)
    {
      if ((_consumptionMask.value & (1 << obj.gameObject.layer)) != 0)
      {
        _mineralsConsumption = obj.GetComponent<MineralsConsumption>();
        _consumptionStartingPoint = _mineralsConsumption.StartingPoint;
        _collectingMinerals.ClearCoroutineList();
        for (var i = _collectingMinerals.AllMinerals.Count - 1; i >= 0; i--)
        {
          if (_mineralsConsumption.ConsumptionStorageMinerals.Count >= _mineralsConsumption.MaxStorageCapacity)
          {
            return;
          }
          _collectingMinerals.MineralCount -= 1;
          GameObject lastMineral = _collectingMinerals.AllMinerals[i];
          // _mineralsConsumption.ConsumptionStorageMinerals.Push(lastMineral);
          _mineralsConsumption.ConsumptionStorageMinerals.Add(lastMineral);
          _collectingMinerals.AllMinerals.Remove(lastMineral);
          _coroutines.Add(StartCoroutine(LaunchMineral(lastMineral, _consumptionStartingPoint, i)));
        }
      }
    }

    private void TriggerExit(Collider obj)
    {
    }

    private IEnumerator LaunchMineral(GameObject mineralInBag, Transform storagePoint, int count, float countTime = 0.02f)
    {
      yield return new WaitForSeconds(0.2f);
      float coroutineTime = 3f;
      float startTime = Time.time;
      
      mineralInBag.transform.SetParent(_mineralsConsumption.transform);
      
      
      while (coroutineTime > 0)
      {
        coroutineTime -= countTime;
        Vector3 targetPosition = CountTargetPosition(storagePoint, count);
        float journeyLength = Vector3.Distance(mineralInBag.transform.position,targetPosition);
        float fractionOfJourney = CountPartOfJourney(startTime, journeyLength);

        mineralInBag.transform.position = Vector3.Lerp(mineralInBag.transform.position, targetPosition, fractionOfJourney);
        if (mineralInBag.transform.position == targetPosition) ExitCoroutine(mineralInBag);

        yield return new WaitForSeconds(countTime);
      }
    }

    private float CountPartOfJourney(float startTime, float journeyLength)
    {
      float distanceCovered = (Time.time - startTime) * _collectingMinerals.MineralFlightSpeed;
      float fractionOfJourney = distanceCovered / journeyLength;
      return fractionOfJourney;
    }

    private Vector3 CountTargetPosition(Transform storagePoint, float bagMineralCount)
    {
      Vector3 targetPosition = storagePoint.position;
      targetPosition.x -= bagMineralCount + 0.1f;
      // Debug.Log($"bagMineralCount = {bagMineralCount}, targetPosition = {targetPosition}");
      // Debug.Log($"{_mineralsConsumption.MaxStorageCapacity / _mineralsConsumption.MaxStorageLines}_mineralsConsumption.MaxStorageCapacity / _mineralsConsumption.MaxStorageLines");
      if (_mineralsConsumption.ConsumptionStorageMinerals.Count > _mineralsConsumption.MaxStorageCapacity / _mineralsConsumption.MaxStorageLines)
      {
        // Debug.Log("if");
        targetPosition.z -= (float)_mineralsConsumption.MaxStorageLines / 10 + 0.2f;
      }
      
      // Debug.Log($"count = {count}, targetPosition = {targetPosition}");
      return targetPosition;
    }

    private void ExitCoroutine(GameObject mineralInBack)
    {
      int num = _mineralsConsumption.ConsumptionStorageMinerals.FindIndex(mineral => mineralInBack.gameObject == mineral);
      StopCoroutine(_coroutines[num]);
      for (var i = _mineralsConsumption.ConsumptionStorageMinerals.Count - 1; i >= 0; i--)
      {

        // if (_mineralsConsumption.ConsumptionStorageMinerals.Peek() == mineralInBack)
        // {
        //   
        //   Debug.Log($"i = {i}");
        //   StopCoroutine(_coroutines[i]);
        // }
      }
    }

  }
}