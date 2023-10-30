using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Player
{
  public class CollectingMinerals : MonoBehaviour
  {
    [SerializeField] private Transform _parentMineralObject;
    [SerializeField] private float _mineralFlightSpeed;

    private int _mineralCount;
    private List<Coroutine> _coroutines = new();
    private List<GameObject> _allMinerals = new();

    public void AddNewItem(Transform mineralTransform)
    {
      _mineralCount += 1;
      _allMinerals.Add(mineralTransform.gameObject);
      _coroutines.Add(StartCoroutine(MoveMineral(mineralTransform)));
    }

    private IEnumerator MoveMineral(Transform mineralTransform, float countTime = 0.02f)
    {
      float time = 4f;
      float startTime = Time.time;
      float journeyLength = Vector3.Distance(mineralTransform.position, _parentMineralObject.position);
      int count = _mineralCount;

      while (time > 0f)
      {
        time -= countTime;
        float fractionOfJourney = CountPartOfJourney(startTime, journeyLength);
        Vector3 targetPosition = CountTargetHeight(count);

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

    private Vector3 CountTargetHeight(int count)
    {
      Vector3 position = _parentMineralObject.position;
      Vector3 targetPosition = position;
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