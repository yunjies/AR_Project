// Detector.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "Detector.h"
#include "MarkerDetector.hpp"
#include "GeometryTypes.hpp"
#include "CameraCalibration.hpp"
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



//extern bool findMarkers(const Mat& frame, vector<Marker>& detectedMarkers);

// This is an example of an exported variable
DETECTOR_API int nDetector=0;

// This is an example of an exported function.
DETECTOR_API int fnDetector(void)
{
    return 42;
}

char* OutputFormatChar(vector<Marker> m, MarkerDetector detector, int width)
{
	string output = "";
	vector<Transformation> trans = detector.getTransformations();
	for (int i = 0; i < m.size(); i++)
	{
		string coordinate = "";
		for (int j = 0; j < 4; j++)
		{
			coordinate = ToString(m[i].points[j].x) + ToString(m[i].points[j].y);
		}
		string res = trans[i].getMat44string();
		output += (ToString(m[i].id)+ "\t"  + res + coordinate + "\n");

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

extern "C" __declspec(dllexport) char* __stdcall GetAllMarkers(void* const colors, int width, int height)
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

	//Mat rawImage(height, width, CV_8UC3, data);
	//Mat src(height, width, CV_8UC3, data);
	Mat rawImage(height, width, CV_8UC4, data);
	Mat src(height, width, CV_8UC4, data);
	cvtColor(rawImage, src, 4);
	flip(src,src,0);

	readCameraParameter(camMatrix, distCoeff, width, height);
	detector.processFrame(src, camMatrix, distCoeff, detectedMarkers);
	char* content = OutputFormatChar(detectedMarkers, detector, width);

	return content;
}


// This is the constructor of a class that has been exported.
// see Detector.h for the class definition
CDetector::CDetector()
{
    return;
}

