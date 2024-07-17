using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCNavTest : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public Transform target;

    private void Update()
    {
        bool moved = false;
        if (Input.GetKeyDown(KeyCode.W))
        {
            target.position += Vector3.forward;
            moved = true;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            target.position -= Vector3.right;
            moved = true;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            target.position -= Vector3.forward;
            moved = true;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            target.position += Vector3.right;
            moved = true;
        }
        if (moved)
        {
            navMeshAgent.SetDestination(target.position);
        }
    }
}
