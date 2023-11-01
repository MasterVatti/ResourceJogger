using UnityEngine;

namespace CodeBase.Minerals
{
  public class MineralStates : MonoBehaviour
  { 
    public bool IsInsideBag { get; set; }
    public MineralType MineralType => _mineralType;
    public SphereCollider CollectingZoneCollider
    {
      get { return _collectingZoneCollider; }
      set { _collectingZoneCollider = value; }
    }

    [SerializeField] private SphereCollider _collectingZoneCollider;
    [SerializeField] private MineralType _mineralType;
  }
}