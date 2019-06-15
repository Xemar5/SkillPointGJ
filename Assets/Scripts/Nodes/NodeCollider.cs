using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NodeCollider : MonoBehaviour
{
    public class NodeHit
    {
        public Vector2 normal;
        public Node node;
        public NodeManager nodeManager;
    }

    [SerializeField]
    private new Rigidbody2D rigidbody2D;
    [SerializeField]
    private float hitFade = 0.95f;
    [SerializeField]
    private float hitFadeThreshold = 0.01f;

    public Dictionary<NodeManager, NodeHit> hits = new Dictionary<NodeManager, NodeHit>();
    public Dictionary<NodeManager, NodeHit> fadingHits = new Dictionary<NodeManager, NodeHit>();
    public event Action<NodeManager, NodeHit> OnHit;

    public void AddCollision(NodeCollisionManager manager, NodeManager nodeManager)
    {
        NodeHit hit = CalculateHit(nodeManager);
        if (fadingHits.ContainsKey(nodeManager) == true)
        {
            fadingHits.Remove(nodeManager);
        }
        hits.Add(nodeManager, hit);
        OnHit?.Invoke(nodeManager, hit);
    }

    public void RemoveCollision(NodeCollisionManager manager, NodeManager nodeManager)
    {
        fadingHits.Add(nodeManager, hits[nodeManager]);
        hits.Remove(nodeManager);
    }

    private void FixedUpdate()
    {
        List<NodeManager> keys = hits.Keys.ToList();
        foreach (NodeManager manager in keys)
        {
            NodeHit hit = CalculateHit(manager);
            hits[manager] = hit;
        }
        List<NodeManager> fadingKeys = fadingHits.Keys.ToList();
        foreach (NodeManager manager in fadingKeys)
        {
            NodeHit hit = fadingHits[manager];
            hit.node.velocity *= hitFade;
            hit.normal *= hitFade;
            if (hit.node.velocity.magnitude <= hitFadeThreshold && hit.normal.magnitude <= hitFadeThreshold)
            {
                fadingHits.Remove(manager);
            }
        }

    }

    private NodeHit CalculateHit(NodeManager nodeManager)
    {
        Vector2 point = transform.position;
        NodeHit hit = new NodeHit();
        hit.nodeManager = nodeManager;
        hit.node = nodeManager.GetClosestNode(point);
        hit.normal = hit.node.side == NodeSide.Left ?
            new Vector2(hit.node.velocity.y, -hit.node.velocity.x).normalized :
            new Vector2(-hit.node.velocity.y, hit.node.velocity.x).normalized;
        return hit;
    }



}






//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;

//public class NodeCollider : MonoBehaviour
//{
//    public class NodeHit
//    {
//        public Vector2 normal;
//        public Node node;
//        public NodeManager nodeManager;
//    }

//    [SerializeField]
//    private new Rigidbody2D rigidbody2D;
//    [SerializeField]
//    private float hitFade = 0.95f;
//    [SerializeField]
//    private float hitFadeThreshold = 0.01f;

//    public Dictionary<NodeManager, NodeHit> hits = new Dictionary<NodeManager, NodeHit>();
//    public Dictionary<Node, NodeHit> fadingHits = new Dictionary<Node, NodeHit>();
//    public event Action<NodeManager, NodeHit> OnHit;

//    public void AddCollision(NodeCollisionManager manager, NodeManager nodeManager)
//    {
//        NodeHit hit = CalculateHit(nodeManager);
//        if (fadingHits.ContainsKey(hit.node) == true)
//        {
//            fadingHits.Remove(hit.node);
//        }
//        hits.Add(nodeManager, hit);
//        OnHit?.Invoke(nodeManager, hit);
//    }

//    public void RemoveCollision(NodeCollisionManager manager, NodeManager nodeManager)
//    {
//        NodeHit hit = hits[nodeManager];
//        fadingHits.Add(hit.node, hit);
//        hits.Remove(nodeManager);
//    }

//    private void FixedUpdate()
//    {
//        List<NodeManager> keys = hits.Keys.ToList();
//        for (int i = 0; i < keys.Count; i++)
//        {
//            NodeManager nodeManager = keys[i];
//            NodeHit hit = CalculateHit(nodeManager);
//            NodeHit previousHit = hits[nodeManager];
//            if (previousHit.node != hit.node)
//            {
//                fadingHits[previousHit.node] = previousHit;
//            }
//            hits[nodeManager] = hit;
//        }
//        List<NodeHit> fadingValues = fadingHits.Values.ToList();
//        for (int i = 0; i < fadingValues.Count; i++)
//        {
//            NodeHit hit = fadingValues[i];
//            hit.node.velocity *= hitFade;
//            hit.normal *= hitFade;
//            if (hit.node.velocity.magnitude <= hitFadeThreshold && hit.normal.magnitude < hitFadeThreshold)
//            {
//                fadingHits.Remove(hit.node);
//            }
//        }

//    }

//    private NodeHit CalculateHit(NodeManager nodeManager)
//    {
//        Vector2 point = transform.position;
//        NodeHit hit = new NodeHit();
//        hit.nodeManager = nodeManager;
//        hit.node = nodeManager.GetClosestNode(point);
//        hit.normal = hit.node.side == NodeSide.Left ?
//            new Vector2(hit.node.velocity.y, -hit.node.velocity.x).normalized :
//            new Vector2(-hit.node.velocity.y, hit.node.velocity.x).normalized;
//        return hit;
//    }



//}