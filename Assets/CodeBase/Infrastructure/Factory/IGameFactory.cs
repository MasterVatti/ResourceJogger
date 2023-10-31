using System.Threading.Tasks;
using CodeBase.Minerals;
using CodeBase.Services;
using UnityEngine;

namespace CodeBase.Infrastructure.Factory
{
  public interface IGameFactory : IService
  {
    Task<GameObject> CreatePlayer(Vector3 at);
    Task<GameObject> CreateHud();
    GameObject PlayerGameObject { get; }
    MineralStates CreateMineral(MineralStates prefab, Transform parent);
  }
}