using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NodeModulation : MonoBehaviour
{
    [SerializeField]
    private float strength = 1f / 25f;
    [SerializeField]
    private float duration = 1f;
    [SerializeField]
    private AnimationCurve curve;

    private float value;
    private float time;

    private void Update()
    {
        if (duration == 0)
        {
            Debug.LogWarning("Modulation duration is set to 0");
            return;
        }

        time += Time.deltaTime;
        while (time > duration)
        {
            time -= duration;
        }
        value = curve.Evaluate(time / duration) * strength;
    }

    public Node ModulateNode(Node node)
    {
        Vector2 left = new Vector2(node.velocity.y, -node.velocity.x).normalized;
        node.position += left * value;
        return node;
    }

}