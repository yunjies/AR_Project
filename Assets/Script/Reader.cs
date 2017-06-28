using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class Reader : MonoBehaviour {

    public List<Matrix4x4> matrixList = new List<Matrix4x4>();
    public List<KeyValuePair<int, List<KeyValuePair<float, float>>>> markers = new List<KeyValuePair<int, List<KeyValuePair<float, float>>>>();
    public Transform m_MarkerModelApple;
    bool m_bMarkerCreated = false;
    private List<Vector3> m_MarkerCentralPoints = new List<Vector3>();
    Vector3 _screenV;
    GameObject plane;
    private int imWidth = 640;
    private int imHeight = 480;

    Matrix4x4 invertYM = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1, -1, 1));
    Matrix4x4 invertZM = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1, 1, -1));
    //public Vector3 Angles = new Quaternion.EulerAngles(90, 0, 0);

    private List<Transform> m_MarkerObjectList = new List<Transform>();
    public Dictionary<int, Transform> m_MarkerModelDict = new Dictionary<int, Transform>();

    // Use this for initialization
    void Start()
    {
        m_MarkerModelDict.Add(213, m_MarkerModelApple);
        plane = GameObject.Find("Plane");
        
    }

    // Update is called once per frame
    void Update()
    {
        _screenV = GetComponent<Camera>().WorldToScreenPoint(plane.GetComponent<Transform>().position);

        if (m_bMarkerCreated)
        {
            DestroyMakerObject();
            m_MarkerObjectList.Clear();
            m_MarkerCentralPoints.Clear();
        }
        Read();
        for (int i = 0; i < matrixList.Count; i++)
        {
            Matrix4x4 matrix = matrixList[i];
            int ID = markers[i].Key;
            Vector3 centralPoint = m_MarkerCentralPoints[i];
            CreateMarkerObject(matrix, ID, centralPoint);
        }

    }

    public void Read()
    {
        matrixList.Clear();
        markers.Clear();
        StreamReader sr = new StreamReader("c:\\Output.txt");
        string line;
        while ((line = sr.ReadLine()) != null)
        {
            List<KeyValuePair<float, float>> points = new List<KeyValuePair<float, float>>();
            string[] elements = line.Split('\t');
            points.Add(new KeyValuePair<float, float>(float.Parse(elements[1]), float.Parse(elements[2])));
            points.Add(new KeyValuePair<float, float>(float.Parse(elements[3]), float.Parse(elements[4])));
            points.Add(new KeyValuePair<float, float>(float.Parse(elements[5]), float.Parse(elements[6])));
            points.Add(new KeyValuePair<float, float>(float.Parse(elements[7]), float.Parse(elements[8])));
            m_MarkerCentralPoints.Add(new Vector3((float.Parse(elements[1]) + float.Parse(elements[3]) + float.Parse(elements[5]) + float.Parse(elements[7])) / 4, imHeight - (float.Parse(elements[2]) + float.Parse(elements[4]) + float.Parse(elements[6]) + float.Parse(elements[8])) / 4, _screenV.z));
            //Debug.Log(m_MarkerCentralPoints[0]);
            markers.Add(new KeyValuePair<int, List<KeyValuePair<float, float>>>(System.Convert.ToInt32(elements[0]), points));

            if ((line = sr.ReadLine()) != null)
            {
                string[] num = line.Split('\t');
                Matrix4x4 matrix = new Matrix4x4();
                Vector4 row0 = new Vector4(float.Parse(num[0]), float.Parse(num[1]), float.Parse(num[2]), float.Parse(num[3]));
                Vector4 row1 = new Vector4(float.Parse(num[4]), float.Parse(num[5]), float.Parse(num[6]), float.Parse(num[7]));
                Vector4 row2 = new Vector4(float.Parse(num[8]), float.Parse(num[9]), float.Parse(num[10]), float.Parse(num[11]));
                Vector4 row3 = new Vector4(float.Parse(num[12]), float.Parse(num[13]), float.Parse(num[14]), float.Parse(num[15]));
                matrix.SetColumn(0, row0);
                matrix.SetColumn(1, row1);
                matrix.SetColumn(2, row2);
                matrix.SetColumn(3, row3);
                //Debug.Log(matrix.ToString());
                matrixList.Add(matrix);
            }
        }
    }

    //matrix4x4 to transform

    //public static Vector3 ExtractTranslationFromMatrix(ref Matrix4x4 matrix)
    //{
    //    Vector3 translate;
    //    translate.x = matrix.m03;
    //    translate.z = matrix.m13;
    //    translate.y = 0*matrix.m23;
    //    return translate;
    //}

    ///// <summary>
    ///// Extract rotation quaternion from transform matrix.
    ///// </summary>
    ///// <param name="matrix">Transform matrix. This parameter is passed by reference
    ///// to improve performance; no changes will be made to it.</param>
    ///// <returns>
    ///// Quaternion representation of rotation transform.
    ///// </returns>
    //public static Quaternion ExtractRotationFromMatrix(ref Matrix4x4 matrix)
    //{
    //    Vector3 forward;
    //    forward.x = matrix.m02;
    //    forward.z = matrix.m12;
    //    forward.y = matrix.m22;

    //    Vector3 upwards;
    //    upwards.x = matrix.m01;
    //    upwards.z = matrix.m11;
    //    upwards.y = matrix.m21;

    //    return Quaternion.LookRotation(forward, upwards);
    //}

    ///// <summary>
    ///// Extract scale from transform matrix.
    ///// </summary>
    ///// <param name="matrix">Transform matrix. This parameter is passed by reference
    ///// to improve performance; no changes will be made to it.</param>
    ///// <returns>
    ///// Scale vector.
    ///// </returns>
    //public static Vector3 ExtractScaleFromMatrix(ref Matrix4x4 matrix)
    //{
    //    Vector3 scale;
    //    scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
    //    scale.z = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
    //    scale.y = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
    //    return scale;
    //}

    ///// <summary>
    ///// Set transform component from TRS matrix.
    ///// </summary>
    ///// <param name="transform">Transform component.</param>
    ///// <param name="matrix">Transform matrix. This parameter is passed by reference
    ///// to improve performance; no changes will be made to it.</param>
    //public static void SetTransformFromMatrix(Transform transform, ref Matrix4x4 matrix)
    //{
    //    transform.localPosition = ExtractTranslationFromMatrix(ref matrix);
    //    transform.localRotation = ExtractRotationFromMatrix(ref matrix);
    //    transform.localScale = ExtractScaleFromMatrix(ref matrix);
    //    //transform.Rotate(0, 0, 45);
    //}



    /// <summary>
    /// Extract translation from transform matrix.
    /// </summary>
    /// <param name="matrix">Transform matrix. This parameter is passed by reference
    /// to improve performance; no changes will be made to it.</param>
    /// <returns>
    /// Translation offset.
    /// </returns>
    public static Vector3 ExtractTranslationFromMatrix(ref Matrix4x4 matrix)
    {
        Vector3 translate;
        translate.x = matrix.m03;
        translate.y = matrix.m13;
        translate.z = matrix.m23;
        return translate;
    }

    /// <summary>
    /// Extract rotation quaternion from transform matrix.
    /// </summary>
    /// <param name="matrix">Transform matrix. This parameter is passed by reference
    /// to improve performance; no changes will be made to it.</param>
    /// <returns>
    /// Quaternion representation of rotation transform.
    /// </returns>
    public static Quaternion ExtractRotationFromMatrix(ref Matrix4x4 matrix)
    {
        Vector3 forward;
        forward.x = matrix.m02;
        forward.y = matrix.m12;
        forward.z = matrix.m22;

        Vector3 upwards;
        upwards.x = matrix.m01;
        upwards.y = matrix.m11;
        upwards.z = matrix.m21;

        return Quaternion.LookRotation(forward, upwards);
    }

    /// <summary>
    /// Extract scale from transform matrix.
    /// </summary>
    /// <param name="matrix">Transform matrix. This parameter is passed by reference
    /// to improve performance; no changes will be made to it.</param>
    /// <returns>
    /// Scale vector.
    /// </returns>
    public static Vector3 ExtractScaleFromMatrix(ref Matrix4x4 matrix)
    {
        Vector3 scale;
        scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
        scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
        scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
        return scale;
    }

    /// <summary>
    /// Extract position, rotation and scale from TRS matrix.
    /// </summary>
    /// <param name="matrix">Transform matrix. This parameter is passed by reference
    /// to improve performance; no changes will be made to it.</param>
    /// <param name="localPosition">Output position.</param>
    /// <param name="localRotation">Output rotation.</param>
    /// <param name="localScale">Output scale.</param>
    public static void DecomposeMatrix(ref Matrix4x4 matrix, out Vector3 localPosition, out Quaternion localRotation, out Vector3 localScale)
    {
        localPosition = ExtractTranslationFromMatrix(ref matrix);
        localRotation = ExtractRotationFromMatrix(ref matrix);
        localScale = ExtractScaleFromMatrix(ref matrix);
    }

    /// <summary>
    /// Set transform component from TRS matrix.
    /// </summary>
    /// <param name="transform">Transform component.</param>
    /// <param name="matrix">Transform matrix. This parameter is passed by reference
    /// to improve performance; no changes will be made to it.</param>
    public static void SetTransformFromMatrix(Transform transform, ref Matrix4x4 matrix)
    {
        transform.localPosition = ExtractTranslationFromMatrix(ref matrix);
        transform.localRotation = ExtractRotationFromMatrix(ref matrix);
        //transform.localScale = ExtractScaleFromMatrix(ref matrix);
    }



    public void CreateMarkerObject(Matrix4x4 matrix, int ID, Vector3 centralPoint)
    {
        bool check = false;
        foreach (int id in m_MarkerModelDict.Keys)
        {
            if (id == ID)
            {
                check = true;
            }
        }
        if (check)
        {
            Vector3 wv = GetComponent<Camera>().ScreenToWorldPoint(centralPoint);
            Transform m_MarkerObject = Instantiate(m_MarkerModelDict[ID], wv, Quaternion.identity);
            Matrix4x4 ARM = getARM(matrix, m_MarkerObject);
            SetTransformFromMatrix(m_MarkerObject, ref ARM);
            m_MarkerObjectList.Add(m_MarkerObject);
            m_bMarkerCreated = true;
        }

    }

    private Matrix4x4 getARM(Matrix4x4 matrix, Transform m_MarkerObject)
    {
        return m_MarkerObject.localToWorldMatrix * invertYM * matrix.inverse * invertZM;
    }

    public void DestroyMakerObject()
    {
        foreach(Transform obj in m_MarkerObjectList)
        {
            Destroy(obj.gameObject);
        }
        
        m_bMarkerCreated = false;
    }

}