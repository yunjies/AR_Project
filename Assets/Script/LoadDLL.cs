using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;


public class LoadDLL : MonoBehaviour
{

//#if UNITY_IPHONE
   
//       // On iOS plugins are statically linked into
//       // the executable, so we have to use __Internal as the
//       // library name.
//       [DllImport ("__Internal")]

//#else

//    // Other platforms load plugins dynamically, so pass the name
//    // of the plugin's dynamic library.
//    [DllImport("Detector")]

//#endif
    [DllImport("Detector")]
    public static extern int FindAllMarkers();

    public WebCamTexture webcamTexture;
    public GameObject planeObj;
    public string deviceName;
    private int devId = 1;
    private int imWidth = 640;
    private int imHeight = 480;
    private Texture2D screenshot;



    // Use this for initialization
    void Start ()
    {

        WebCamDevice[] devices = WebCamTexture.devices;
        Debug.Log("num:" + devices.Length);

        for (int i = 0; i < devices.Length; i++)
        {
            print(devices[i].name);
            if (devices[i].name.CompareTo(deviceName) == 1)
            {
                devId = i;
            }
        }

        if (devId >= 0)
        {
            planeObj = GameObject.Find("Plane");
            screenshot = new Texture2D(imWidth, imHeight, TextureFormat.RGB24, false);
            webcamTexture = new WebCamTexture(devices[devId].name, imWidth, imHeight, 60);
            webcamTexture.Play();

        }



    }
	
	// Update is called once per frame
	void Update ()
    {
        screenshot.SetPixels(webcamTexture.GetPixels());
        screenshot.Apply();
        planeObj.GetComponent<MeshRenderer>().material.mainTexture = screenshot;

        byte[] byt = screenshot.EncodeToPNG();
        File.WriteAllBytes("c:\\capture.jpg", byt);

        //Application.CaptureScreenshot("c:\\capture.jpg");
        FindAllMarkers();
    }
}
