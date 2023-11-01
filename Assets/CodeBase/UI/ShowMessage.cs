using System.Collections;
using CodeBase.Infrastructure;
using CodeBase.Services.UIService;
using TMPro;
using UnityEngine;
using Zenject;

namespace CodeBase.UI
{
  public class ShowMessage : MonoBehaviour
  {
    [SerializeField] private TextMeshProUGUI _text;

    private IUIService _uiService;
    private ICoroutineRunner _coroutineRunner;

    [Inject]
    public void Constructor(IUIService uiService, ICoroutineRunner coroutineRunner)
    {
      _uiService = uiService;
      _coroutineRunner = coroutineRunner;
    }

    private void Start()
    {
      _text.alpha = 0f;
      _uiService.MessageInputted += OnMessageInputted;
    }

    private void OnMessageInputted(string message)
    {
      _text.text = message;
      _text.alpha = 1f;
      _coroutineRunner.StartCoroutine(DisableText());
    }

    private IEnumerator DisableText()
    {
      yield return new WaitForSeconds(1f);
      while (_text.alpha > 0)
      {
        _text.alpha -= 0.03f;
        yield return new WaitForSeconds(0.03f);
      }
    }
  }
}