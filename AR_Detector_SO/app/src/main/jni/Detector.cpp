//
// Created by 1 on 2017/7/10.
//

#include "MarkerDetector.hpp"
#include "GeometryTypes.hpp"
#include <opencv2/opencv.hpp>
#include <iostream>
#include <string>
#include <stdlib.h>
#include <stdio.h>
#include <fstream>
#include <vector>
#include <sstream>
#include <algorithm>

using namespace std;
using namespace cv;

char* OutputFormatChar(vector<Marker> m, MarkerDetector detector, int width)
{
    string output = "";
    vector<Transformation> trans = detector.getTransformations();
    for (int i = 0; i < m.size(); i++)
    {
        float x = 0;
        float y = 0;
        for (int j = 0; j < 4; j++)
        {
            x += m[i].points[j].x;
            y += m[i].points[j].y;
        }
        string res = trans[i].getMat44string();
        float scale = sqrt(pow((m[i].points[1].x - m[i].points[2].x), 2) + pow((m[i].points[1].y - m[i].points[2].y), 2)) / width;
        output += (ToString(m[i].id)+ "\t" + ToString(x / 4) + "\t" + ToString(y / 4) + "\t" + res + ToString(scale) + "\n");

    }
    char* result = new char[output.length() + 1];
    strcpy(result, output.c_str());
    return result;
}

void readCameraParameter(Mat_<float>& camMatrix, Mat_<float>& distCoeff, double width, double height)
{
    //set cameraparam
    int max_d = (int)max(width, height);
    double fx = max_d;
    double fy = max_d;
    double cx = width / 2.0f;
    double cy = height / 2.0f;

    camMatrix(0, 0) = fx;
    camMatrix(1, 1) = fy;
    camMatrix(0, 2) = cx;
    camMatrix(1, 2) = cy;

    for (int i = 0; i<4; i++)
        distCoeff(i, 0) = 0;

}

extern "C" char* GetAllMarkers(void* const colors, int width, int height)
{
    // safeguard - array must be not null
    if (!colors)
        return 0;

    // Color structure in Unity is four RGBA floats
    unsigned char* data = reinterpret_cast<unsigned char*>(colors);

    MarkerDetector detector;
    Mat_<float> camMatrix = Mat::eye(3, 3, CV_64F);
    Mat_<float> distCoeff = Mat::zeros(8, 1, CV_64F);
    vector<Marker> detectedMarkers;


    Mat rawImage(height, width, CV_8UC4, data);
    Mat src(height, width, CV_8UC4, data);
    cvtColor(rawImage, src, 4);
    flip(src,src,0);

    readCameraParameter(camMatrix, distCoeff, width, height);
    detector.processFrame(src, camMatrix, distCoeff, detectedMarkers);
    char* content = OutputFormatChar(detectedMarkers, detector, width);

    return content;
}


