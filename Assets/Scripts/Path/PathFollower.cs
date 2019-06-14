using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PathFollower : MonoBehaviour
{

    [SerializeField]
    private int currentFollowedNode;
    [SerializeField]
    private bool increasingOrder = true;

    public int CurrentFollowedNode => currentFollowedNode;
    public bool IncreasingOrder => increasingOrder;
    public Vector2 NodePosition { get; private set; }


    public void SetCurrentPathNode(int nodeIndex, Vector2 nodePosition)
    {
        currentFollowedNode = nodeIndex;
        NodePosition = nodePosition;
    }

}