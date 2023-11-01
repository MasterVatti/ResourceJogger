using System;

namespace CodeBase.Services.UIService
{
  public interface IUIService
  {
    event Action<string> MessageInputted;
    void ShowMessage(string message);
  }
}