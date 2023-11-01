using CodeBase.Minerals;
using UnityEngine;

namespace CodeBase.Infrastructure.AssetManagement
{
  public interface IAssetProvider
  {
    GameObject Instantiate(string path, Vector3 at);
    GameObject Instantiate(string path);
    MineralStates Instantiate(MineralStates prefab, Vector3 position, Quaternion rotation);
  }
}