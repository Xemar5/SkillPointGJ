using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    [SerializeField]
    private List<PathNode> pathNodes;


    private void Awake()
    {
        for (int i = 0; i < pathNodes.Count; i++)
        {
            pathNodes[i].Initialize(i);
            pathNodes[i].OnPointReached += PathManager_OnPointReached;
        }
    }

    public void InitializePathFollower(PathFollower pathFollower)
    {
        PathManager_OnPointReached(pathFollower.CurrentFollowedNode - 1, pathFollower);
    }

    private void PathManager_OnPointReached(int index, PathFollower pathFollower)
    {
        int direction = pathFollower.IncreasingOrder == true ? 1 : -1;
        int nodeIndex = (index + direction) % pathNodes.Count;
        pathFollower.SetCurrentPathNode(nodeIndex, pathNodes[nodeIndex].transform.position);
    }
}
