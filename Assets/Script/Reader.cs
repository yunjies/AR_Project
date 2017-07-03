using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class Reader : MonoBehaviour {

    private static Vector3 _screenV;
    private static GameObject plane;
    private static int imWidth = 640;
    private static int imHeight = 480;

    // Use this for initialization
    void Start()
    {
        plane = GameObject.Find("Plane");
    }

    // Update is called once per frame
    void Update()
    {
        _screenV = GetComponent<Camera>().WorldToScreenPoint(plane.GetComponent<Transform>().position);
    }

    public static void Read(ref List<Matrix4x4> matrixList,  ref List<KeyValuePair<int, List<KeyValuePair<float, float>>>> markers, ref List<Vector3> m_MarkerCentralPoints)
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
            m_MarkerCentralPoints.Add(new Vector3((float.Parse(elements[1]) + float.Parse(elements[3]) + float.Parse(elements[5]) + float.Parse(elements[7])) / 4, 
                imHeight - (float.Parse(elements[2]) + float.Parse(elements[4]) + float.Parse(elements[6]) + float.Parse(elements[8])) / 4, _screenV.z - 0.5f));
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



}