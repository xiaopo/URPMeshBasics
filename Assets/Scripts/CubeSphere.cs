using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CubeSphere : MonoBehaviour
{
    public int xSize, ySize, zSize;
    public int gridSize;
    public float radius = 1;

    private int mXSize, mYSize,mZSize,mgridSize;
    private float mRadius;
    private Mesh mesh;
    private Vector3[] vertices;
    private Vector3[] normals;
    private Color32[] cubeUV;

    private void Update()
    {
        if (xSize < 2) xSize = 2;
        if (ySize < 2) ySize = 2;
        if (zSize < 2) zSize = 2;

        if (mXSize != xSize || mYSize != ySize || mZSize != zSize || mgridSize != gridSize || mRadius != radius)
        {
            mXSize = xSize;
            mYSize = ySize;
            mZSize = zSize;
            mgridSize = gridSize;
            mRadius = radius;

            Generate();
        }
    }
    private void Generate()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Sphere";

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
        normals = new Vector3[vertices.Length];
        cubeUV = new Color32[vertices.Length];
        //顶点必须得有序不然在指定三角形的时候会出乱

        int v = 0;
        for (int y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++)
                SetVertex(v++, x, y, 0);

            for (int z = 1; z <= zSize; z++)
                SetVertex(v++, xSize, y, z);

            for (int x = xSize - 1; x >= 0; x--)
                SetVertex(v++, x, y, zSize);

            for (int z = zSize - 1; z > 0; z--)
                SetVertex(v++, 0, y, z);

        }

        //top 
        for (int z = 1; z < zSize; z++)
        {
            for (int x = 1; x < xSize; x++)
                SetVertex(v++, x, ySize, z);
        }

        //bottom
        for (int z = 1; z < zSize; z++)
        {
            for (int x = 1; x < xSize; x++)
                SetVertex(v++, x, 0, z);

        }

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.colors32 = cubeUV;
    }

    private void SetVertex(int i, int x, int y, int z)
    {
        //先获得在圆心的方向，然后在乘以半径
        Vector3 v = new Vector3(x, y, z) * 2f / gridSize - Vector3.one;
        float x2 = v.x * v.x;
        float y2 = v.y * v.y;
        float z2 = v.z * v.z;
        Vector3 s;
        s.x = v.x * Mathf.Sqrt(1f - y2 / 2f - z2 / 2f + y2 * z2 / 3f);
        s.y = v.y * Mathf.Sqrt(1f - x2 / 2f - z2 / 2f + x2 * z2 / 3f);
        s.z = v.z * Mathf.Sqrt(1f - x2 / 2f - y2 / 2f + x2 * y2 / 3f);
       // normals[i] = s;
        normals[i] = v.normalized;
        vertices[i] = normals[i] * radius;

        cubeUV[i] = new Color32((byte)x, (byte)y, (byte)z, 0);
    }
    private void CreateTriangles()
    {
        //6个大面
        //int quads = (xSize * ySize + xSize * zSize + ySize * zSize) * 2;
        //每个四边形面有6个index
        //int[] triangles = new int[quads * 6];
        int[] trianglesZ = new int[(xSize * ySize) * 2 * 6];
        int[] trianglesX = new int[(ySize * zSize) * 2 * 6];
        int[] trianglesY = new int[(xSize * zSize) * 2 * 6];
        int ring = (xSize + zSize) * 2;
        int tZ = 0, tX = 0, tY = 0, v = 0;
        for (int y = 0; y < ySize; y++, v++)
        {
            for (int q = 0; q < xSize; q++, v++)
                tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);

            for (int q = 0; q < zSize; q++, v++)
                tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);

            for (int q = 0; q < xSize; q++, v++)
                tZ = SetQuad(trianglesZ, tZ, v, v + 1, v + ring, v + ring + 1);

            for (int q = 0; q < zSize - 1; q++, v++)
                tX = SetQuad(trianglesX, tX, v, v + 1, v + ring, v + ring + 1);

            tX = SetQuad(trianglesX, tX, v, v - ring + 1, v + ring, v + 1);
        }

        tY = CreateTopFace(trianglesY, tY, ring);
        tY = CreateBottomFace(trianglesY, tY, ring);

        mesh.subMeshCount = 3;
        mesh.SetTriangles(trianglesZ, 0);
        mesh.SetTriangles(trianglesX, 1);
        mesh.SetTriangles(trianglesY, 2);
    }
    //private void CreateTriangles()
    //{
    //    //6个大面
    //    int quads = (xSize * ySize + xSize * zSize + ySize * zSize) * 2;
    //    //每个四边形面有6个index
    //    int[] triangles = new int[quads * 6];

    //    //画三角形时顶上那个index
    //    //二维的grid中该index和 i 差距一个xSize
    //    //三维中根据顶点的排序我们需要加上一圈的差距
    //    int ring = (xSize + zSize) * 2;
    //    int v = 0;
    //    for(int y = 0;y<ySize;y++)
    //    {
    //        for (int i = 0; i < ring - 1; i++)
    //        {
    //            int t = y * ring + i;
    //            v = SetQuad(triangles, v, t, t + 1, t + ring, t + ring + 1);
    //        }

    //        //一圈中最后一个四边形得特殊处理
    //        int endRing = (y + 1) * ring;
    //        int lastleftBottom = endRing - 1;
    //        v = SetQuad(triangles, v, lastleftBottom, ring*y, lastleftBottom + ring, endRing);
    //    }

    //    //顶面
    //    v = CreateTopFace(triangles,v,ring);

    //    //底面
    //    v = CreateBottomFace(triangles, v, ring);

    //    mesh.triangles = triangles;
    //}
    private int CreateTopFace(int[] triangles,int v,int ring)
    {
        //第一排
        int sIndex = ring * ySize;
        int lxs = xSize - 1;
        for(int i = 0;i<lxs;i++)
        {
            v = SetQuad(triangles, v, sIndex, sIndex + 1, sIndex + ring-1, sIndex + ring);
            sIndex++;
        }
        v = SetQuad(triangles, v, sIndex, sIndex + 1, sIndex + ring - 1, sIndex + 2);


        int vMin = ring * (ySize + 1) - 1;
        int vMid = vMin + 1;
        int vMax = sIndex + 2;
        //其他排中间
        for (int z = 1; z < zSize - 1; z++)
        {
            //left
            v = SetQuad(triangles, v, vMin, vMid, vMin - 1, vMid + xSize - 1);

            //middle
            for (int x = 1; x < xSize-1; x++)
            {
                v = SetQuad(triangles, v, vMid, vMid + 1, vMid + xSize - 1, vMid + xSize);
                vMid++;
            }

            //right
            v = SetQuad(triangles, v, vMid, vMax, vMid + xSize - 1, vMax + 1);
            vMid++;//在最后一个四边形指定后把 vMid 在移动一个位置


            //vMin左边 vMax 右边
            // -- 和 ++ 表示把他们向 Z 方向移动
            vMin--; vMax++;
        }

        if (zSize <= 1) return v;//不存在最后一排
        //最后一排
        //left
        int vTop = vMin - 2;
        v = SetQuad(triangles, v, vMin, vMid, vMin - 1, vTop);
        //middle
        for (int x = 1; x < xSize - 1; x++)
        {
            v = SetQuad(triangles, v, vMid, vMid + 1, vTop, vTop - 1);
            vMid++;
            vTop--;
        }
        //right
        v = SetQuad(triangles, v, vMid, vTop - 2, vTop, vTop - 1);

        return v;
    }

    private int CreateBottomFace(int[] triangles, int t, int ring)
    {
        int v = 1;
        int vMid = vertices.Length - (xSize - 1) * (zSize - 1);//起点

        t = SetQuad(triangles, t, ring - 1, vMid, 0, 1);
        for (int x = 1; x < xSize - 1; x++)
        {
            t = SetQuad(triangles, t, vMid, vMid + 1, v, v + 1);
            v++;
            vMid++;
        }

        t = SetQuad(triangles, t, vMid, v + 2, v, v + 1);
        v += 2;
        vMid++;

        for (int z = 2; z < zSize; z++)
        {
            t = SetQuad(triangles, t, ring - z, vMid, ring - z + 1, vMid - xSize + 1);

            for (int x = 1; x < xSize - 1; x++)
            {
                t = SetQuad(triangles, t, vMid, vMid + 1, vMid - xSize + 1, vMid - xSize + 2);
                vMid++;
            }

            t = SetQuad(triangles, t, vMid, v + 1, vMid - xSize + 1, v);
            v += 1;
            vMid++;
        }

        if (xSize <= 1) return t;
        //最后一行
        int zMid = ring - zSize;
        int vvMind = vMid - xSize + 1;
        t = SetQuad(triangles, t, zMid, zMid - 1, zMid + 1, vvMind);

        for (int x = 1; x < xSize - 1; x++)
        {
            zMid--;
            t = SetQuad(triangles, t, zMid, zMid - 1, vvMind, vvMind + 1);
            vvMind++;
        }

        zMid--;
        t = SetQuad(triangles, t, zMid, zMid - 1, vvMind, zMid - 2);

        return t;
    }

    private static int SetQuad(int[] triangles, int i, int v00, int v10, int v01, int v11)
    {
        triangles[i] = v00;
        triangles[i + 1] = triangles[i + 4] = v01;
        triangles[i + 2] = triangles[i + 3] = v10;
        triangles[i + 5] = v11;
        return i + 6;
    }

    private void OnDrawGizmos()
    {
        if (vertices == null)
        {
            return;
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(vertices[i], 0.1f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(vertices[i], normals[i]);
        }
    }
}
