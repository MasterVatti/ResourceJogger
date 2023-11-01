using System.Collections.Generic;
using System.Linq;
using CodeBase.Minerals;
using CodeBase.Player;
using UnityEngine;

namespace CodeBase.Production
{
  public class MineralsConsumption : MonoBehaviour
  {
    public List<StoragePoint> StoragePoints => _storagePoints;

    [SerializeField] private Transform _targetPoint;
    [SerializeField] private MineralsProduction _mineralsProduction;
    [SerializeField] private float _mineralFlightSpeed;
    [SerializeField] private float _cooldown;
    [SerializeField] private List<StoragePoint> _storagePoints;

    private List<MineralStates> _mineralStates = new();
    private List<StoragePoint> _tempPoints = new();
    private List<float> _journeyLengths = new();

    private MineralStates _currentMineral;
    private StoragePoint _currentTempPoint;
    private float _currentJourneyLength;
    private float _startTime;
    private float _currentCooldownTime;

    private void Update()
    {
      _currentCooldownTime += Time.deltaTime;
    }

    private void FixedUpdate()
    {
      if (_mineralStates.Count <= 0 || _currentMineral == null || _currentCooldownTime < _cooldown) return;

      MoveMineral();
    }

    public void AddMineralToConsumption(MineralStates mineral, StoragePoint storagePoint)
    {
      float journeyLength = Vector3.Distance(mineral.transform.position, _targetPoint.transform.position);
      if (_mineralStates.Count <= 0)
      {
        _currentMineral = mineral;
        _currentTempPoint = storagePoint;
        _currentJourneyLength = journeyLength;
        _startTime = Time.time;
      }

      _currentCooldownTime = 0f;
      _mineralStates.Add(mineral);
      _tempPoints.Add(storagePoint);
      _journeyLengths.Add(journeyLength);
    }

    private void MoveMineral()
    {
      float fractionOfJourney = CountPartOfJourney(_startTime, _currentJourneyLength);
      _currentMineral.transform.position =
        Vector3.Lerp(_currentMineral.transform.position, _targetPoint.transform.position, fractionOfJourney);
      if (_currentMineral.transform.position == _targetPoint.transform.position) SetCurrentValues();
    }

    private void SetCurrentValues()
    {
      _currentTempPoint.IsInsideStorage = false;
      _mineralStates.Remove(_currentMineral);
      _tempPoints.Remove(_currentTempPoint);
      _journeyLengths.Remove(_currentJourneyLength);

      _mineralsProduction.AddMineralToProduction(_currentMineral);
      Destroy(_currentMineral.gameObject);

      _currentMineral = _mineralStates.FirstOrDefault();
      _currentTempPoint = _tempPoints.FirstOrDefault();
      _currentJourneyLength = _journeyLengths.FirstOrDefault();

      _currentCooldownTime = 0f;
      _startTime = Time.time;
    }

    private float CountPartOfJourney(float startTime, float journeyLength)
    {
      float distanceCovered = (Time.time - startTime) * _mineralFlightSpeed;
      float fractionOfJourney = distanceCovered / journeyLength;
      return fractionOfJourney;
    }
  }
}