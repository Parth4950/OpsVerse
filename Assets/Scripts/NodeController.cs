using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using TMPro;
using System;
using System.Threading.Tasks;

public class NodeController : XRGrabInteractable {
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

        // Find DataLoader in the scene
        dataLoader = FindObjectOfType<DataLoader>();
        if (dataLoader == null) {
            Debug.LogError("DataLoader not found in the scene!");
        }

        if (infoPanel != null) {
            spawnedInfoPanel = Instantiate(infoPanel, transform);
            if (spawnedInfoPanel != null) {
                spawnedInfoPanel.transform.localPosition = Vector3.up * 1.5f; // Position above node
                infoText = spawnedInfoPanel.GetComponentInChildren<TextMeshProUGUI>();
                if (infoText == null) {
                    Debug.LogError("TextMeshProUGUI component not found in info panel!");
                }
                spawnedInfoPanel.SetActive(false);
            } else {
                Debug.LogError("Failed to instantiate info panel!");
            }
        } else {
            Debug.LogWarning("Info panel prefab not assigned!");
        }
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
                        OnSelectEntered(new SelectEnterEventArgs {
                            interactorObject = null,
                            interactableObject = this
                        });
                    } else {
                        OnSelectExited(new SelectExitEventArgs {
                            interactorObject = null,
                            interactableObject = this
                        });
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
        if (args == null) return;
        
        base.OnSelectEntered(args);
        isSelected = true;

        if (spawnedInfoPanel != null) {
            spawnedInfoPanel.SetActive(true);
            UpdateInfoPanel();
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args) {
        if (args == null) return;
        
        base.OnSelectExited(args);
        isSelected = false;

        if (spawnedInfoPanel != null) {
            spawnedInfoPanel.SetActive(false);
        }
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
        if (args == null) return;
        
        base.OnSelectExiting(args);

        if (nodeData != null) {
            nodeData.positionX = transform.position.x;
            nodeData.positionY = transform.position.y;
            nodeData.positionZ = transform.position.z;
            UpdateMetrics();
        }
    }
} 