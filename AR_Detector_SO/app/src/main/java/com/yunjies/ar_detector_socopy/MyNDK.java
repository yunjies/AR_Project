package com.yunjies.ar_detector_socopy;
import org.opencv.android.OpenCVLoader;
import android.util.Log;

/**
 * Created by 1 on 2017/7/7.
 */

public class MyNDK {

    static {
        if (!OpenCVLoader.initDebug()) {
            Log.d("Error", "Unable to load OpenCV");
        } else {
            System.loadLibrary("CvExample");
        }
    }


    public native char[] GetAllMarkers(char[] data, int width, int height);
}
