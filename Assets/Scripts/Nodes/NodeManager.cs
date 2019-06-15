using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    private AnimationCurve widthCurve;
    private float widthMultiplier;

    private int nodeCount = 0;
    public Node[] nodesLeft;
    public Node[] nodesRight;
    public Node[] nodesMiddle;

    public bool IsGenerated => nodeCount > 0;
    public int NodeCount => nodeCount;


    public void Generate(int nodeCount)
    {

        if (IsGenerated == true)
        {
            return;
        }
        this.nodeCount = nodeCount;

        nodesMiddle = new Node[nodeCount];
        nodesLeft = new Node[nodeCount];
        nodesRight = new Node[nodeCount];
    }

    public void InitializeNodes(Vector2 headPosition, Vector2 tailDirection, float speed, float widthMultiplier, AnimationCurve widthCurve)
    {
        this.widthMultiplier = widthMultiplier;
        this.widthCurve = widthCurve;
        tailDirection = tailDirection.normalized;

        Vector2 leftDirection = new Vector2(-tailDirection.y, tailDirection.x).normalized;
        Vector2 rightDirection = new Vector2(tailDirection.y, -tailDirection.x).normalized;
        Vector2 velocity = -tailDirection * speed;

        for (int i = 0; i < nodeCount; i++)
        {
            int index = nodeCount - i - 1;
            nodesMiddle[index].position = headPosition + tailDirection * speed * (float)i;
            nodesMiddle[index].velocity = velocity;
            nodesMiddle[index].side = NodeSide.Middle;
            nodesMiddle[index].index = index;

            float ratio = (float)i / (float)(nodeCount - 1);
            float distance = widthCurve.Evaluate(ratio);
            nodesLeft[index].position = nodesMiddle[index].position + leftDirection * widthMultiplier * distance;
            nodesLeft[index].velocity = velocity;
            nodesLeft[index].side = NodeSide.Left;
            nodesLeft[index].index = index;

            nodesRight[index].position = nodesMiddle[index].position + rightDirection * widthMultiplier * distance;
            nodesRight[index].velocity = velocity;
            nodesRight[index].side = NodeSide.Right;
            nodesRight[index].index = index;
        }
    }


    public void UpdateMiddleTail()
    {
        for (int i = 0; i < nodeCount - 1; i++)
        {
            nodesMiddle[i].position = nodesMiddle[i + 1].position;
            nodesMiddle[i].velocity = nodesMiddle[i + 1].velocity;
        }

    }

    public Node CalculateVelocity(Vector2 targetPosition, float maxTurnAngle, float speed, Node lastNode)
    {
        Vector2 newVelocity = targetPosition - lastNode.position;
        float currentAngle = -Vector2.SignedAngle(lastNode.velocity, Vector2.right);
        float newAngle = -Vector2.SignedAngle(newVelocity, Vector2.right);
        float angle = Mathf.MoveTowardsAngle(currentAngle, newAngle, maxTurnAngle);

        float x = Mathf.Cos(angle * Mathf.Deg2Rad);
        float y = Mathf.Sin(angle * Mathf.Deg2Rad);
        Vector2 velocity = new Vector2(x, y) * speed;

        lastNode.velocity = velocity;
        lastNode.position += lastNode.velocity;

        return lastNode;
    }

    public void UpdateSides()
    {
        for (int i = 0; i < nodeCount; i++)
        {
            Node middle = nodesMiddle[i];
            float ratio = (float)i / (float)(nodeCount - 1);
            float width = widthCurve.Evaluate(ratio) * widthMultiplier;

            Vector2 leftOffset = new Vector2(middle.velocity.y, -middle.velocity.x).normalized * width;
            Vector2 rightOffset = new Vector2(-middle.velocity.y, middle.velocity.x).normalized * width;

            Node left = nodesLeft[i];
            Node right = nodesRight[i];

            Vector2 lastLeft = left.position;
            Vector2 lastRight = right.position;

            left.position = middle.position + leftOffset;
            left.velocity = left.position - lastLeft;
            right.position = middle.position + rightOffset;
            right.velocity = right.position - lastRight;

            nodesLeft[i] = left;
            nodesRight[i] = right;
        }
    }

    public void InitializeCollider(PolygonCollider2D collider)
    {
        collider.points = new Vector2[nodeCount * 2];
    }
    public void UpdateCollider(PolygonCollider2D collider)
    {
        Vector2[] points = collider.points;
        for (int i = 0; i < nodeCount; i++)
        {
            int oppositeIndex = 2 * nodeCount - 1 - i;
            points[i] = nodesLeft[i].position;
            points[oppositeIndex] = nodesRight[i].position;
        }
        collider.points = points;
    }


    public Node GetClosestNode(Vector2 contactPoint)
    {
        float closestDistance = float.PositiveInfinity;
        Node closestNode = new Node();
        for (int i = 0; i < nodeCount; i++)
        {
            float distance = Vector2.Distance(nodesLeft[i].position, contactPoint);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNode = nodesLeft[i];
            }
            distance = Vector2.Distance(nodesRight[i].position, contactPoint);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNode = nodesRight[i];
            }
        }
        return closestNode;
    }
}
