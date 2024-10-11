using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Cable : MonoBehaviour
{
    public List<Transform> nodes;

    private LineRenderer lineRenderer;

    private void Start()
    {
        SetNodes(nodes);
    }

    public void SetNodes(List<Transform> newNodes)
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = newNodes.Count;
        nodes = newNodes;
        UpdateLinePositions();
    }

    private void Update()
    {
        UpdateLinePositions();
    }

    private void UpdateLinePositions()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            lineRenderer.SetPosition(i, nodes[i].position);
        }
    }
}
