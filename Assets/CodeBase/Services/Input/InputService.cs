using UnityEngine;

namespace CodeBase.Services.Input
{
  public class InputService : IInputService
  {
    private const string Horizontal = "Horizontal";
    private const string Vertical = "Vertical";

#if UNITY_IOS || UNITY_ANDROID
    public Vector3 Axis => SimpleInputAxis();
#else
    public InputService()
    {
      Debug.Log("InputService");
    }
    public Vector3 Axis
    {
      get
      {
        Vector3 axis = SimpleInputAxis();

        if (axis == Vector3.zero)
        {
          axis = UnityAxis();
        }

        return axis;
      }
    }

    private static Vector3 UnityAxis()
    {
      return new Vector3(UnityEngine.Input.GetAxis(Horizontal), 0, UnityEngine.Input.GetAxis(Vertical)).normalized;
    }
#endif

    private static Vector3 SimpleInputAxis() => new(SimpleInput.GetAxis(Horizontal), 0, SimpleInput.GetAxis(Vertical));
  }
}