using CodeBase.Services.Input;
using UnityEngine;

namespace CodeBase.Player
{
  public class PlayerMovement : MonoBehaviour
  {
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private float _movementSpeed;

    private IInputService _inputService;

    public void Constructor(IInputService inputService)
    {
      _inputService = inputService;
    }

    private void FixedUpdate()
    {
      MovePlayer();
    }

    private void MovePlayer()
    {
      Vector3 movementVector = _inputService.Axis.normalized;
      movementVector += Physics.gravity;
      _characterController.Move(_movementSpeed * movementVector * Time.deltaTime);
    }
  }
}