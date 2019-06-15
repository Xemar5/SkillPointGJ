using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SerpentController : MonoBehaviour
{
    [SerializeField]
    private NodeManager nodeManager;
    [SerializeField]
    private NodeModulation nodeNoise;

    [SerializeField]
    private PathFollower pathFollower;
    [SerializeField]
    private PathManager pathManager;

    [SerializeField]
    private PolygonCollider2D polygonCollider;


    [SerializeField]
    private int nodeCount = 30;
    [SerializeField]
    private Vector2 startingPosition;

    [SerializeField]
    private float speed = 10;
    [SerializeField]
    private float turning = 30f;
    [SerializeField]
    private float widthMultiplier = 0.05f;
    [SerializeField]
    private AnimationCurve widthCurve = new AnimationCurve();

    private Node lastNode;

    private void Awake()
    {
        nodeManager.Generate(nodeCount);
        nodeManager.InitializeNodes(startingPosition, Vector2.left, speed * Time.fixedDeltaTime, widthMultiplier * Time.fixedDeltaTime, widthCurve);
        nodeManager.InitializeCollider(polygonCollider);
        pathManager.InitializePathFollower(pathFollower);
        lastNode = nodeManager.nodesMiddle[nodeManager.NodeCount - 1];
    }

    private void OnDrawGizmos()
    {
        if (nodeManager == null || nodeManager.IsGenerated == false)
        {
            return;
        }
        for (int i = 1; i < nodeManager.NodeCount; i++)
        {
            Gizmos.DrawLine(nodeManager.nodesMiddle[i - 1].position, nodeManager.nodesMiddle[i].position);
            Gizmos.DrawLine(nodeManager.nodesLeft[i - 1].position, nodeManager.nodesLeft[i].position);
            Gizmos.DrawLine(nodeManager.nodesRight[i - 1].position, nodeManager.nodesRight[i].position);
        }
    }

    private void FixedUpdate()
    {

        lastNode = nodeManager.CalculateVelocity(pathFollower.NodePosition, turning, speed * Time.fixedDeltaTime, lastNode);
        Node modulatedNode = nodeNoise.ModulateNode(lastNode);
        nodeManager.nodesMiddle[nodeManager.NodeCount - 1] = modulatedNode;

        nodeManager.UpdateMiddleTail();
        nodeManager.UpdateSides();
        nodeManager.UpdateCollider(polygonCollider);
    }


}