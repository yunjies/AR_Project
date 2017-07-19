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
    public static extern string GetAllMarkers(IntPtr imgBuffer, int width, int height);

    // Camera Setup
    public WebCamTexture webcamTexture;
    private string deviceName;
    private int devId = -1;
    private Texture2D screenshot;
    protected GCHandle m_PixelsHandle;
    private float camera_z;
    private Texture2D newTex;
   
    // Screen
    public RawImage background;

    // Object Model
    public Transform m_MarkerModelApple;
    public Transform m_MarkerModelCorrdinate;
    private Dictionary<int, Transform> m_MarkerModelDict = new Dictionary<int, Transform>();

    // Marker
    private string markerData = "";
    private List<Matrix4x4> matrixList = new List<Matrix4x4>();
    private List<int> markerids = new List<int>();
    private Vector3 position = Vector3.zero;
    private List<Vector3> m_MarkerCentralPoints = new List<Vector3>();
    private List<Transform> m_MarkerObjectList = new List<Transform>();
    private float scale;

    // GUI
    private string signal;


    // Use this for initialization
    void Start()
    {
#if UNITY_EDITOR_WIN
        screenshot = Resources.Load("12") as Texture2D;
        newTex = Instantiate(screenshot);
        TextureScale.Bilinear(newTex, Screen.width, Screen.height);
        screenshot = newTex;
        background.texture = screenshot;
        m_MarkerModelDict.Add(213, m_MarkerModelApple);
        m_MarkerModelDict.Add(212, m_MarkerModelCorrdinate);
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
        camera_z = Camera.main.GetComponent<Camera>().transform.position.z;
        signal = "initialating...";

    }

    public void OnGUI()
    {
        string strLabel = "Matrix: " + signal + "\n" + markerData;

        GUI.Label(new Rect(10, 10, 500, Screen.height), strLabel);
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
            m_MarkerObject.localScale *= scale;
            m_MarkerObjectList.Add(m_MarkerObject);
        }

    }

    // Destory all the marker at end of the frame
    public void DestroyMakerObject()
    {
        for(int i = 0; i<m_MarkerObjectList.Count; i++)
        {
            Destroy(m_MarkerObjectList[i].gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR_WIN

        m_PixelsHandle = GCHandle.Alloc(newTex.GetPixels32(), GCHandleType.Pinned);
        signal = "Raw image data ready.";

        markerData = GetAllMarkers(m_PixelsHandle.AddrOfPinnedObject(), Screen.width, Screen.height);
        signal = "DLL ready";

        m_PixelsHandle.Free();

#elif UNITY_ANDROID
        
        m_PixelsHandle = GCHandle.Alloc(webcamTexture.GetPixels32(), GCHandleType.Pinned);
        signal = "Raw image data ready.";
        markerData = GetAllMarkers(m_PixelsHandle.AddrOfPinnedObject(), Screen.width, Screen.height);
        signal = "DLL ready";
        m_PixelsHandle.Free();
        
#endif

        DestroyMakerObject();
        m_MarkerObjectList.Clear();
        m_MarkerCentralPoints.Clear();

        Reader.Read(ref matrixList, ref markerids, ref markerData, ref m_MarkerCentralPoints, camera_z, ref scale);

        for (int i = 0; i < matrixList.Count; i++)
        {
            Matrix4x4 matrix = matrixList[i];
            Vector3 centralPoint = m_MarkerCentralPoints[i];
            int ID = markerids[i];
            CreateMarkerObject(matrix, ID, centralPoint);
        }
    }




}
