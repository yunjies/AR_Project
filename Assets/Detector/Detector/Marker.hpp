#include <vector>
#include <iostream>
#include <opencv2/opencv.hpp>

#ifndef MARKER
#define MARKER

using namespace cv;
using namespace std;

class Marker
{
public:
  Marker();

  friend bool operator<(const Marker& M1,const Marker& M2);
  //friend ostream & operator << (ostream &str,const Marker &M);

  static Mat rotate(Mat in);
  static int hammDistMarker(Mat bits);
  static int mat2id(const Mat &bits);
  static int getMarkerId(Mat &in,int &nRotations);

public:
  //Id of the marker
  int id;

  vector<Point2f> points;

  //Helper function to draw the marker contour over the image
  void drawContour(Mat& image,Scalar color = CV_RGB(0,250,0)) const;
};

#endif