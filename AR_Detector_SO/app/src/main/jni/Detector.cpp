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

char* OutputFormatChar(vector<Marker> m, MarkerDetector detector, int width, vector<float> fov)
{
    string output = (ToString(fov[0]) + "\t" + ToString(fov[1]) + "\n");
    vector<Transformation> trans = detector.getTransformations();
    for (int i = 0; i < m.size(); i++)
    {
        string coordinate = "";
        for (int j = 0; j < 4; j++)
        {
            coordinate += ToString(m[i].points[j].x) + "\t" + ToString(m[i].points[j].y) + "\t";
        }
        string res = trans[i].getMat44string();
        output += (ToString(m[i].id) + "\t" + res + coordinate + "\n");

    }
    char* result = new char[output.length() + 1];
    strcpy(result, output.c_str());
    return result;
}

vector<float> readCameraParameter(Mat_<float>& camMatrix, Mat_<float>& distCoeff, double width, double height, float imageSizeScale)
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

    Size imageSize(width * imageSizeScale, height * imageSizeScale);
    double apertureWidth = 0;
    double apertureHeight = 0;
    double fovx;
    double fovy;
    double focalLength;
    Point2d principalPoint(0, 0);
    double aspectratio;

    calibrationMatrixValues(camMatrix, imageSize, apertureWidth, apertureHeight, fovx, fovy, focalLength, principalPoint, aspectratio);

    double fovXScale = (2.0 * atan((float)(imageSize.width / (2.0 * fx)))) / (atan2((float)cx, (float)fx) + atan2((float)(imageSize.width - cx), (float)fx));
    double fovYScale = (2.0 * atan((float)(imageSize.height / (2.0 * fy)))) / (atan2((float)cy, (float)fy) + atan2((float)(imageSize.height - cy), (float)fy));
    vector<float> fov;
    fov.push_back((float)(fovx * fovXScale));
    fov.push_back((float)(fovy * fovYScale));
    return fov;

}

extern "C" char* GetAllMarkers(void* const colors, int width, int height, float imageSizeScale)
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
    vector<float> fov;

    //Mat rawImage(height, width, CV_8UC3, data);
    //Mat src(height, width, CV_8UC3, data);
    Mat rawImage(height, width, CV_8UC4, data);
    Mat src(height, width, CV_8UC4, data);
    cvtColor(rawImage, src, 4);
    flip(src,src,0);

    fov = readCameraParameter(camMatrix, distCoeff, width, height, imageSizeScale);
    detector.processFrame(src, camMatrix, distCoeff, detectedMarkers);
    char* content = OutputFormatChar(detectedMarkers, detector, width, fov);

    return content;
}


