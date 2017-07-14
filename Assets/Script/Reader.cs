using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class Reader : MonoBehaviour {

   // private static Vector3 _screenV;
    //private static GameObject quad;
    //private static int imWidth = 640;
    //private static int imHeight = 480;

    // Use this for initialization
    void Start()
    {
        //quad = GameObject.Find("Quad");
    }

    // Update is called once per frame
    void Update()
    {
        //_screenV = GetComponent<Camera>().WorldToScreenPoint(quad.GetComponent<Transform>().position);
    }

    //public static void Read(ref List<Matrix4x4> matrixList,  ref List<KeyValuePair<int, List<KeyValuePair<float, float>>>> markers, ref List<Vector3> m_MarkerCentralPoints)
    //{
    //    matrixList.Clear();
    //    markers.Clear();
    //    //StreamReader sr = new StreamReader("c:\\Output.txt"); 
    //    StreamReader sr = new StreamReader("E:\\Output.txt");
    //    string line;
    //    while ((line = sr.ReadLine()) != null)
    //    {
    //        List<KeyValuePair<float, float>> points = new List<KeyValuePair<float, float>>();
    //        string[] elements = line.Split('\t');
    //        points.Add(new KeyValuePair<float, float>(float.Parse(elements[1]), float.Parse(elements[2])));
    //        points.Add(new KeyValuePair<float, float>(float.Parse(elements[3]), float.Parse(elements[4])));
    //        points.Add(new KeyValuePair<float, float>(float.Parse(elements[5]), float.Parse(elements[6])));
    //        points.Add(new KeyValuePair<float, float>(float.Parse(elements[7]), float.Parse(elements[8])));
    //        m_MarkerCentralPoints.Add(new Vector3((float.Parse(elements[1]) + float.Parse(elements[3]) + float.Parse(elements[5]) + float.Parse(elements[7])) / 4, 
    //            (float.Parse(elements[2]) + float.Parse(elements[4]) + float.Parse(elements[6]) + float.Parse(elements[8])) / 4, _screenV.z));
    //        //Debug.Log(m_MarkerCentralPoints[0]);
    //        markers.Add(new KeyValuePair<int, List<KeyValuePair<float, float>>>(System.Convert.ToInt32(elements[0]), points));

    //        if ((line = sr.ReadLine()) != null)
    //        {
    //            string[] num = line.Split('\t');
    //            Matrix4x4 matrix = new Matrix4x4();

    //            Vector4 v0 = new Vector4(float.Parse(num[0]), float.Parse(num[1]), float.Parse(num[2]), float.Parse(num[3]));
    //            Vector4 v1 = new Vector4(float.Parse(num[4]), float.Parse(num[5]), float.Parse(num[6]), float.Parse(num[7]));
    //            Vector4 v2 = new Vector4(float.Parse(num[8]), float.Parse(num[9]), float.Parse(num[10]), float.Parse(num[11]));
    //            Vector4 v3 = new Vector4(float.Parse(num[12]), float.Parse(num[13]), float.Parse(num[14]), float.Parse(num[15]));
    //            matrix.SetRow(0, v0);
    //            matrix.SetRow(1, v1);
    //            matrix.SetRow(2, v2);
    //            matrix.SetRow(3, v3);

    //            Debug.Log(matrix.ToString());

    //            matrixList.Add(matrix);
    //        }
    //    }


    public static void Read(ref List<Matrix4x4> matrixList, ref List<int> markerids, ref string markerData, ref List<Vector3> position)
    {
        matrixList.Clear();
        markerids.Clear();
        string[] data = markerData.Split('\n');
        //Debug.Log(data);
        foreach(string line in data)
        {
            if (line == "")
                continue;
            string[] num = line.Split('\t');
            int id = int.Parse(num[0]);
            markerids.Add(id);

            position.Add(new Vector3(float.Parse(num[1]), float.Parse(num[2]), 5));

            //Debug.Log(num[0]);

            Matrix4x4 matrix = new Matrix4x4();
            Vector4 v0 = new Vector4(float.Parse(num[3]), float.Parse(num[4]), float.Parse(num[5]), float.Parse(num[6]));
            Vector4 v1 = new Vector4(float.Parse(num[7]), float.Parse(num[8]), float.Parse(num[9]), float.Parse(num[10]));
            Vector4 v2 = new Vector4(float.Parse(num[11]), float.Parse(num[12]), float.Parse(num[13]), float.Parse(num[14]));
            Vector4 v3 = new Vector4(float.Parse(num[15]), float.Parse(num[16]), float.Parse(num[17]), float.Parse(num[18]));
            matrix.SetRow(0, v0);
            matrix.SetRow(1, v1);
            matrix.SetRow(2, v2);
            matrix.SetRow(3, v3);

            //Debug.Log(matrix.ToString());

            matrixList.Add(matrix);
        }
    }



}