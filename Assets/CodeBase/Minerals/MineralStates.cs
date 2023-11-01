using UnityEngine;

namespace CodeBase.Minerals
{
  public class MineralStates : MonoBehaviour
  {
    public bool IsInsideBag { get; set; }
    public MineralType MineralType => _mineralType;
    public SphereCollider CollectingZoneCollider => _collectingZoneCollider;

    [SerializeField] private SphereCollider _collectingZoneCollider;
    [SerializeField] private MineralType _mineralType;
  }
}