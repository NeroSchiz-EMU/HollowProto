using System.Collections.Generic;
using TriInspector;
using UnityEngine;

public class WireNodePlacer : MonoBehaviour
{
    [SerializeField] private GameObject startPoint;
    [SerializeField] private GameObject endPoint;
    [SerializeField] private int connectingPoints = 10;
    [SerializeField] private GameObject nodePrefab;

    List<GameObject> nodes = new List<GameObject>();

    [SerializeField] float frequency = 10f;
    [SerializeField] float dampingFactor = 0.5f;
    [SerializeField] Cable cable = null;

    [Button(ButtonSizes.Large)]
    private void GenerateWireNodes()
    {
        ClearPreviousNodes();
        DrawNodesInLine();
        CreateSpringJoints();
        HookUpRenderer();
    }

    private void DrawNodesInLine()
    {
        // Calculate the step size
        Vector3 step = (endPoint.transform.position - startPoint.transform.position) / (connectingPoints + 1);

        for (int i = 1; i <= connectingPoints; i++)
        {
            // Calculate the position for each node
            Vector3 position = startPoint.transform.position + step * i;

            // Instantiate the node
            GameObject node = Instantiate(nodePrefab, position, Quaternion.identity);
            node.transform.parent = this.transform;

            // Add the node to the list
            nodes.Add(node);
        }
    }

    private void CreateSpringJoints()
    {
        List<SpringJoint2D> joints = new List<SpringJoint2D>();
        // Connect start point to the first node
        joints.Add(nodes[0].AddComponent<SpringJoint2D>());
        joints[joints.Count - 1].connectedBody = startPoint.GetComponent<Rigidbody2D>();

        // Connect each node to the previous one
        for (int i = 1; i < nodes.Count; i++)
        {
            joints.Add(nodes[i].AddComponent<SpringJoint2D>());
            joints[joints.Count - 1].connectedBody = nodes[i-1].GetComponent<Rigidbody2D>();
        }

        if (endPoint.TryGetComponent<SpringJoint2D>(out var endJoint))
        {
            joints.Add(endJoint);
        }
        else
        {
            joints.Add(endPoint.AddComponent<SpringJoint2D>());
        }
        joints[joints.Count - 1].connectedBody = nodes[nodes.Count - 1].GetComponent<Rigidbody2D>();

        for (int i = 0; i < joints.Count; i++)
        {
            joints[i].frequency = frequency;
            joints[i].dampingRatio = dampingFactor;
        }
    }

    public void HookUpRenderer()
    {
        if (cable == null) return;
        List<Transform> allNodes = new List<Transform>();
        allNodes.Add(startPoint.transform);
        for (int i = 0; i < nodes.Count; i++)
        {
            allNodes.Add(nodes[i].transform);
        }
        allNodes.Add(endPoint.transform);
        cable.SetNodes(allNodes);
    }

    private void ClearPreviousNodes()
    {
        for (var i = gameObject.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(gameObject.transform.GetChild(i).gameObject);
        }
        nodes.Clear();
    }
}