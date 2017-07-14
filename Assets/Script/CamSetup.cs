using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Runtime.InteropServices;
using System.IO;
using System;
using UnityEngine.UI;

public class CamSetup : MonoBehaviour
{
    // DLL Import
    [DllImport("Detector")]
    public static extern string GetAllMarkers(byte[] imgBuffer, int width, int height);

    // Camera Setup
    public WebCamTexture webcamTexture;
    public string deviceName;
    private int devId = -1;
    Texture2D screenshot;

    // Screen
    public RawImage background;

    // Object Model
    public Transform m_MarkerModelApple;
    public Transform m_MarkerModelCorrdinate;
    public Dictionary<int, Transform> m_MarkerModelDict = new Dictionary<int, Transform>();

    // Marker
    private string markerData = "";
    private List<Matrix4x4> matrixList = new List<Matrix4x4>();
    private List<int> markerids = new List<int>();
    Vector3 position = Vector3.zero;
    private List<Vector3> m_MarkerCentralPoints = new List<Vector3>();
    private List<Transform> m_MarkerObjectList = new List<Transform>();
    bool m_bMarkerCreated = false;

    // GUI
    string signal = "initialating...";


    // Use this for initialization
    void Start()
    {
#if UNITY_EDITOR_WIN
        screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        m_MarkerModelDict.Add(212, m_MarkerModelApple);
        m_MarkerModelDict.Add(213, m_MarkerModelCorrdinate);
#elif UNITY_ANDROID
        WebCamDevice[] devices = WebCamTexture.devices;
        Debug.Log("num:" + devices.Length);

        // Find front camera
        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].name.CompareTo(deviceName) == 1 && !devices[i].isFrontFacing)
            {
                devId = i;
            }
        }

        // Initializa default screen
        if (devId >= 0)
        {
            screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            m_MarkerModelDict.Add(213, m_MarkerModelApple);
            m_MarkerModelDict.Add(212, m_MarkerModelCorrdinate);
            webcamTexture = new WebCamTexture(devices[devId].name, Screen.width, Screen.height, 60);
            webcamTexture.Play();
            background.texture = webcamTexture;
        }
#endif



    }

    public void OnGUI()
    {
        string strLabel = "Matrix: " + signal + markerData;

        GUI.Label(new Rect(10, 10, 500, 20), strLabel);
    }

    // Create a new object on the Marker based on the data passing from DLL
    public void CreateMarkerObject(Matrix4x4 matrix, int ID, Vector3 position)
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
            position.y = Screen.height - position.y;
            position = GetComponent<Camera>().ScreenToWorldPoint(position);
            Transform m_MarkerObject = Instantiate(m_MarkerModelDict[ID], position, Quaternion.identity);
            Matrix4x4 ARM = matrix * m_MarkerObject.localToWorldMatrix;
            SetTransform.SetTransformFromMatrix(m_MarkerObject, ref ARM);
            m_MarkerObjectList.Add(m_MarkerObject);
            m_bMarkerCreated = true;
        }

    }

    // Destory all the marker at end of the frame
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

#if UNITY_EDITOR_WIN   
        screenshot = Resources.Load("5") as Texture2D;
        var newTex = Instantiate(screenshot);
        TextureScale.Bilinear(newTex, Screen.width, Screen.height);
        screenshot = newTex;
        background.texture = screenshot;

        //buf = new CommandBuffer();
        //buf.name = "RenderScene";

        //buf.Blit(screenshot, BuiltinRenderTextureType.CurrentActive);
        //GetComponent<Camera>().AddCommandBuffer(CameraEvent.AfterSkybox, buf);

        signal = "texture ready.";


        //buf.Dispose();
#elif UNITY_ANDROID

        screenshot.SetPixels(webcamTexture.GetPixels());
        
#endif

        byte[] imgBuffer = screenshot.GetRawTextureData();
        signal = "Raw image data ready.";

        markerData = GetAllMarkers(imgBuffer, Screen.width, Screen.height);
        signal = "DLL ready";

        if (m_bMarkerCreated)
        {
            DestroyMakerObject();
            m_MarkerObjectList.Clear();
            m_MarkerCentralPoints.Clear();
        }

        Reader.Read(ref matrixList, ref markerids, ref markerData, ref m_MarkerCentralPoints);

        for (int i = 0; i < matrixList.Count; i++)
        {
            Matrix4x4 matrix = matrixList[i];
            Vector3 centralPoint = m_MarkerCentralPoints[i];
            int ID = markerids[i];
            CreateMarkerObject(matrix, ID, centralPoint);
        }



    }




}
