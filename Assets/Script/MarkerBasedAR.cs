using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerBasedAR : MonoBehaviour {



    private List<Matrix4x4> matrixList = new List<Matrix4x4>();
    private List<KeyValuePair<int, List<KeyValuePair<float, float>>>> markers = new List<KeyValuePair<int, List<KeyValuePair<float, float>>>>();
    public Transform m_MarkerModelApple;
    bool m_bMarkerCreated = false;
    private List<Vector3> m_MarkerCentralPoints = new List<Vector3>();


    Matrix4x4 invertYM = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1, -1, 1));
    Matrix4x4 invertZM = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(1, 1, -1));
    //public Vector3 Angles = new Quaternion.EulerAngles(90, 0, 0);

    private List<Transform> m_MarkerObjectList = new List<Transform>();
    public Dictionary<int, Transform> m_MarkerModelDict = new Dictionary<int, Transform>();

    // Use this for initialization
    void Start()
    {
        m_MarkerModelDict.Add(213, m_MarkerModelApple);
    }

    // Update is called once per frame
    void Update()
    {


        if (m_bMarkerCreated)
        {
            DestroyMakerObject();
            m_MarkerObjectList.Clear();
            m_MarkerCentralPoints.Clear();
        }

        Reader.Read(ref matrixList, ref markers, ref m_MarkerCentralPoints);

        for (int i = 0; i < matrixList.Count; i++)
        {
            Matrix4x4 matrix = matrixList[i];
            int ID = markers[i].Key;
            Vector3 centralPoint = m_MarkerCentralPoints[i];
            CreateMarkerObject(matrix, ID, centralPoint);
        }

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
            Matrix4x4 ARM = NewMethod(matrix, m_MarkerObject);
            SetTransform.SetTransformFromMatrix(m_MarkerObject, ref ARM);
            m_MarkerObjectList.Add(m_MarkerObject);
            m_bMarkerCreated = true;
        }

    }

    private Matrix4x4 NewMethod(Matrix4x4 matrix, Transform m_MarkerObject)
    {
        return m_MarkerObject.localToWorldMatrix * (invertZM * matrix.inverse);
    }

    public void DestroyMakerObject()
    {
        foreach (Transform obj in m_MarkerObjectList)
        {
            Destroy(obj.gameObject);
        }

        m_bMarkerCreated = false;
    }
}
