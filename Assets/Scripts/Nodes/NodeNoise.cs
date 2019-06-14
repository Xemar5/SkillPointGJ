using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NodeNoise : MonoBehaviour
{
    private float sin;

    private void Update()
    {
        sin = Mathf.Sin(Time.time * 5);
    }

    public Node ModulateNode(Node node)
    {
        Vector2 left = new Vector2(node.velocity.y, -node.velocity.x).normalized;
        node.position += left * sin / 25;
        return node;
    }

}