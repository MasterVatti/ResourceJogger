using System.Linq;
using CodeBase.Minerals;
using CodeBase.Production;
using UnityEngine;

namespace CodeBase.Player
{
  public class UnloadingMinerals : MonoBehaviour
  {
    [SerializeField] private TriggerObserver _triggerObserver;
    [SerializeField] private CollectingMinerals _collectingMinerals;
    [SerializeField] private LayerMask _consumptionMask;
    [SerializeField] private float _mineralFlightSpeed;

    private MineralsConsumption _mineralsConsumption;
    private bool _isEnteredConsumptionZone;
    private bool _isFirstEnteredTrigger;
    private float _startTime;

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
        _isEnteredConsumptionZone = true;
        SetInitialValues(obj);
      }
    }

    private void OnTriggerExit(Collider obj)
    {
      if ((_consumptionMask.value & (1 << obj.gameObject.layer)) != 0)
      {
        _isEnteredConsumptionZone = false;
        _isFirstEnteredTrigger = false;
      }
    }

    private void FixedUpdate()
    {
      if (!_isEnteredConsumptionZone || _collectingMinerals.AllBagMinerals.Count <= 0) return;
      if (!(_mineralsConsumption.ConsumeMineralType.Any(type => type == _collectingMinerals.AllBagMinerals[0].MineralType))) return;

      MineralStates mineral = _collectingMinerals.AllBagMinerals.Last();
      StoragePoint targetPoint = _mineralsConsumption.StoragePoints.FirstOrDefault(point => !point.IsInsideStorage);
      if (targetPoint == null)
      {
        _startTime = Time.time;
        return;
      }

      UnloadMineral(mineral, targetPoint);
    }

    private void UnloadMineral(MineralStates mineral, StoragePoint targetPoint)
    {
      mineral.transform.SetParent(targetPoint.transform);
      MoveMineral(mineral, targetPoint);

      if (mineral.transform.position == targetPoint.transform.position)
      {
        _collectingMinerals.DeleteMineralFromBag(mineral);
        targetPoint.IsInsideStorage = true;
        _mineralsConsumption.AddMineralToConsumption(mineral, targetPoint);
      }
    }

    private void MoveMineral(MineralStates mineral, StoragePoint targetPoint)
    {
      float journeyLength = Vector3.Distance(mineral.transform.position, targetPoint.transform.position);
      float fractionOfJourney = CountPartOfJourney(_startTime, journeyLength);
      mineral.transform.position = Vector3.Lerp(mineral.transform.position, targetPoint.transform.position, fractionOfJourney);
    }

    private float CountPartOfJourney(float startTime, float journeyLength)
    {
      float distanceCovered = (Time.time - startTime) * _mineralFlightSpeed;
      float fractionOfJourney = distanceCovered / journeyLength;
      return fractionOfJourney;
    }

    private void SetInitialValues(Collider obj)
    {
      _mineralsConsumption = obj.GetComponent<MineralsConsumption>();
      _startTime = Time.time;
    }
  }
}