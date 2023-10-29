using UnityEngine;

namespace CodeBase.CameraLogic
{
  public class CameraFollow : MonoBehaviour
  {
    [SerializeField] private Vector3 _offsetPosition;
    [SerializeField] private bool _lookAt;

    private Transform _playerTransform;
    private Transform _mainCamera;

    private void Start()
    {
      _mainCamera = Camera.main.transform;
    }

    public void Follow(GameObject following) => _playerTransform = following.transform;

    private void LateUpdate()
    {
      UpdateCamera();
    }

    private void UpdateCamera()
    {
      transform.position = _playerTransform.position + -(_playerTransform.forward * _offsetPosition.z) +
                           (_playerTransform.up * _offsetPosition.y);

      if (_lookAt) _mainCamera.LookAt(_playerTransform, _playerTransform.up);
      else _mainCamera.LookAt(_playerTransform);
    }
  }
}