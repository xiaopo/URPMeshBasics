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
        //�˸���
        int cornerVertices = 8;
        //12����Ե���ڵĵ�
        int edgeVertices = (xSize + ySize + zSize - 3) * 4;
        //6�����ڵĵ� (xy+xz+yz)*2
        int faceVertices = ((xSize - 1) * (ySize - 1) + (xSize - 1) * (zSize - 1) + (ySize - 1) * (zSize - 1)) * 2;
        vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];

        //������������Ȼ��ָ�������ε�ʱ������

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
        //6���ı�����
        int quads = (xSize * ySize + xSize * zSize + ySize * zSize) * 2;
        //ÿ���ı�������6��index
        int[] triangles = new int[quads * 6];

        //��������ʱ�����Ǹ�index
        //��ά��grid�и�index�� i ���һ��xSize
        //��ά�и��ݶ��������������Ҫ����һȦ�Ĳ��
        int ring = (xSize + zSize) * 2;
        int v = 0;
        for(int y = 0;y<ySize;y++)
        {
            for (int i = 0; i < ring - 1; i++)
            {
                int t = y * ring + i;
                v = SetQuad(triangles, v, t, t + 1, t + ring, t + ring + 1);
            }

            //һȦ�����һ���ı��ε����⴦��
            int endRing = (y + 1) * ring;
            int lastleftBottom = endRing - 1;
            v = SetQuad(triangles, v, lastleftBottom, 0, lastleftBottom + ring, endRing);
        }

        //����
        v = CreateTopFace(triangles,v,ring);

        //����

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
        triangles[i] = v00;//���½�
        triangles[i + 1] = triangles[i + 4] = v01;//���Ͻ�
        triangles[i + 2] = triangles[i + 3] = v10;//���½�
        triangles[i + 5] = v11;//���Ͻ�
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
