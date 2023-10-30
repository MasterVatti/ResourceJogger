using CodeBase.Player;
using UnityEngine;

namespace CodeBase.Minerals
{
  public class CollectingZone : MonoBehaviour
  {
    [SerializeField] private TriggerObserver _triggerObserver;
    [SerializeField] private SphereCollider _collectingZoneCollider;

    private bool _isCollected;

    private void Start()
    {
      _triggerObserver.TriggerEnter += TriggerEnter;
      _triggerObserver.TriggerExit += TriggerExit;
    }

    private void TriggerEnter(Collider obj)
    {
      obj.GetComponent<CollectingMinerals>().AddNewItem(_collectingZoneCollider, transform);
    }

    private void TriggerExit(Collider obj)
    {
    }
  }
}