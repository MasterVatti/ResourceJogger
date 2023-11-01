using System.Threading.Tasks;
using CodeBase.Minerals;
using UnityEngine;

namespace CodeBase.Infrastructure.Factory
{
  public interface IGameFactory
  {
    Task<GameObject> CreatePlayer(Vector3 at);
    Task<GameObject> CreateHud();
    GameObject PlayerGameObject { get; }
    MineralStates CreateMineral(MineralStates prefab, Vector3 position, Quaternion rotation);
  }
}