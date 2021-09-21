using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Cube : MonoBehaviour
{
    public int xSize, ySize, zSize;

    private Mesh mesh;
    private Vector3[] vertices;

    private void Awake()
    {
        Generate();
    }

    private void Generate()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Cube";

        CreateVertices();

        CreateTriangles();

    }
    private void CreateVertices()
    {
        //八个角
        int cornerVertices = 8;
        //12条边缘线内的点
        int edgeVertices = (xSize + ySize + zSize - 3) * 4;
        //6个面内的点 (xy+xz+yz)*2
        int faceVertices = ((xSize - 1) * (ySize - 1) + (xSize - 1) * (zSize - 1) + (ySize - 1) * (zSize - 1)) * 2;
        vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];

        //顶点必须得有序不然在指定三角形的时候会出乱

        int v = 0;
        for (int y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++)
                vertices[v++] = new Vector3(x, y, 0);

            for (int z = 1; z <= zSize; z++)
                vertices[v++] = new Vector3(xSize, y, z);

            for (int x = xSize - 1; x >= 0; x--)
                vertices[v++] = new Vector3(x, y, zSize);

            for (int z = zSize - 1; z > 0; z--)
                vertices[v++] = new Vector3(0, y, z);

        }

        //top 
        for (int z = 1; z < zSize; z++)
        {
            for (int x = 1; x < xSize; x++)
                vertices[v++] = new Vector3(x, ySize, z);
        }

        //bottom
        for (int z = 1; z < zSize; z++)
        {
            for (int x = 1; x < xSize; x++)
                vertices[v++] = new Vector3(x, 0, z);
        }

        mesh.vertices = vertices;
    }

    private void CreateTriangles()
    {
        //6个四边形面
        int quads = (xSize * ySize + xSize * zSize + ySize * zSize) * 2;
        //每个四边形面有6个index
        int[] triangles = new int[quads * 6];

        //画三角形时顶上那个index
        //二维的grid中该index和 i 差距一个xSize
        //三维中根据顶点的排序我们需要加上一圈的差距
        int ring = (xSize + zSize) * 2;
        int v = 0;
        for(int y = 0;y<ySize;y++)
        {
            for (int i = 0; i < ring - 1; i++)
            {
                int t = y * ring + i;
                v = SetQuad(triangles, v, t, t + 1, t + ring, t + ring + 1);
            }

            //一圈中最后一个四边形得特殊处理
            int endRing = (y + 1) * ring;
            int lastleftBottom = endRing - 1;
            v = SetQuad(triangles, v, lastleftBottom, 0, lastleftBottom + ring, endRing);
        }

        //顶面
        v = CreateTopFace(triangles,v,ring);

        //底面

        mesh.triangles = triangles;
    }
    private int CreateTopFace(int[] triangles,int v,int ring)
    {


        return v;
    }

    private int CreateBottomFace(int[] triangles, int v, int ring)
    {

        return v;
    }

    private static int SetQuad(int[] triangles, int i, int v00, int v10, int v01, int v11)
    {
        triangles[i] = v00;//左下角
        triangles[i + 1] = triangles[i + 4] = v01;//左上角
        triangles[i + 2] = triangles[i + 3] = v10;//右下角
        triangles[i + 5] = v11;//右上角
        return i + 6;
    }
 
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
}
