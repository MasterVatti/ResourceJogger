using System;
using CodeBase.Services.Input;
using UnityEngine;

namespace CodeBase
{
  public class Player : MonoBehaviour
  {
    private IInputService _inputService;

    public void Constructor(IInputService inputService)
    {
      _inputService = inputService;
    }

    private void FixedUpdate()
    {
      Debug.Log($"{_inputService.Axis}Axis");
    }
  }
}