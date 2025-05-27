using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class NodeInteractionHandler : MonoBehaviour
{
    [Inject] private ISecurityService _securityService;

    [SerializeField] private GameObject _panel;

    [Header("Fortify")]
    [SerializeField] private Button _fortifyButton;
    [SerializeField] private TMP_Text _fortifyDetectionChanceText;

    [Header("Capture")]
    [SerializeField] private Button _captureButton;
    [SerializeField] private TMP_Text _captureDetectionChangeText;

    private Action<NodeInteractionType> _onResultCallback;

    private bool _isOpen => _panel.activeSelf;

    private void Awake()
    {
        if (_fortifyButton != null)
        {
            _fortifyButton.onClick.AddListener(() => HandleResult(NodeInteractionType.Fortify));
        }
            
        if (_captureButton != null)
        {
            _captureButton.onClick.AddListener(() => HandleResult(NodeInteractionType.Capture));
        }
    }

    private void HandleResult(NodeInteractionType interactionType)
    {
        _onResultCallback?.Invoke(interactionType);
        HideUI();
    }

    public void ShowUI(NodeType type, NodeState state, Vector3 position, SecurityLevel securityLevel, Action<NodeInteractionType> onResult)
    {
        _onResultCallback = onResult;

        this.transform.position = position;

        UpdateButtonsInteractable(type, state);
        UpdateDetectionChances(securityLevel);       

        _panel.SetActive(true);
    }

    private void UpdateButtonsInteractable(NodeType type, NodeState state)
    {
        var (fortifyEnable, captureEnable) = type switch
        {
            NodeType.IOPort => (true, false),
            NodeType.Regular => (state == NodeState.Captured || state == NodeState.Alerted, true),
            NodeType.Registry => (false, true),
            _ => (false, false)
        };

        _fortifyButton.interactable = fortifyEnable;
        _captureButton.interactable = captureEnable;
    }
    private void UpdateDetectionChances(SecurityLevel securityLevel)
    {
        float captureDetectionChange = _securityService.GetCaptureDetectionChance(securityLevel) * 100f;
        float fortifyDetectionChange = _securityService.GetFortifyDetectionChance(securityLevel) * 100f;

        if (_captureDetectionChangeText != null)
        {
            _captureDetectionChangeText.text = $"{captureDetectionChange}%";
        }
        if (_fortifyDetectionChanceText != null)
        {
            _fortifyDetectionChanceText.text = $"{fortifyDetectionChange}%";
        }
    }

    public void HideUI()
    {
        if (_isOpen)
        {
            _panel.SetActive(false);
        }
    }
}
