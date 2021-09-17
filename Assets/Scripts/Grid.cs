using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Grid : MonoBehaviour
{
    private Mesh mesh;

    public int xSize, ySize;
    private int mXSize, mYSize;

    private void Update()
    {
        if(mXSize != xSize || mYSize != ySize)
        {
            mXSize = xSize;
            mYSize = ySize;
            Generate();
        }
    }

    private Vector3[] vertices;
    private void Generate()
    {
        WaitForSeconds wait = new WaitForSeconds(0.05f);

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.name = "Procedural Grid";
        vertices = new Vector3[(xSize + 1) * (ySize + 1)];

        for (int i = 0, y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                vertices[i] = new Vector3(x, y);
            }
        }

        mesh.vertices = vertices;

        //每一个点都需要uv，tangents 所以是 <=

        Vector2[] uv = new Vector2[vertices.Length];
        Vector4[] tangents = new Vector4[vertices.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
        for (int i = 0, y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
                tangents[i] = tangent;
            }
        }
        mesh.uv = uv;
        mesh.tangents = tangents;

        //need 6 vertices for each grid
        //int[] triangles = new int[6];
        //triangles[0] = 0;
        //triangles[3] = triangles[2] = 1;
        //triangles[4] = triangles[1] = xSize + 1;
        //triangles[5] = xSize + 2;

        int[] triangles = new int[xSize * ySize * 6];
        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                int count = y * xSize + x;
                int svi = count + y;
          
                int triangleStartIndex = count * 6;
                triangles[triangleStartIndex] = svi;
                triangles[triangleStartIndex + 2] = triangles[triangleStartIndex + 3] = svi + 1;
                triangles[triangleStartIndex + 1] = triangles[triangleStartIndex + 4] = svi + xSize + 1;
                triangles[triangleStartIndex + 5] = svi + xSize + 2;

            }
        }


        mesh.triangles = triangles;
        mesh.RecalculateNormals();
     
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (vertices == null)
        {
            return;
        }

        Gizmos.color = Color.black;
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }
#endif
}
