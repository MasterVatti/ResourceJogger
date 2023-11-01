using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Minerals;
using UnityEngine;

namespace CodeBase.Player
{
  public class CollectingMinerals : MonoBehaviour
  {
    public int MineralCount { get; set; }
    public float MineralFlightSpeed => _mineralFlightSpeed;
    public List<MineralStates> AllMinerals => _allMinerals;

    [SerializeField] private Transform _parentMineralObject;
    [SerializeField] private float _mineralFlightSpeed;
    [SerializeField] private int _maxMineralCount;

    private List<Coroutine> _coroutines = new();
    private List<MineralStates> _allMinerals = new();
    private float _journeyLength;
    private float _startTime;

    public void AddNewItem(MineralStates mineralStates, SphereCollider collectingZoneCollider)
    {
      if (_allMinerals.Count >= _maxMineralCount) return;
      if (_allMinerals.Count > 0) if (mineralStates.MineralType != _allMinerals[0].MineralType) return;
      
      // MineralCount += 1;
      collectingZoneCollider.enabled = false;
      StoragePoint storagePoint = mineralStates.GetComponentInParent<StoragePoint>();
      if (storagePoint != null)
      {
        Debug.Log("storagePoint.IsInsideStorage = false");
        storagePoint.IsInsideStorage = false;
      }
      _allMinerals.Add(mineralStates);
      _journeyLength = Vector3.Distance(mineralStates.transform.position, _parentMineralObject.position);
      _startTime = Time.time;
      // Debug.Log($"!_allMinerals.First(state => state.IsInsideBag == false) = {!_allMinerals.First(state => state.IsInsideBag == false)}");
    }

    private void FixedUpdate()
    {
      if (_allMinerals.Count <= 0 || _allMinerals.All(state => state.IsInsideBag))
      {
        return;
      }

      for (int i = 0; i < _allMinerals.Count; i++)
      {
        if (!_allMinerals[i].IsInsideBag)
        {
          float fractionOfJourney = CountPartOfJourney(_startTime, _journeyLength);
          Vector3 targetPosition = CountTargetPosition(i);
          // Debug.Log($"i = {i}, targetPosition = {targetPosition}");
          _allMinerals[i].transform.position = Vector3.Lerp(_allMinerals[i].transform.position, targetPosition, fractionOfJourney);
          if (_allMinerals[i].transform.position == targetPosition)
          {
            _allMinerals[i].transform.SetParent(_parentMineralObject);
            _allMinerals[i].IsInsideBag = true;
          }
        }
      }
    }

    public void DeleteMineralFromBag(MineralStates mineral)
    {
      // mineral.IsInsideBag = false;
      _allMinerals.Remove(mineral);
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
  }
}