using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SystemNode {
    public string id;  // Maps to NodeId
    public string name;
    public string type;
    public string status;
    public float positionX;
    public float positionY;
    public float positionZ;
    public string createdAt;  // DateTime will be serialized as string
    public string updatedAt;  // DateTime will be serialized as string
    public Dictionary<string, object> metadata;  // For the jsonb Metadata field
    
    // Performance metrics (from Metadata)
    public float cpu_usage;
    public float memory_usage;

    public void UpdateFromMetadata() {
        if (metadata != null) {
            if (metadata.TryGetValue("cpu_usage", out object cpuObj) && cpuObj != null) {
                cpu_usage = Convert.ToSingle(cpuObj);
            }
            if (metadata.TryGetValue("memory_usage", out object memObj) && memObj != null) {
                memory_usage = Convert.ToSingle(memObj);
            }
        }
    }
}

[System.Serializable]
public class SystemData {
    public List<SystemNode> nodes;
} 