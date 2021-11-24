using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class QuaternionTest : MonoBehaviour
{
    enum Rotation
    {
        Axis,
        Matrix
    }

    // Start is called before the first frame update
    public Transform center;
    public Vector3 u = Vector3.zero;
    public Vector3 v = Vector3.zero;
    public float degree = 0;
    [SerializeField]
    Rotation mode = default;
    void Start()
    {
        
    }
    [ExecuteInEditMode]
    // Update is called once per frame
    void Update()
    {
        degree += 0.5f;
        degree %= 360;

        if (mode == Rotation.Axis)
            this.transform.position = center.position + Quaternion_Rotation3D(u, v);
        else
            this.transform.position = Quaternion_Matrix3D(u, v);
 
    }

    public Vector3 Quaternion_Rotation3D(Vector3 u,Vector3 v)
    {
        Vector3 u_n = u.normalized;
        Vector3 v_n = v;

        float radian = Mathf.Deg2Rad * degree;


        float cosO = Mathf.Cos(radian);
        float sinO = Mathf.Sin(radian);
        Vector3 pos = cosO * v_n + (1 - cosO) * (Vector3.Dot(u_n, v_n)) * u_n + sinO * (Vector3.Cross(u_n, v_n));

        return pos;
    }

    public Vector3 Quaternion_Matrix3D(Vector3 u, Vector3 v)
    {
        Vector3 u_n = u.normalized;
        Vector3 v_n = v;

        float radian = Mathf.Deg2Rad * degree * 0.5f;


        float a = Mathf.Cos(radian);
        float b = Mathf.Sin(radian) * u_n.x;
        float b2 = Mathf.Pow(b, 2);
        float c = Mathf.Sin(radian) * u_n.y;
        float c2 = Mathf.Pow(c, 2);
        float d = Mathf.Sin(radian) * u_n.z;
        float d2 = Mathf.Pow(d, 2);

        
        Vector4 col_0 = new Vector4(1 - 2 * c2 - 2 * d2, 2 * b * c + 2 * a * d, 2 * b * d - 2 * a * c, 0);
        Vector4 col_1 = new Vector4(2 * b * c - 2 * a * d, 1 - 2 * b2 - 2 * d2, 2 * a * b + 2 * c * d,0);
        Vector4 col_2 = new Vector4(2 * a * c + 2 * b * d, 2 * c * d - 2 * a * b, 1 - 2 * b2 - 2 * c2,0);

        Vector3 pos = center.position;
        Matrix4x4 matrix4x4 = new Matrix4x4(col_0, col_1, col_2, new Vector4(pos.x, pos.y, pos.z, 1));

        return matrix4x4.MultiplyPoint(v_n);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        
        Gizmos.DrawLine(this.transform.position, center.position);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(Vector3.zero, new Vector3(100, 0, 0));
        Gizmos.DrawLine(Vector3.zero, new Vector3(0,100, 0));
        Gizmos.DrawLine(Vector3.zero, new Vector3(0, 0,100));

        Gizmos.color = Color.green;
        Gizmos.DrawLine(center.position, center.position+u);

        //Gizmos.color = Color.yellow;
        //Gizmos.DrawLine(Vector3.zero, v);

    }
}
