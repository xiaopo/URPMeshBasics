using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Cube : MonoBehaviour
{
    public int xSize, ySize, zSize;
    private int mXSize, mYSize,mZSize;
    private Mesh mesh;
    private Vector3[] vertices;


    private void Update()
    {
        if (mXSize != xSize || mYSize != ySize || mZSize != zSize)
        {
            mXSize = xSize;
            mYSize = ySize;
            mZSize = zSize;

            Generate();
        }
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
        //v = CreateBottomFace(triangles, v, ring);

        mesh.triangles = triangles;
    }
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

        for (int z = 2;z< zSize;z++)
        {
            t = SetQuad(triangles, t, ring - z, vMid, ring - z+1, vMid - xSize +1);

            for (int x = 1; x < xSize - 1; x++)
            {
                t = SetQuad(triangles, t, vMid, vMid + 1, vMid - xSize + 1, vMid - xSize + 2);
                vMid++;
            }

            t = SetQuad(triangles, t, vMid, v + 1, vMid - xSize + 1, v);
            v += 1;
            vMid++;
        }

        int zMid = ring - zSize;
        int vvMind = vMid - xSize + 1;
        t = SetQuad(triangles, t, zMid, zMid-1, zMid+1, vvMind);

        for(int x = 1;x<xSize -1;x++)
        {
            zMid--;
            t = SetQuad(triangles, t, zMid, zMid - 1, vvMind, vvMind+1);
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
        Gizmos.color = Color.black;
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }
}
