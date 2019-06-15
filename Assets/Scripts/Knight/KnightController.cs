using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class KnightController : MonoBehaviour
{
    [SerializeField]
    private new Rigidbody2D rigidbody2D;
    [SerializeField]
    private NodeCollider nodeCollider;
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float jumpForce = 5f;
    [SerializeField]
    private float breaking = 0.95f;

    private float xVel = 0;
    private Vector2 velocity;

    private bool jump = false;
    private bool grab = false;
    private NodeCollider.NodeHit grabbedNode = null;
    private float grabOffset;
    private Quaternion grabRotation;

    private void Update()
    {
        xVel = 0;
        jump = false;
        grab = Input.GetKey(KeyCode.Space);
        if (grabbedNode == null)
        {
            if (Input.GetKey(KeyCode.A)) xVel -= 1;
            if (Input.GetKey(KeyCode.D)) xVel += 1;
            jump = Input.GetKeyDown(KeyCode.W);
        }

        if (grab == false && grabbedNode != null)
        {
            grabbedNode = null;
            rigidbody2D.isKinematic = false;
        }
        if (grab == true && grabbedNode == null && nodeCollider.hits.Count > 0)
        {
            grabbedNode = nodeCollider.hits.First().Value;
            Vector2 grabTranslation = (Vector2)transform.position - grabbedNode.node.position;
            grabOffset = grabTranslation.magnitude;
            grabRotation = Quaternion.FromToRotation(grabbedNode.node.velocity, grabTranslation);
            rigidbody2D.isKinematic = true;
        }
    }


    private void FixedUpdate()
    {
        if (grabbedNode != null)
        {
            HandleGrab();
        }
        else
        {
            HandleMovement();
        }
    }

    private void HandleGrab()
    {
        Node updatedNode;
        int nodeIndex = grabbedNode.node.index;
        if (grabbedNode.node.side == NodeSide.Left)
        {
            updatedNode = grabbedNode.nodeManager.nodesLeft[nodeIndex];
        }
        else
        {
            updatedNode = grabbedNode.nodeManager.nodesRight[nodeIndex];
        }
        Vector2 nodePosition = updatedNode.position;
        Vector2 nodeVelocity = updatedNode.velocity;

        transform.position = nodePosition + (Vector2)(grabRotation * nodeVelocity.normalized * grabOffset);
    }

    private void HandleMovement()
    {
        Vector2 external;
        Vector2 normal;
        Vector2 gravity;
        CalculateExternalAndNormal(out external, out normal, out gravity);

        if (jump == true)
        {
            jump = false;
            Debug.Log(normal.x + " " + normal.y);
            velocity = jumpForce * normal;
        }

        velocity += Vector2.right * xVel * speed * Time.fixedDeltaTime;

        velocity += 2 * gravity * Time.fixedDeltaTime;
        velocity *= breaking;

        rigidbody2D.velocity = velocity + external / Time.fixedDeltaTime;
    }

    private void CalculateExternalAndNormal(out Vector2 external, out Vector2 normal, out Vector2 gravity)
    {
        Vector2 hitSum = Vector2.zero;
        Vector2 fadeSum = Vector2.zero;
        float normalWeight = 0;
        external = Vector2.zero;
        foreach (var hit in nodeCollider.hits)
        {
            hitSum += hit.Value.normal;
            normalWeight += hit.Value.normal.magnitude;
            external += hit.Value.node.velocity;
        }
        foreach (var hit in nodeCollider.fadingHits)
        {
            fadeSum += hit.Value.normal;
            normalWeight += hit.Value.normal.magnitude;
            external += hit.Value.node.velocity;
        }

        gravity = (hitSum + fadeSum + Vector2.down) / (nodeCollider.hits.Count + nodeCollider.fadingHits.Count + 1);
        gravity = -gravity.y * Physics2D.gravity;
        normal = (hitSum + Vector2.up) / (nodeCollider.hits.Count + 1);
    }
}