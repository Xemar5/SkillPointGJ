using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NodeMeshRenderer : MonoBehaviour
{
    [SerializeField]
    private MeshFilter meshFilter;
    [SerializeField]
    private NodeManager nodeManager;

    private Mesh mesh;

    private void Awake()
    {
        mesh = new Mesh();
        meshFilter.sharedMesh = mesh;
    }

    private void Start()
    {
        mesh.vertices = UpdateVertices(nodeManager.NodeCount);
        mesh.triangles = InitializeTriangles(nodeManager.NodeCount);
        meshFilter.sharedMesh = mesh;
    }

    private int[] InitializeTriangles(int nodeCount)
    {
        int[] triangles = new int[(nodeCount - 1) * 6];
        for (int i = 0; i < nodeCount - 1; i++)
        {
            int triangle = i * 6;
            int vertex = i * 2;
            // 2 * i + 0 == right
            // 2 * i + 1 == left
            triangles[triangle + 0] = vertex;       // back right
            triangles[triangle + 1] = vertex + 1;   // back left
            triangles[triangle + 2] = vertex + 3;   // front left

            triangles[triangle + 3] = vertex;       // back right
            triangles[triangle + 4] = vertex + 3;   // front left
            triangles[triangle + 5] = vertex + 2;   // front right
        }
        return triangles;
    }

    private Vector3[] UpdateVertices(int nodeCount)
    {
        Vector3[] vertices = new Vector3[nodeCount * 2];
        for (int i = 0; i < nodeCount; i++)
        {
            int index = i * 2;
            vertices[index + 0] = nodeManager.nodesRight[i].position;
            vertices[index + 1] = nodeManager.nodesLeft[i].position;
        }
        return vertices;
    }

    private void Update()
    {
        mesh.vertices = UpdateVertices(nodeManager.NodeCount);
    }

}