using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCvSharp;


public class CaptureScript : MonoBehaviour
{
    public GameObject planeObj;
	public WebCamTexture webcamTexture;
    public Texture2D texImage;
	public string deviceName;
	private int devId = 1;
	private int imWidth = 640;
	private int imHeight = 480;
	private string errorMsg = "No errors found!";
	static IplImage matrix;

    // Use this for initialization
	void Start ()
	{
		WebCamDevice[] devices = WebCamTexture.devices;
		Debug.Log ("num:" + devices.Length);

	    for (int i=0; i<devices.Length; i++)
        {
			print (devices [i].name);
			if (devices [i].name.CompareTo (deviceName) == 1)
            {
				devId = i;
            }
		}

		if (devId >= 0)
        {
			planeObj = GameObject.Find ("Plane");
			texImage = new Texture2D (imWidth, imHeight, TextureFormat.RGB24, false);
			webcamTexture = new WebCamTexture (devices [devId].name, imWidth, imHeight, 60);
			webcamTexture.Play ();

            //matrix = new IplImage (cvSize(imWidth, imHeight), BitDepth.U8, 3);
        }
	}

	void Update ()
	{
		if (devId >= 0)
        {
			//Texture2DtoIplImage ();

			CvFont font = new CvFont (FontFace.Vector0, 1.0, 1.0);
			CvColor rcolor = CvColor.Random ();
			Cv.PutText (matrix, "Snapshot taken!", new CvPoint (15, 30), font, rcolor);

			IplImage cny = new IplImage (imWidth, imHeight, BitDepth.U8, 1);
			matrix.CvtColor (cny, ColorConversion.RgbToGray);

			Cv.Canny (cny, cny, 50, 50, ApertureSize.Size3);

			Cv.CvtColor(cny, matrix, ColorConversion.GrayToBgr);

            if (webcamTexture.didUpdateThisFrame)
            {
                IplImageToTexture2D();
            }

        }
        else
        {
	    	Debug.Log ("Can't find camera!");
		}
	}

	void OnGUI ()
	{
		GUI.Label (new Rect (200, 200, 100, 90), errorMsg);
	}

	void IplImageToTexture2D ()
	{
		int jBackwards = imHeight;

		for (int i = 0; i < imHeight; i++)
        {
			for (int j = 0; j < imWidth; j++)
            {
				float b = (float)matrix [i, j].Val0;
				float g = (float)matrix [i, j].Val1;
				float r = (float)matrix [i, j].Val2;
				Color color = new Color (r / 255.0f, g / 255.0f, b / 255.0f);


				jBackwards = imHeight - i - 1; // notice it is jBackward and i
				texImage.SetPixel (j, jBackwards, color);
			}
		}
		texImage.Apply ();
		GetComponent<MeshRenderer>().material.mainTexture = texImage;

	}

		//void Texture2DtoIplImage ()
		//{
		//		int jBackwards = imHeight;

		//		for (int v=0; v<imHeight; ++v) {
		//				for (int u=0; u<imWidth; ++u) {

		//						CvScalar col = new CvScalar ();
		//						col.Val0 = (double)webcamTexture.GetPixel (u, v).b * 255;
		//						col.Val1 = (double)webcamTexture.GetPixel (u, v).g * 255;
		//						col.Val2 = (double)webcamTexture.GetPixel (u, v).r * 255;

		//						jBackwards = imHeight - v - 1;

		//						matrix.Set2D (jBackwards, u, col);
		//						//matrix [jBackwards, u] = col;
		//				}
		//		}
		//		Cv.SaveImage ("C:\\Hasan.jpg", matrix);
		//}
}
