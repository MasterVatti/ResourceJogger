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
  public class MineralsProduction : MonoBehaviour
  {
    [SerializeField] private MineralStates _mineralPrefab;
    [SerializeField] private Transform _startPointPosition;
    [SerializeField] private List<MineralType> _consumptionMineralTypes;
    [SerializeField] private MineralType _productionMineralType;
    [SerializeField] private float _mineralFlightSpeed;
    [SerializeField] private float _cooldown;
    [SerializeField] private List<StoragePoint> _storagePoints;

    private IGameFactory _gameFactory;
    private IUIService _uiService;
    private MineralStates _currentMineral;
    private StoragePoint _currentTargetPoint;
    private int[] _productionCount;
    private float _currentCooldownTime;
    private bool _isFirstAddedMineral = true;
    private bool _isMessageShowed;

    [Inject]
    public void Constructor(IGameFactory gameFactory, IUIService uiService)
    {
      _gameFactory = gameFactory;
      _uiService = uiService;
    }

    private void Start()
    {
      _productionCount = new int[_consumptionMineralTypes.Count];
      _currentTargetPoint = _storagePoints.FirstOrDefault(point => !point.IsInsideStorage);
      SetCurrentMineral();
    }

    private void Update()
    {
      _currentCooldownTime += Time.deltaTime;
    }

    private void FixedUpdate()
    {
      if (_productionCount.Any(counter => counter <= 0))
      {
        _currentCooldownTime = 0f;
        return;
      }

      if (_currentTargetPoint == null)
      {
        CreateMineral();
        return;
      }

      if (_currentCooldownTime < _cooldown) return;

      MoveMineral();
      if (_currentMineral.transform.position == _currentTargetPoint.transform.position)
      {
        _currentTargetPoint.IsInsideStorage = true;
        _currentMineral.CollectingZoneCollider.enabled = true;
        CreateMineral();
      }
    }

    private void MoveMineral()
    {
      float currentJourneyLength =
        Vector3.Distance(_currentMineral.transform.position, _currentTargetPoint.transform.position);
      float fractionOfJourney = CountPartOfJourney(currentJourneyLength);
      _currentMineral.transform.position = Vector3.Lerp(_currentMineral.transform.position,
        _currentTargetPoint.transform.position, fractionOfJourney);
    }

    public void AddMineralToProduction(MineralStates mineral)
    {
      if (_isFirstAddedMineral) _isFirstAddedMineral = false;

      for (var i = 0; i < _consumptionMineralTypes.Count; i++)
        if (mineral.MineralType == _consumptionMineralTypes[i])
          _productionCount[i] += 1;
    }

    private void CreateMineral()
    {
      _currentTargetPoint = _storagePoints.FirstOrDefault(point => !point.IsInsideStorage);
      if (_currentTargetPoint == null)
      {
        if (!_isMessageShowed) SelectUIMessage();
        _isMessageShowed = true;
        return;
      }

      for (var i = 0; i < _consumptionMineralTypes.Count; i++) _productionCount[i] -= 1;

      _isMessageShowed = false;
      SetCurrentMineral();
      _currentCooldownTime = 0f;
    }

    private float CountPartOfJourney(float journeyLength)
    {
      float fractionOfJourney = _mineralFlightSpeed / journeyLength;
      return fractionOfJourney;
    }

    private void SelectUIMessage()
    {
      _uiService.ShowMessage(_productionMineralType == MineralType.Charcoal
        ? Constants.CharcoalStorageProductionMessage
        : Constants.PlankStorageProductionMessage);
    }

    private void SetCurrentMineral()
    {
      _currentMineral = _gameFactory.CreateMineral(_mineralPrefab, _startPointPosition.position, Quaternion.identity);
      _currentMineral.transform.SetParent(_currentTargetPoint.transform);
      _currentMineral.CollectingZoneCollider.enabled = false;
    }
  }
}