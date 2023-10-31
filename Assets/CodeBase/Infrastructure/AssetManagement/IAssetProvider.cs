using CodeBase.Minerals;
using CodeBase.Services;
using UnityEngine;

namespace CodeBase.Infrastructure.AssetManagement
{
  public interface IAssetProvider : IService
  {
    GameObject Instantiate(string path, Vector3 at);
    GameObject Instantiate(string path);
    MineralStates Instantiate(MineralStates prefab, Transform parent);
  }
}