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
    [SerializeField]
    private KnightFeet feet;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private float attackSpeed = 0.5f;

    private float xVel = 0;
    private Vector2 velocity;
    private Vector2 normal;

    private bool jumped = false;
    private bool grab = false;
    private NodeCollider.NodeHit grabbedNode = null;
    private float grabOffset;
    private Quaternion grabRotation;


    private void Update()
    {
        if (jumped == true && feet.OnTheGround)
        {
            jumped = false;
        }

        xVel = 0;
        grab = Input.GetKey(KeyCode.Space);
        if (Input.GetKey(KeyCode.LeftArrow)) xVel -= 1;
        if (Input.GetKey(KeyCode.RightArrow)) xVel += 1;
        if (grabbedNode == null && Input.GetKeyDown(KeyCode.UpArrow) && jumped == false)
        {
            Jump(normal);
        }

        UpdateAnimator();
    }

    private void UpdateAnimator()
    {
        animator.SetBool("walking", xVel != 0);
        animator.SetInteger("falling", rigidbody2D.velocity.y > 0.1f ? 1 : rigidbody2D.velocity.y < -0.1 ? -1 : 0);
        Vector3 scale = animator.transform.localScale;
        scale.x = xVel < -0.1f ? -1 : xVel > 0.1f ? 1 : scale.x;
        animator.transform.localScale = scale;
    }

    private void FixedUpdate()
    {
        InitializeGrab();

        if (grabbedNode != null)
        {
            HandleGrab();
        }
        else
        {
            HandleMovement();
        }
    }
    private void InitializeGrab()
    {
        if (grab == false && grabbedNode != null)
        {
            grabbedNode = null;
            rigidbody2D.isKinematic = false;
            SwordEffect.Stop();
        }
        if (grab == true && grabbedNode == null && nodeCollider.hits.Count > 0)
        {
            grabbedNode = nodeCollider.hits.First().Value;
            Vector2 grabTranslation = (Vector2)transform.position - grabbedNode.node.position;
            grabOffset = grabTranslation.magnitude;
            grabRotation = Quaternion.FromToRotation(grabbedNode.node.velocity, grabTranslation);
            rigidbody2D.isKinematic = true;

            SwordEffect.Play(grabbedNode, attackSpeed);
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

        this.normal = normal;

        velocity += Vector2.right * xVel * speed * Time.fixedDeltaTime;

        velocity += 2 * gravity * Time.fixedDeltaTime;
        velocity *= breaking;

        rigidbody2D.velocity = velocity + external / Time.fixedDeltaTime;
    }

    private void Jump(Vector2 normal)
    {
        jumped = true;
        normal.x *= 0.5f; // Make jumping sideways easier
        normal.y = Mathf.Lerp(normal.y, 1, 0.7f);
        velocity = jumpForce * normal;
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