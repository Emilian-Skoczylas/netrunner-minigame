using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class NodeHandler : MonoBehaviour
{
    [Inject] private NodesController nodesController;
    [Inject] private ISecurityService securityService;

    [SerializeField] private NodeType Type;
    [SerializeField] private NodeState State;
    [SerializeField] private SecurityLevel SecurityLevel;

    [SerializeField] private Transform lineParent;

    [SerializeField] private List<NodeHandler> NextNeighbors = new List<NodeHandler>();
    [SerializeField] private List<NodeHandler> PreviousNeighbors = new List<NodeHandler>();

    public Dictionary<NodeHandler, List<UILineConnector>> Connections = new Dictionary<NodeHandler, List<UILineConnector>>();

    private RectTransform rectTransform;
    private Image image;
    private Button button;
    [SerializeField] private bool isInteractable;

    [Header("Capture UI")]
    [SerializeField] private TMP_Text captureValueText;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        button = GetComponent<Button>();
    }
    private void Start()
    {
        if (button != null)
            button.onClick.AddListener(OnNodeClicked);

        foreach (var neighbor in NextNeighbors)
        {
            neighbor.PreviousNeighbors.Add(this);
            RectTransform neighborRect = neighbor.GetComponent<RectTransform>();

            Vector3 fromPos = this.rectTransform.position;
            Vector3 toPos = neighborRect.position;
            Vector3 dir = (toPos - fromPos).normalized;
            Vector2 normal = new Vector2(-dir.y, dir.x);
            float spacing = 20f;

            List<UILineConnector> tempLineList = new List<UILineConnector>();

            //  OUTGOING LINE
            {
                GameObject lineGO = new GameObject($"Line_OUT_{name}_to_{neighbor.name}", typeof(RectTransform));
                lineGO.transform.SetParent(lineParent, false);

                var line = lineGO.AddComponent<UILineConnector>();
                line.SetTargets(this.rectTransform, neighborRect, true);
                line.SetProgress(0f);
                line.SetOffset(normal * (spacing * 0.5f));

                tempLineList.Add(line);

                if (Type == NodeType.IOPort)
                {
                    neighbor.SetupInteractable(true);
                }
            }

            // INCOMING LINE
            {
                GameObject lineGO = new GameObject($"Line_IN_{name}_to_{neighbor.name}", typeof(RectTransform));
                lineGO.transform.SetParent(lineParent, false);

                var line = lineGO.AddComponent<UILineConnector>();
                line.SetTargets(this.rectTransform, neighborRect, false);
                line.SetProgress(0f);
                line.SetOffset(normal * (-spacing * 0.5f));

                tempLineList.Add(line);
            }

            Connections.Add(neighbor, tempLineList);
            neighbor.Connections.Add(this, tempLineList);
        }
    }

    private void OnNodeClicked()
    {
        if (isInteractable && State == NodeState.Uncaptured)
        {
            nodesController.TryCapture(PreviousNeighbors[0], this);
            captureValueText.transform.parent.gameObject.SetActive(true);
        }
    }

    public void UpdateUI(NodeHandler nodeUI, float progress)
    {
        if (Connections.ContainsKey(nodeUI))
        {
            Connections[nodeUI][0].SetProgress(progress);

            int progressInt = (int)(progress * 100);
            captureValueText.text = $"{progressInt}%";
        } 
    }

    public void OnCaptureCompleted()
    {
        captureValueText.transform.parent.gameObject.SetActive(false);

        bool detected = securityService.IsDetected(SecurityLevel);
        State = !detected ? NodeState.Capturing : NodeState.Alerted;
        if (detected)
            Debug.Log("DETECTED!");

        for (int i = 0; i < NextNeighbors.Count; i++)
        {
            var neighbor = NextNeighbors[i];

            neighbor.SetupInteractable(true);
        }
    } 

    private void SetupInteractable(bool interactable)
    {
        if (Type == NodeType.IOPort)
            return;

        isInteractable = interactable;

        button.interactable = interactable;
        image.color = isInteractable ? new Color(1f, 1f, 1f, 1f) : new Color(0.3f, 0.3f, 0.3f, 1f);
    }
}
