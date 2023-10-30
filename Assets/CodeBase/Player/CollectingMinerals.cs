using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Player
{
  public class CollectingMinerals : MonoBehaviour
  {
    public int MineralCount { get; set; }
    public float MineralFlightSpeed => _mineralFlightSpeed;
    public List<GameObject> AllMinerals => _allMinerals;

    [SerializeField] private Transform _parentMineralObject;
    [SerializeField] private float _mineralFlightSpeed;
    [SerializeField] private int _maxMineralCount;

    private List<Coroutine> _coroutines = new();
    private List<GameObject> _allMinerals = new();

    public void AddNewItem(SphereCollider collectingZoneCollider, Transform mineralTransform)
    {
      if (_allMinerals.Count >= _maxMineralCount) return;
      MineralCount += 1;
      collectingZoneCollider.enabled = false;
      _allMinerals.Add(mineralTransform.gameObject);
      _coroutines.Add(StartCoroutine(MoveMineral(mineralTransform)));
    }

    public void ClearCoroutineList()
    {
      _coroutines.Clear();
    }

    private IEnumerator MoveMineral(Transform mineralTransform, float countTime = 0.02f)
    {
      float time = 3f;
      float startTime = Time.time;
      float journeyLength = Vector3.Distance(mineralTransform.position, _parentMineralObject.position);
      int count = MineralCount;

      while (time > 0f)
      {
        time -= countTime;
        float fractionOfJourney = CountPartOfJourney(startTime, journeyLength);
        Vector3 targetPosition = CountTargetPosition(count);

        mineralTransform.position = Vector3.Lerp(mineralTransform.position, targetPosition, fractionOfJourney);
        if (mineralTransform.position == targetPosition) ExitCoroutine(mineralTransform);

        yield return new WaitForSeconds(countTime);
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
      targetPosition.y = count * 0.25f;
      return targetPosition;
    }

    private void ExitCoroutine(Transform mineralTransform)
    {
      mineralTransform.SetParent(_parentMineralObject);
      int num = _allMinerals.FindIndex(mineral => mineralTransform.gameObject == mineral);
      StopCoroutine(_coroutines[num]);
    }
  }
}