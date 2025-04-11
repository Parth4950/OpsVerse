using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SystemNode {
    public string id, name, type, status;
    public float cpu_usage, memory_usage;
    // Add ALL other fields from your JSON here
}

[System.Serializable]
public class SystemData {
    public List<SystemNode> nodes;
} 