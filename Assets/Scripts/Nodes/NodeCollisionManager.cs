using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NodeCollisionManager : MonoBehaviour
{
    [SerializeField]
    private NodeManager nodeManager;



    private void OnCollisionEnter2D(Collision2D collision)
    {
        NodeCollider nodeCollider = collision.gameObject.GetComponent<NodeCollider>();
        if (nodeCollider != null)
        {
            nodeCollider.AddCollision(this, nodeManager);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        NodeCollider nodeCollider = collision.gameObject.GetComponent<NodeCollider>();
        if (nodeCollider != null)
        {
            nodeCollider.RemoveCollision(this, nodeManager);
        }
    }

}