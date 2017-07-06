using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;
using System;


public class CamSetup : MonoBehaviour
{

    [DllImport("Detector", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    [return: MarshalAs(UnmanagedType.LPStr)]
    public static extern string GetAllMarkers(byte[] imgBuffer, int width, int height);

    public WebCamTexture webcamTexture;
    public GameObject planeObj;
    public string deviceName;
    private int devId = -1;
    private int imWidth =  640;
    private int imHeight = 480;
    private string markerData = "";
    Texture2D screenshot;

    private List<Matrix4x4> matrixList = new List<Matrix4x4>();
    private List<int> markerids = new List<int>();
    public Transform m_MarkerModelApple;
    bool m_bMarkerCreated = false;
    private List<Vector3> m_MarkerCentralPoints = new List<Vector3>();

    private List<Transform> m_MarkerObjectList = new List<Transform>();
    public Dictionary<int, Transform> m_MarkerModelDict = new Dictionary<int, Transform>();
    private int iCamNum = 0;

    string signal = "initialating...";



    // Use this for initialization
    void Start()
    {

        //planeObj = GameObject.Find("Plane");
        //screenshot = new Texture2D(imWidth, imHeight, TextureFormat.RGB24, false);
        //m_MarkerModelDict.Add(213, m_MarkerModelApple);


        WebCamDevice[] devices = WebCamTexture.devices;
        Debug.Log("num:" + devices.Length);
        iCamNum = devices.Length;

        for (int i = 0; i < devices.Length; i++)
        {
            print(devices[i].name);
            if (devices[i].name.CompareTo(deviceName) == 1 && !devices[i].isFrontFacing)
            {
                devId = i;
            }
        }

        if (devId >= 0)
        {
            planeObj = GameObject.Find("Plane");
            screenshot = new Texture2D(imWidth, imHeight, TextureFormat.RGB24, false);
            m_MarkerModelDict.Add(213, m_MarkerModelApple);
            webcamTexture = new WebCamTexture(devices[devId].name, imWidth, imHeight, 60);
            //planeObj.GetComponent<Renderer>().material.mainTexture = webcamTexture;
            webcamTexture.Play();

        }


    }

    public void OnGUI()
    {
        string strLabel = "Matrix: " + signal + markerData;

        GUI.Label(new Rect(10, 10, 500, 20), strLabel);
    }

    public void CreateMarkerObject(Matrix4x4 matrix, int ID)
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
            Transform m_MarkerObject = Instantiate(m_MarkerModelDict[ID], Vector3.zero, Quaternion.identity);
            Matrix4x4 ARM = NewMethod(matrix, m_MarkerObject);
            SetTransform.SetTransformFromMatrix(m_MarkerObject, ref ARM);
            m_MarkerObjectList.Add(m_MarkerObject);
            m_bMarkerCreated = true;
        }

    }

    private Matrix4x4 NewMethod(Matrix4x4 matrix, Transform m_MarkerObject)
    {
        return matrix * m_MarkerObject.localToWorldMatrix;
        //return m_MarkerObject.localToWorldMatrix * invertYM * matrix * invertZM;
    }

    public void DestroyMakerObject()
    {
        foreach (Transform obj in m_MarkerObjectList)
        {
            Destroy(obj.gameObject);
        }

        m_bMarkerCreated = false;
    }

    // Update is called once per frame
    void Update()
    {
        ////创建文件读取流
        //Texture2D cc=Resources.Load("2") as Texture2D;
        //screenshot = cc;
        //screenshot.Apply();
        //planeObj.GetComponent<Renderer>().material.mainTexture = screenshot;

        screenshot.SetPixels(webcamTexture.GetPixels());
        screenshot.Apply();
        planeObj.GetComponent<MeshRenderer>().material.mainTexture = screenshot;
        signal = "texture ready.";

        byte[] imgBuffer = screenshot.EncodeToPNG();
        signal = "Raw image data ready.";

        markerData = GetAllMarkers(imgBuffer, imWidth, imHeight);
        signal = "DLL ready";


        if (m_bMarkerCreated)
        {
            DestroyMakerObject();
            m_MarkerObjectList.Clear();
            m_MarkerCentralPoints.Clear();
        }

        Reader.Read(ref matrixList, ref markerids, ref markerData);

        for (int i = 0; i < matrixList.Count; i++)
        {
            Matrix4x4 matrix = matrixList[i];
            int ID = markerids[i];
            CreateMarkerObject(matrix, ID);
        }



    }




}
