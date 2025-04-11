using UnityEngine;

public class DataLoader : MonoBehaviour {
    public TextAsset jsonFile;  // Assign data.json in Inspector
    public GameObject nodePrefab; // Assign a Cube/Capsule in Inspector

    void Start() {
        if (jsonFile == null) Debug.LogError("Assign JSON file!");
        else LoadData();
    }

    void LoadData() {
        SystemData data = JsonUtility.FromJson<SystemData>(jsonFile.text);
        foreach (SystemNode node in data.nodes) {
            Create3DNode(node);
        }
    }

    void Create3DNode(SystemNode node) {
        GameObject newNode = Instantiate(nodePrefab, Vector3.zero, Quaternion.identity);
        newNode.name = node.name;
        // Customize based on node data (e.g., color by status)
        Renderer rend = newNode.GetComponent<Renderer>();
        rend.material.color = node.status == "online" ? Color.green : Color.red;
    }
} 