using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;


public class test : MonoBehaviour {

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
        List<KeyValuePair<int, List<KeyValuePair<double, double>>>> markers = new List<KeyValuePair<int, List<KeyValuePair<double, double>>>>();
        screenshot.SetPixels(webcamTexture.GetPixels());
        screenshot.Apply();
        planeObj.GetComponent<MeshRenderer>().material.mainTexture = screenshot;

        byte[] byt = screenshot.EncodeToPNG();
        File.WriteAllBytes("capture.jpg", byt);

        //Application.CaptureScreenshot("c:\\capture.jpg");
        int i = FindAllMarkers();
        ReadFile(ref markers);
        Debug.Log(markers[0].Key);

    }

    public void ReadFile(ref List<KeyValuePair<int, List<KeyValuePair<double, double>>>> m)
    {
        StreamReader sr = new StreamReader("c:\\capture.jpg");
        string line;
        while ((line = sr.ReadLine()) != null)
        {
            List<KeyValuePair<double, double>> points = new List<KeyValuePair<double, double>>();
            string[] elements = line.Split('\t');
            points.Add(new KeyValuePair<double, double>(System.Convert.ToDouble(elements[1]), System.Convert.ToDouble(elements[2])));
            points.Add(new KeyValuePair<double, double>(System.Convert.ToDouble(elements[3]), System.Convert.ToDouble(elements[4])));
            points.Add(new KeyValuePair<double, double>(System.Convert.ToDouble(elements[5]), System.Convert.ToDouble(elements[6])));
            points.Add(new KeyValuePair<double, double>(System.Convert.ToDouble(elements[7]), System.Convert.ToDouble(elements[8])));
            m.Add(new KeyValuePair<int, List<KeyValuePair<double, double>>>(System.Convert.ToInt32(elements[0]), points));
        }
    }
}
