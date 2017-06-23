// Detector.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "Detector.h"
#include "MarkerDetector.hpp"
#include <opencv2/opencv.hpp>
#include <iostream>
#include <string>
#include <stdlib.h>  
#include <stdio.h>
#include <fstream>
#include <vector>
#include <sstream>


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

void SaveFile(string  filename, string s)
{
	ofstream file(filename);
	if (file.is_open())
	{
		file << s;
	}
	file.close();
}

string OutputFormat(vector<Marker> m)
{
	string output = "";
	for (int i = 0; i < m.size(); i++)
	{
		stringstream iss;
		iss << m[i].id << "\t";
		for (int j = 0; j < m[i].points.size(); j++)
		{
			iss << m[i].points[j].x << "\t" << m[i].points[j].y << "\t";
		}
		iss << endl;
		output += iss.str();
	}
	return output;
}

extern "C" __declspec(dllexport) int FindAllMarkers()
{
	vector<Marker> detectedMarkers;
	MarkerDetector detector;
	string filename = "C:\\capture.jpg";
	Mat src = imread(filename);
	string content;
	//MessageBox(NULL, L"Start", L"Main", MB_OK);
	detector.findMarkers(src, detectedMarkers);
	content = OutputFormat(detectedMarkers);
	SaveFile("C:\\Output.txt", content);

	return 1;
}

// This is the constructor of a class that has been exported.
// see Detector.h for the class definition
CDetector::CDetector()
{
    return;
}

