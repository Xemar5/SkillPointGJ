using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordEffect : MonoBehaviour
{
    private static SwordEffect instance;

    [SerializeField]
    private SpriteRenderer swordSprite;

    private void Awake()
    {
        instance = this;
        instance.swordSprite.enabled = false;
    }


    public static void Play(NodeCollider.NodeHit grabbedNode, float attackSpeed)
    {
        instance.swordSprite.enabled = true;
        instance.StartCoroutine(instance.PlaySword(grabbedNode, attackSpeed));
    }
    public static void Stop()
    {
        instance.StopAllCoroutines();
        instance.swordSprite.enabled = false;
    }

    private IEnumerator PlaySword(NodeCollider.NodeHit grabbedNode, float attackSpeed)
    {
        float slashTime = attackSpeed / 3f;
        while(true)
        {
            slashTime += Time.deltaTime;
            Node updatedNode = UpdateNode(grabbedNode);
            Vector2 direction = GetNodeDirection(updatedNode);

            transform.position = updatedNode.position;
            transform.rotation = Quaternion.FromToRotation(Vector3.right, direction);

            if (slashTime >= attackSpeed)
            {
                slashTime -= attackSpeed;

                BloodParticlesManager.Play(updatedNode.position + Vector2.right / 10, direction);
                ScreenShake.Play();
            }

            if (slashTime > attackSpeed / 3f)
            {
                swordSprite.transform.localPosition = Vector3.right / 10 + Vector3.down / 10;
            }
            else
            {
                swordSprite.transform.localPosition = Vector3.left / 20 + Vector3.down / 10;
            }


            yield return null;
        }
    }

    private Vector2 GetNodeDirection(Node node)
    {
        Vector2 direction;
        if (node.side == NodeSide.Left)
        {
            direction = new Vector2(node.velocity.y, -node.velocity.x);
        }
        else
        {
            direction = new Vector2(-node.velocity.y, node.velocity.x);
        }
        return direction;
    }
    private Node UpdateNode(NodeCollider.NodeHit grabbedNode)
    {
        Node updatedNode;
        if (grabbedNode.node.side == NodeSide.Left)
        {
            updatedNode = grabbedNode.nodeManager.nodesLeft[grabbedNode.node.index];
        }
        else
        {
            updatedNode = grabbedNode.nodeManager.nodesRight[grabbedNode.node.index];
        }
        return updatedNode;
    }

}
