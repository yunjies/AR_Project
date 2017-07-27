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
    public static extern string GetAllMarkers(IntPtr imgBuffer, int width, int height, float imageSizeScale);

    // Camera Setup
    public Camera backgroundCam;
    public WebCamTexture webcamTexture;
    private string deviceName;
    private int devId = -1;
    private int imWidth = Screen.width / 4;
    private int imHeight = Screen.height / 4;
    private Texture2D screenshot;
    private Texture2D readTexture;
    protected GCHandle m_PixelsHandle;
    private Texture2D newTex;
    private float imageSizeScale;
    private float widthScale;
    private float heightScale;
    private List<float> fov = new List<float>();

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
    private List<Transform> m_MarkerObjectList = new List<Transform>();

    // GUI
    private string signal;


    // Use this for initialization
    void Start()
    {
#if UNITY_EDITOR_WIN
        readTexture = new Texture2D(imWidth, imHeight, TextureFormat.RGB24, false);
        screenshot = Resources.Load("7") as Texture2D;
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
            readTexture = new Texture2D(imWidth, imHeight, TextureFormat.RGB24, false);
            m_MarkerModelDict.Add(213, m_MarkerModelApple);
            m_MarkerModelDict.Add(212, m_MarkerModelCorrdinate);
            webcamTexture = new WebCamTexture(devices[devId].name, Screen.width, Screen.height, 30);
            background.texture = webcamTexture;
            webcamTexture.Play();
        }
        



#endif


        imageSizeScale = 1.0f;
        widthScale = (float)Screen.width / imWidth;
        heightScale = (float)Screen.height / imHeight;
        if (widthScale < heightScale)
        {
            backgroundCam.orthographicSize = (imWidth * (float)Screen.height / (float)Screen.width) / 2;
            imageSizeScale = (float)Screen.height / (float)Screen.width;
        }
        else
        {
            backgroundCam.orthographicSize = imHeight / 2;
        }
        signal = "initialating...";

    }
    public void OnGUI()
    {
        //string strLabel = "Matrix: " + signal + "\n" + markerData;

        //GUI.Label(new Rect(10, 10, 500, Screen.height), strLabel);
    }

    // Create a new object on the Marker based on the data passing from DLL
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
            Matrix4x4 ARM = matrix * m_MarkerObject.localToWorldMatrix;
            SetTransform.SetTransformFromMatrix(m_MarkerObject, ref ARM);
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

    /// Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR_WIN

        var renderTexture = RenderTexture.GetTemporary(imWidth, imHeight);
        Graphics.Blit(screenshot, renderTexture);
        RenderTexture.active = renderTexture;

        readTexture.ReadPixels(new Rect(0, 0, imWidth, imHeight), 0, 0);

        m_PixelsHandle = GCHandle.Alloc(readTexture.GetPixels32(), GCHandleType.Pinned);
        signal = "Raw image data ready.";

        markerData = GetAllMarkers(m_PixelsHandle.AddrOfPinnedObject(), imWidth, imHeight, imageSizeScale);
         signal = "DLL ready";

    
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(renderTexture);
        m_PixelsHandle.Free();

#elif UNITY_ANDROID

        var renderTexture = RenderTexture.GetTemporary(imWidth, imHeight);
        Graphics.Blit(webcamTexture, renderTexture);
        RenderTexture.active = renderTexture;

        readTexture.ReadPixels(new Rect(0, 0, imWidth, imHeight), 0, 0);
        
        m_PixelsHandle = GCHandle.Alloc(readTexture.GetPixels32(), GCHandleType.Pinned);
        signal = "Raw image data ready.";

        markerData = GetAllMarkers(m_PixelsHandle.AddrOfPinnedObject(), imWidth, imHeight, imageSizeScale);
        signal = "DLL ready";

        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(renderTexture);
        m_PixelsHandle.Free();
        
#endif

        DestroyMakerObject();
        m_MarkerObjectList.Clear();
        fov.Clear();

        if (Reader.Read(ref matrixList, ref markerids, ref markerData, ref fov))
        {

            if (widthScale < heightScale)
            {
                Camera.main.fieldOfView = fov[0];
            }
            else
            {
                Camera.main.fieldOfView = fov[1];
            }

            for (int i = 0; i < matrixList.Count; i++)
            {
                Matrix4x4 matrix = matrixList[i];
                int ID = markerids[i];
                CreateMarkerObject(matrix, ID);
            }
        }
    }




}
