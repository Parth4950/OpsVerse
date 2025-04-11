using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using System;
using System.Threading.Tasks;

public class NodeController : XRGrabInteractable {
    public SystemNode nodeData; // Data from DataLoader
    public GameObject infoPanel; // Assign a UI panel prefab in Inspector
    private TextMeshProUGUI infoText;
    private bool isSelected = false;
    private DataLoader dataLoader;
    private float lastMetricUpdateTime;
    private const float METRIC_UPDATE_INTERVAL = 1f;

    protected override void Awake() {
        base.Awake();
        
        // Create info panel if assigned
        if (infoPanel != null) {
            var panel = Instantiate(infoPanel, transform);
            panel.transform.localPosition = Vector3.up * 1.5f; // Position above node
            infoText = panel.GetComponentInChildren<TextMeshProUGUI>();
            panel.SetActive(false);
        }
        dataLoader = FindObjectOfType<DataLoader>();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args) {
        base.OnSelectEntered(args);
        isSelected = true;
        UpdateInfoPanel();
        if (infoPanel != null) infoPanel.SetActive(true);
    }

    protected override void OnSelectExited(SelectExitEventArgs args) {
        base.OnSelectExited(args);
        isSelected = false;
        if (infoPanel != null) infoPanel.SetActive(false);
    }

    private void Update() {
        if (isSelected) {
            UpdateInfoPanel();
            
            // Update metrics periodically
            if (Time.time - lastMetricUpdateTime >= METRIC_UPDATE_INTERVAL) {
                UpdateMetrics();
                lastMetricUpdateTime = Time.time;
            }
        }
    }

    private async void UpdateMetrics() {
        if (dataLoader != null && nodeData != null) {
            await dataLoader.UpdateNodeMetrics(nodeData);
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

    // Called when the node is moved in VR
    protected override void OnSelectExiting(SelectExitEventArgs args) {
        base.OnSelectExiting(args);
        
        // Update position in backend
        if (nodeData != null) {
            nodeData.positionX = transform.position.x;
            nodeData.positionY = transform.position.y;
            nodeData.positionZ = transform.position.z;
            UpdateMetrics();
        }
    }
} 