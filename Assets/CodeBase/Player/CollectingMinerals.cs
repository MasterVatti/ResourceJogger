using System.Collections.Generic;
using System.Linq;
using CodeBase.Minerals;
using UnityEngine;

namespace CodeBase.Player
{
  public class CollectingMinerals : MonoBehaviour
  {
    public List<MineralStates> AllBagMinerals => _allBagMinerals;

    [SerializeField] private Transform _parentMineralObject;
    [SerializeField] private float _mineralFlightSpeed;
    [SerializeField] private int _maxMineralCount;

    private List<MineralStates> _allBagMinerals = new();
    private float _journeyLength;
    private float _startTime;

    private void FixedUpdate()
    {
      if (_allBagMinerals.Count <= 0 || _allBagMinerals.All(state => state.IsInsideBag)) return;

      for (int i = 0; i < _allBagMinerals.Count; i++) AddMineralToBag(i);
    }

    public void AddNewItem(MineralStates mineralStates, SphereCollider collectingZoneCollider)
    {
      if (_allBagMinerals.Count >= _maxMineralCount) return;
      if (_allBagMinerals.Count > 0) if (mineralStates.MineralType != _allBagMinerals[0].MineralType) return;

      collectingZoneCollider.enabled = false;
      StoragePoint storagePoint = mineralStates.GetComponentInParent<StoragePoint>();
      if (storagePoint != null) storagePoint.IsInsideStorage = false;

      SetInitialValues(mineralStates);
    }

    public void DeleteMineralFromBag(MineralStates mineral) => _allBagMinerals.Remove(mineral);

    private void AddMineralToBag(int i)
    {
      if (!_allBagMinerals[i].IsInsideBag)
      {
        float fractionOfJourney = CountPartOfJourney(_startTime, _journeyLength);
        Vector3 targetPosition = CountTargetPosition(i);
        _allBagMinerals[i].transform.position =
          Vector3.Lerp(_allBagMinerals[i].transform.position, targetPosition, fractionOfJourney);

        if (_allBagMinerals[i].transform.position == targetPosition)
        {
          _allBagMinerals[i].transform.SetParent(_parentMineralObject);
          _allBagMinerals[i].IsInsideBag = true;
        }
      }
    }

    private float CountPartOfJourney(float startTime, float journeyLength)
    {
      float distanceCovered = (Time.time - startTime) * _mineralFlightSpeed;
      float fractionOfJourney = distanceCovered / journeyLength;
      return fractionOfJourney;
    }

    private Vector3 CountTargetPosition(int count)
    {
      Vector3 targetPosition = _parentMineralObject.position;
      targetPosition.y = count * 0.3f + 0.2f;
      return targetPosition;
    }

    private void SetInitialValues(MineralStates mineralStates)
    {
      _allBagMinerals.Add(mineralStates);
      _journeyLength = Vector3.Distance(mineralStates.transform.position, _parentMineralObject.position);
      _startTime = Time.time;
    }
  }
}