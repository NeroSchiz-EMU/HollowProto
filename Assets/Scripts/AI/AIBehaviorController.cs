using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]

public class AIBehaviorController : MonoBehaviour {
    [SerializeReference] AIBehavior mainBehavior;

    private void Start() {
        InitializeTree();
        mainBehavior.Activate();
    }

    private void InitializeTree()
    {
        foreach (AITreeNode node in mainBehavior.Subtree())
        {
            node.SetController(this);
        }
    }

    GameObject target = null;
    public GameObject Target()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player");
        }
        return target;
    }

    public void Update()
    {
        mainBehavior.Update();
    }
    
}
