using UnityEngine;
using TMPro;
using System;
using System.Threading.Tasks;

#if ENABLE_XR
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
#endif

public class NodeController : MonoBehaviour 
{
    public SystemNode nodeData; // Data from DataLoader
    public GameObject infoPanel; // Assign a UI panel prefab in Inspector

    private GameObject spawnedInfoPanel;
    private TextMeshProUGUI infoText;
    private bool isSelected = false;
    private DataLoader dataLoader;
    private float lastMetricUpdateTime;
    private const float METRIC_UPDATE_INTERVAL = 1f;

    private void Awake() 
    {
        // Find DataLoader in the scene using the new method
        dataLoader = FindFirstObjectByType<DataLoader>();
        if (dataLoader == null)
        {
            Debug.LogError("DataLoader not found in the scene!");
        }

        if (infoPanel != null) 
        {
            spawnedInfoPanel = Instantiate(infoPanel, transform);
            if (spawnedInfoPanel != null)
            {
                spawnedInfoPanel.transform.localPosition = Vector3.up * 1.5f; // Position above node
                infoText = spawnedInfoPanel.GetComponentInChildren<TextMeshProUGUI>();
                if (infoText == null)
                {
                    Debug.LogError("TextMeshProUGUI component not found in info panel!");
                }
                spawnedInfoPanel.SetActive(false);
            }
            else
            {
                Debug.LogError("Failed to instantiate info panel!");
            }
        }
        else
        {
            Debug.LogWarning("Info panel prefab not assigned!");
        }
    }

    private void Update() 
    {
        // Handle mouse input for non-VR interaction
        if (Input.GetMouseButtonDown(0)) 
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit)) 
            {
                if (hit.collider.gameObject == gameObject) 
                {
                    Debug.Log("Tapped on: " + hit.collider.name);
                    // Toggle selection state
                    isSelected = !isSelected;
                    if (isSelected)
                    {
                        OnNodeSelected();
                    }
                    else
                    {
                        OnNodeDeselected();
                    }
                }
            }
        }

        if (isSelected && Time.time - lastMetricUpdateTime >= METRIC_UPDATE_INTERVAL) 
        {
            UpdateMetricsAsync();
            lastMetricUpdateTime = Time.time;
        }
    }

    private void OnNodeSelected()
    {
        if (spawnedInfoPanel != null)
        {
            spawnedInfoPanel.SetActive(true);
            UpdateInfoPanel();
        }
    }

    private void OnNodeDeselected()
    {
        if (spawnedInfoPanel != null)
        {
            spawnedInfoPanel.SetActive(false);
        }
    }

    private async void UpdateMetricsAsync() 
    {
        if (dataLoader != null && nodeData != null) 
        {
            try 
            {
                await dataLoader.UpdateNodeMetricsAsync(nodeData);
                UpdateInfoPanel();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error updating metrics: {e.Message}");
            }
        }
    }

    private void UpdateInfoPanel() 
    {
        if (infoText != null && nodeData != null) 
        {
            infoText.text = $"Name: {nodeData.name}\n" +
                           $"Type: {nodeData.type}\n" +
                           $"Status: {nodeData.status}\n" +
                           $"CPU: {nodeData.cpu_usage:F1}%\n" +
                           $"Memory: {nodeData.memory_usage:F1}%\n" +
                           $"Position: ({nodeData.positionX:F1}, {nodeData.positionY:F1}, {nodeData.positionZ:F1})\n" +
                           $"Updated: {nodeData.updatedAt}";
        }
    }

    private void OnDisable()
    {
        if (nodeData != null) 
        {
            nodeData.positionX = transform.position.x;
            nodeData.positionY = transform.position.y;
            nodeData.positionZ = transform.position.z;
            UpdateMetricsAsync();
        }
    }

#if ENABLE_XR
    // XR-specific functionality will be added here when XR package is properly installed
    public void OnXRGrab(SelectEnterEventArgs args)
    {
        isSelected = true;
        OnNodeSelected();
    }

    public void OnXRRelease(SelectExitEventArgs args)
    {
        isSelected = false;
        OnNodeDeselected();
    }
#endif
} 