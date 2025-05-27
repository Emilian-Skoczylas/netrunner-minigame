using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class NodeHandler : MonoBehaviour
{
    [Inject] private NodesController _nodesController;
    [Inject] private ISecurityService _securityService;
    [Inject] private NodeInteractionHandler _nodeInteractionHandler;

    [SerializeField] private NodeType _type;
    [SerializeField] private NodeState _state;
    [SerializeField] private SecurityLevel _securityLevel;
    public SecurityLevel SecurityLevel => _securityLevel;

    [SerializeField] private Transform _lineParent;

    [SerializeField] private List<NodeHandler> _nextNeighbors = new List<NodeHandler>();
    public List<NodeHandler> PreviousNeighbors = new List<NodeHandler>();

    private Dictionary<NodeHandler, List<UILineConnector>> _connections = new Dictionary<NodeHandler, List<UILineConnector>>();

    private RectTransform _rectTransform;
    private Image _image;
    private Button _button;
    [SerializeField] private bool _isInteractable;

    [Header("Node UI")]
    [SerializeField] private TMP_Text _captureValueText;
    [SerializeField] private TMP_Text _securityLevelText;
    [SerializeField] private Image _fortifyProgressImage;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        _button = GetComponent<Button>();
    }
    private void Start()
    {
        SetupSecurityLevelUI();

        if (_type == NodeType.Diagnosis)
            _nodesController.InitDiagnosisNode(this);
        else if (_type == NodeType.IOPort)
            SetupInteractable(true);

        if (_button != null)
            _button.onClick.AddListener(OnNodeClicked);

        foreach (var neighbor in _nextNeighbors)
        {
            neighbor.PreviousNeighbors.Add(this);

            Vector3 dir = (neighbor.transform.position - transform.position).normalized;
            Vector2 normal = new Vector2(-dir.y, dir.x);
            float spacing = 20f;

            CreateLine(neighbor, normal, spacing);

            if (_type == NodeType.IOPort)
                neighbor.SetupInteractable(true);

            if (!neighbor._connections.ContainsKey(this))
                neighbor._connections[this] = _connections[neighbor]; // współdzielenie
        }
    }

    private void CreateLine(NodeHandler neighbor, Vector2 normal, float spacing)
    {
        CreateLineInstance($"Line_OUT_{name}_to_{neighbor.name}", true, normal * (spacing * 0.5f), neighbor);
        CreateLineInstance($"Line_IN_{name}_to_{neighbor.name}", false, normal * (-spacing * 0.5f), neighbor);
    }

    private void CreateLineInstance(string name, bool outgoing, Vector2 offset, NodeHandler neighbor)
    {
        GameObject lineGO = new GameObject(name, typeof(RectTransform));
        lineGO.transform.SetParent(_lineParent, false);

        var line = lineGO.AddComponent<UILineConnector>();
        line.SetTargets(_rectTransform, neighbor.GetComponent<RectTransform>(), outgoing);
        line.SetProgress(0f);
        line.SetOffset(offset);

        if (!outgoing)
            line.SetReverseProgress(true);

        if (!_connections.ContainsKey(neighbor)) _connections[neighbor] = new List<UILineConnector>();
            _connections[neighbor].Add(line);
    }

    private void SetupSecurityLevelUI()
    {
        int securityInt = (int)_securityLevel;
        _securityLevelText.text = securityInt.ToString();
    }

    private void OnNodeClicked()
    {
        if (_isInteractable)
        {
            _nodeInteractionHandler.ShowUI(_type, _state, this.transform.position, _securityLevel, (result) =>
            {
                if (result == NodeInteractionType.Capture)
                {
                    _nodesController.TryCapture(PreviousNeighbors[0], this);
                    _captureValueText.transform.parent.gameObject.SetActive(true);
                }
                else
                {
                    _nodesController.TryFortify(this);
                    _fortifyProgressImage.transform.parent.gameObject.SetActive(true);
                }
            });
        }
    }

    public void UpdateUI(NodeHandler nodeUI, float progress, bool enemyCapture)
    {
        if (_connections.ContainsKey(nodeUI))
        {
            if (!enemyCapture)
                _connections[nodeUI][0].SetProgress(progress);
            else
                _connections[nodeUI][1].SetProgress(progress);

            int progressInt = (int)(progress * 100);
            _captureValueText.text = $"{progressInt}%";
        } 
    }
    public void UpdateFortifyProgress(float progress)
    {
        if (_fortifyProgressImage != null)
        {
            _fortifyProgressImage.fillAmount = progress / 1f;
        }
    }

    public void OnCaptureCompleted()
    {
        _captureValueText.transform.parent.gameObject.SetActive(false);

        bool winCondition = _type == NodeType.Registry;

        if (winCondition)
        {
            Debug.Log("WIN");
            return;
        }

        bool detected = _securityService.IsDetected(_securityLevel);
        _state = !detected ? NodeState.Captured : NodeState.Alerted;

        for (int i = 0; i < _nextNeighbors.Count; i++)
        {
            var neighbor = _nextNeighbors[i];

            neighbor.SetupInteractable(true);
        }

        if (detected)
        {
            Debug.Log("DETECTED!");
            _nodesController.Detected();
        }
    }
    public void OnEnemyCaptureCompleted()
    {
        for (int i = 0;i < PreviousNeighbors.Count; i++)
        {
            var node = PreviousNeighbors[i];

            _nodesController.TryCapture(this, node, true);
        }
        if (_type == NodeType.IOPort)
        {
            Debug.Log("DEFEAT!");
        }
    }

    public void OnFortifyCompleted()
    {
        _fortifyProgressImage.transform.parent.gameObject.SetActive(false);
        _securityLevel++;
        SetupSecurityLevelUI();

        bool detected = _securityService.IsDetected(_securityLevel);

        if (detected)
        {
            _state = NodeState.Captured;
            Debug.Log("DETECTED!");
            _nodesController.Detected();
        }
    }

    private void SetupInteractable(bool interactable)
    {
        _isInteractable = interactable;
        _button.interactable = interactable;

        if (_type == NodeType.Regular)
            _image.color = _isInteractable ? new Color(1f, 1f, 1f, 1f) : new Color(0.3f, 0.3f, 0.3f, 1f);
    }
}
