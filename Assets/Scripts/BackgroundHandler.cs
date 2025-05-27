using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class BackgroundHandler : MonoBehaviour
{
    [Inject] private NodeInteractionHandler _nodeInteractionHandler;
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();

        if (_button != null)
        {
            _button.onClick.AddListener(() => OnBackgroundClicked());
        }
    }

    private void OnBackgroundClicked()
    {
        if (_nodeInteractionHandler != null)
        {
            _nodeInteractionHandler.HideUI();
        }
    }
}
