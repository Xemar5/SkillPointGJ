using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PathNode : MonoBehaviour
{
    [SerializeField]
    private Collider2D contactCollider;
    private int index;

    public event Action<int, PathFollower> OnPointReached;



    public void Initialize(int index)
    {
        this.index = index;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PathFollower pathFollower = collision.GetComponent<PathFollower>();

        if (pathFollower != null && index != -1 && index == pathFollower.CurrentFollowedNode)
        {
            OnPointReached?.Invoke(index, pathFollower);
        }
    }
}