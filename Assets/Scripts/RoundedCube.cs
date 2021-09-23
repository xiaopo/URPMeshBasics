using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RoundedCube : MonoBehaviour
{
    public int xSize, ySize, zSize;
    public int roundness;

    private int mXSize, mYSize,mZSize;
    private Mesh mesh;
    private Vector3[] vertices;
    private Vector3[] normals;

    private void Update()
    {
        if (xSize < 2) xSize = 2;
        if (ySize < 2) ySize = 2;
        if (zSize < 2) zSize = 2;

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
        //�˸���
        int cornerVertices = 8;
        //12����Ե���ڵĵ�
        int edgeVertices = (xSize + ySize + zSize - 3) * 4;
        //6�����ڵĵ� (xy+xz+yz)*2
        int faceVertices = ((xSize - 1) * (ySize - 1) + (xSize - 1) * (zSize - 1) + (ySize - 1) * (zSize - 1)) * 2;
        vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];
        normals = new Vector3[vertices.Length];
        //������������Ȼ��ָ�������ε�ʱ������

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
    }
    private void SetVertex(int i, int x, int y, int z)
    {
        Vector3 inner = vertices[i] = new Vector3(x, y, z);

        //���� x,y,z �ڱ߽�ƫ����
        if (x < roundness) inner.x = roundness;
        else if (x > xSize - roundness)  inner.x = xSize - roundness;

        if (y < roundness)  inner.y = roundness;
        else if (y > ySize - roundness)  inner.y = ySize - roundness;

        if (z < roundness)  inner.z = roundness;
        else if (z > zSize - roundness)  inner.z = zSize - roundness;

        normals[i] = (vertices[i] - inner).normalized;

        //�ط��߷�������
        vertices[i] = inner + normals[i] * roundness;
    }
    private void CreateTriangles()
    {
        //6������
        //int quads = (xSize * ySize + xSize * zSize + ySize * zSize) * 2;
        //ÿ���ı�������6��index
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
    //    //6������
    //    int quads = (xSize * ySize + xSize * zSize + ySize * zSize) * 2;
    //    //ÿ���ı�������6��index
    //    int[] triangles = new int[quads * 6];

    //    //��������ʱ�����Ǹ�index
    //    //��ά��grid�и�index�� i ���һ��xSize
    //    //��ά�и��ݶ��������������Ҫ����һȦ�Ĳ��
    //    int ring = (xSize + zSize) * 2;
    //    int v = 0;
    //    for(int y = 0;y<ySize;y++)
    //    {
    //        for (int i = 0; i < ring - 1; i++)
    //        {
    //            int t = y * ring + i;
    //            v = SetQuad(triangles, v, t, t + 1, t + ring, t + ring + 1);
    //        }

    //        //һȦ�����һ���ı��ε����⴦��
    //        int endRing = (y + 1) * ring;
    //        int lastleftBottom = endRing - 1;
    //        v = SetQuad(triangles, v, lastleftBottom, ring*y, lastleftBottom + ring, endRing);
    //    }

    //    //����
    //    v = CreateTopFace(triangles,v,ring);

    //    //����
    //    v = CreateBottomFace(triangles, v, ring);

    //    mesh.triangles = triangles;
    //}
    private int CreateTopFace(int[] triangles,int v,int ring)
    {
        //��һ��
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
        //�������м�
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
            vMid++;//�����һ���ı���ָ����� vMid ���ƶ�һ��λ��


            //vMin��� vMax �ұ�
            // -- �� ++ ��ʾ�������� Z �����ƶ�
            vMin--; vMax++;
        }

        if (zSize <= 1) return v;//���������һ��
        //���һ��
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
        int vMid = vertices.Length - (xSize - 1) * (zSize - 1);//���

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
        //���һ��
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