using System.Collections;
using System.Linq;
using UnityEngine;

namespace CodeBase.Production
{
  public class MineralsProduction : MonoBehaviour
  {
    [SerializeField] private int _maxStorage;
    [SerializeField] private MineralsConsumption _mineralsConsumption;
    [SerializeField] private Transform _targetPoint;
    [SerializeField] private float _makeMineralTime;
    [SerializeField] private float _mineralFlightSpeed;
    private int _consumptionMineralCount;

    private void Start()
    {
      // _mineralsConsumption.MineralAdded += OnMineralAdded;
    }

    private void OnMineralAdded(GameObject mineral)
    {
      // int consumptionMineralCount = _mineralsConsumption.ConsumptionStorageMinerals.Count;
      // int consumptionMineralCount = _mineralsConsumption.ConsumptionStorageMinerals.Count;
      _consumptionMineralCount += 1;
      // if (_consumptionMineralCount >= _mineralsConsumption.MaxStorageCapacity) return;
      _mineralsConsumption.ConsumptionStorageMinerals.Remove(mineral);
      // Debug.Log($"Before Wait _mineralsConsumption.ConsumptionMineralCount = {_mineralsConsumption.ConsumptionMineralCount}");
      StartCoroutine(Wait(_consumptionMineralCount, mineral));
      
    }
    
    private IEnumerator LaunchMineral(Transform mineralTransform, float countTime = 0.02f)
    {
      mineralTransform.transform.SetParent(transform);
      float time = _makeMineralTime;
      float startTime = Time.time;
      float journeyLength = Vector3.Distance(mineralTransform.position, _targetPoint.position);

      while (time > 0f)
      {
        time -= countTime;
        float fractionOfJourney = CountPartOfJourney(startTime, journeyLength);

        mineralTransform.position = Vector3.Lerp(mineralTransform.position, _targetPoint.position, fractionOfJourney);
        if (mineralTransform.position == _targetPoint.position) ExitCoroutine(mineralTransform);

        yield return new WaitForSeconds(countTime);
      }
    }

    private float CountPartOfJourney(float startTime, float journeyLength)
    {
      float distanceCovered = (Time.time - startTime) * _mineralFlightSpeed;
      float fractionOfJourney = distanceCovered / journeyLength;
      return fractionOfJourney;
    }

    // private Vector3 CountTargetPosition(int count)
    // {
    //   Vector3 targetPosition = _parentMineralObject.position;
    //   targetPosition.y = count * 0.25f;
    //   return targetPosition;
    // }

    private void ExitCoroutine(Transform mineralTransform)
    {
      mineralTransform.gameObject.SetActive(false);
      
      // Debug.Log("ExitCoroutine");
      
     
      
      // mineralTransform.SetParent(_parentMineralObject);
      // int num = _allMinerals.FindIndex(mineral => mineralTransform.gameObject == mineral);
      // StopCoroutine(_coroutines[num]);
    }

    private IEnumerator Wait(int consumptionMineralCount, GameObject mineral)
    {
      // Debug.Log($"Middle Wait _consumptionMineralCount = {_consumptionMineralCount}");
      yield return new WaitForSeconds(consumptionMineralCount + _makeMineralTime);
      _mineralsConsumption.ConsumptionMineralCount -= 1;
      _consumptionMineralCount -= 1;
      consumptionMineralCount = _mineralsConsumption.ConsumptionStorageMinerals.Count;
      // Debug.Log($"After Wait _consumptionMineralCount = {_mineralsConsumption.ConsumptionMineralCount}");
      StartCoroutine(LaunchMineral(mineral.transform));
    }
  }
}