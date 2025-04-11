using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using System;
using System.Threading.Tasks;

public class NodeController : UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable {
    public SystemNode nodeData; // Data from DataLoader
    public GameObject infoPanel; // Assign a UI panel prefab in Inspector

    private GameObject spawnedInfoPanel;
    private TextMeshProUGUI infoText;
    private bool isSelected = false;
    private DataLoader dataLoader;
    private float lastMetricUpdateTime;
    private const float METRIC_UPDATE_INTERVAL = 1f;

    protected override void Awake() {
        base.Awake();

        if (infoPanel != null) {
            spawnedInfoPanel = Instantiate(infoPanel, transform);
            spawnedInfoPanel.transform.localPosition = Vector3.up * 1.5f; // Position above node
            infoText = spawnedInfoPanel.GetComponentInChildren<TextMeshProUGUI>();
            spawnedInfoPanel.SetActive(false);
        }

        dataLoader = FindObjectOfType<DataLoader>();
    }

    private void Update() {
        // Handle mouse input for non-VR interaction
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                if (hit.collider.gameObject == gameObject) {
                    Debug.Log("Tapped on: " + hit.collider.name);
                    // Simulate XR selection for mouse interaction
                    if (!isSelected) {
                        OnSelectEntered(new SelectEnterEventArgs());
                    } else {
                        OnSelectExited(new SelectExitEventArgs());
                    }
                }
            }
        }

        if (isSelected && Time.time - lastMetricUpdateTime >= METRIC_UPDATE_INTERVAL) {
            UpdateMetrics();
            lastMetricUpdateTime = Time.time;
        }
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args) {
        base.OnSelectEntered(args);
        isSelected = true;

        if (spawnedInfoPanel != null)
            spawnedInfoPanel.SetActive(true);

        UpdateInfoPanel();
    }

    protected override void OnSelectExited(SelectExitEventArgs args) {
        base.OnSelectExited(args);
        isSelected = false;

        if (spawnedInfoPanel != null)
            spawnedInfoPanel.SetActive(false);
    }

    private async void UpdateMetrics() {
        if (dataLoader != null && nodeData != null) {
            await dataLoader.UpdateNodeMetrics(nodeData);
            UpdateInfoPanel();
        }
    }

    private void UpdateInfoPanel() {
        if (infoText != null && nodeData != null) {
            infoText.text = $"Name: {nodeData.name}\n" +
                            $"Type: {nodeData.type}\n" +
                            $"Status: {nodeData.status}\n" +
                            $"CPU: {nodeData.cpu_usage:F1}%\n" +
                            $"Memory: {nodeData.memory_usage:F1}%\n" +
                            $"Position: ({nodeData.positionX:F1}, {nodeData.positionY:F1}, {nodeData.positionZ:F1})\n" +
                            $"Updated: {nodeData.updatedAt}";
        }
    }

    protected override void OnSelectExiting(SelectExitEventArgs args) {
        base.OnSelectExiting(args);

        if (nodeData != null) {
            nodeData.positionX = transform.position.x;
            nodeData.positionY = transform.position.y;
            nodeData.positionZ = transform.position.z;
            UpdateMetrics();
        }
    }
} 