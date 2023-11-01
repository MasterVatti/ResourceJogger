using System;

namespace CodeBase.Services.UIService
{
  public class UIService : IUIService
  {
    public event Action<string> MessageInputted;

    public void ShowMessage(string message)
    {
      MessageInputted?.Invoke(message);
    }
  }
}