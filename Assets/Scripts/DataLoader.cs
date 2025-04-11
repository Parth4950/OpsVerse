using UnityEngine;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Text;

public class DataLoader : MonoBehaviour {
    public TextAsset jsonFile;  // Assign data.json in Inspector
    public GameObject nodePrefab; // Assign a Cube/Capsule in Inspector
    public float updateInterval = 5f; // Update interval in seconds
    public string apiUrl = "http://localhost:5000/api"; // Backend API URL
    
    private HttpClient _httpClient;
    private Dictionary<string, GameObject> _nodeObjects = new Dictionary<string, GameObject>();
    
    private void Start() {
        _httpClient = new HttpClient();
        
        if (jsonFile == null) {
            Debug.LogError("Assign JSON file!");
            return;
        }
        
        LoadData();
        StartWebSocketConnection();
        
        // Start periodic updates
        InvokeRepeating(nameof(LoadData), updateInterval, updateInterval);
    }
    
    private async void LoadData() {
        try {
            // Fetch data from API instead of local file
            var response = await _httpClient.GetAsync($"{apiUrl}/nodes");
            if (!response.IsSuccessStatusCode) {
                Debug.LogError($"Failed to fetch nodes: {response.StatusCode}");
                return;
            }
            
            var json = await response.Content.ReadAsStringAsync();
            SystemData data = JsonUtility.FromJson<SystemData>(json);
            
            if (data == null || data.nodes == null) {
                Debug.LogError("Failed to parse JSON data or no nodes found!");
                return;
            }

            foreach (SystemNode node in data.nodes) {
                try {
                    node.UpdateFromMetadata();
                    CreateOrUpdate3DNode(node);
                }
                catch (Exception e) {
                    Debug.LogError($"Error processing node {node.name}: {e.Message}");
                }
            }
        }
        catch (Exception e) {
            Debug.LogError($"Error loading data: {e.Message}");
        }
    }
    
    private async void StartWebSocketConnection() {
        try {
            // Implement WebSocket connection here
            await Task.Delay(1); // Placeholder for actual WebSocket implementation
            Debug.Log("WebSocket connection started");
        }
        catch (Exception e) {
            Debug.LogError($"WebSocket error: {e.Message}");
        }
    }
    
    public async Task UpdateNodeMetricsAsync(SystemNode node) {
        if (node == null) throw new ArgumentNullException(nameof(node));
        
        try {
            var metrics = new {
                CpuUsage = node.cpu_usage,
                MemoryUsage = node.memory_usage
            };
            
            var content = new StringContent(
                JsonUtility.ToJson(metrics),
                Encoding.UTF8,
                "application/json"
            );
            
            var response = await _httpClient.PostAsync(
                $"{apiUrl}/metrics/nodes/{node.id}",
                content
            );
            
            if (!response.IsSuccessStatusCode) {
                throw new Exception($"Failed to update metrics: {response.StatusCode}");
            }
        }
        catch (Exception e) {
            Debug.LogError($"Error updating metrics for node {node.name}: {e.Message}");
            throw; // Rethrow to let caller handle the error
        }
    }
    
    private void CreateOrUpdate3DNode(SystemNode node) {
        if (nodePrefab == null) {
            Debug.LogError("Node prefab not assigned!");
            return;
        }

        Vector3 position = new Vector3(node.positionX, node.positionY, node.positionZ);
        GameObject nodeObj;
        
        if (_nodeObjects.TryGetValue(node.id, out nodeObj)) {
            // Update existing node
            nodeObj.transform.position = position;
        }
        else {
            // Create new node
            nodeObj = Instantiate(nodePrefab, position, Quaternion.identity);
            nodeObj.name = node.id;
            _nodeObjects[node.id] = nodeObj;
        }

        // Get or add NodeController
        NodeController controller = nodeObj.GetComponent<NodeController>();
        if (controller == null) {
            controller = nodeObj.AddComponent<NodeController>();
        }
        
        // Update controller data
        controller.nodeData = node;
        
        // Update visualization
        Renderer rend = nodeObj.GetComponent<Renderer>();
        if (rend != null) {
            // Set color based on status
            rend.material.color = node.status.ToLower() == "online" ? Color.green : Color.red;
            
            // Scale based on resource usage
            float scale = 1f + (node.cpu_usage / 100f);
            nodeObj.transform.localScale = Vector3.one * scale;
        }
    }
    
    private void OnDestroy() {
        _httpClient?.Dispose();
    }
} 