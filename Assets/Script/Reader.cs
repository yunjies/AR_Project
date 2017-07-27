using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class Reader : MonoBehaviour {

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public static bool Read(ref List<Matrix4x4> matrixList, ref List<int> markerids, ref string markerData, ref List<float> fov)
    {
        matrixList.Clear();
        markerids.Clear();
        string[] data = markerData.Split('\n');
        string[] fovString = data[0].Split('\t');
        fov.Add(float.Parse(fovString[0]));
        fov.Add(float.Parse(fovString[1]));

        //Debug.Log(data);
        for (int i = 1; i < data.Length; i++)
        {
            if (data[i] == "")
            {
                if(i == 1)
                {
                    return false;
                }
                continue;
            }

            string[] num = data[i].Split('\t');
            int id = int.Parse(num[0]);
            markerids.Add(id);
            
            //Debug.Log(num[0]);

            Matrix4x4 matrix = new Matrix4x4();
            Vector4 v0 = new Vector4(float.Parse(num[1]), float.Parse(num[2]), float.Parse(num[3]), float.Parse(num[4]));
            Vector4 v1 = new Vector4(float.Parse(num[5]), float.Parse(num[6]), float.Parse(num[7]), float.Parse(num[8]));
            Vector4 v2 = new Vector4(float.Parse(num[9]), float.Parse(num[10]), float.Parse(num[11]), float.Parse(num[12]));
            Vector4 v3 = new Vector4(float.Parse(num[13]), float.Parse(num[14]), float.Parse(num[15]), float.Parse(num[16]));
            matrix.SetRow(0, v0);
            matrix.SetRow(1, v1);
            matrix.SetRow(2, v2);
            matrix.SetRow(3, v3);

            matrixList.Add(matrix);
        }
        return true;
    }



}
