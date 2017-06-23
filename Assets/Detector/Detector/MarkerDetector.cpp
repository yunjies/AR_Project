#include "stdafx.h"
#include <stdio.h>
#include <stdlib.h>
#include <iostream>
#include <sstream>       //stringstream

#include "MarkerDetector.hpp"

using namespace cv;
using namespace std;


float perimeter(const vector<Point2f> &a)//�������ܳ���
{
	float sum = 0, dx, dy;
	for (size_t i = 0; i<a.size(); i++)
	{
		size_t i2 = (i + 1) % a.size();

		dx = a[i].x - a[i2].x;
		dy = a[i].y - a[i2].y;

		sum += sqrt(dx*dx + dy*dy);
	}

	return sum;
}

MarkerDetector::MarkerDetector()
	:m_minContourLengthAllowed(100.0)
	, markerSize(100, 100)
{
	bool centerOrigin = true;
	if (centerOrigin)
	{
		m_markerCorners3d.push_back(Point3f(-0.5f, -0.5f, 0));
		m_markerCorners3d.push_back(Point3f(+0.5f, -0.5f, 0));
		m_markerCorners3d.push_back(Point3f(+0.5f, +0.5f, 0));
		m_markerCorners3d.push_back(Point3f(-0.5f, +0.5f, 0));
	}
	else
	{
		m_markerCorners3d.push_back(Point3f(0, 0, 0));
		m_markerCorners3d.push_back(Point3f(1, 0, 0));
		m_markerCorners3d.push_back(Point3f(1, 1, 0));
		m_markerCorners3d.push_back(Point3f(0, 1, 0));
	}

	m_markerCorners2d.push_back(Point2f(0, 0));
	m_markerCorners2d.push_back(Point2f(markerSize.width - 1, 0));
	m_markerCorners2d.push_back(Point2f(markerSize.width - 1, markerSize.height - 1));
	m_markerCorners2d.push_back(Point2f(0, markerSize.height - 1));

}


void MarkerDetector::prepareImage(const Mat& src, Mat& grayscale)
{
	//��ɫת���ɻ�ɫͼ��
	cvtColor(src, grayscale, CV_BGRA2GRAY);
	//imshow("grey image.jpg", grayscale);
}

//������ֵ���ȡ���ڹ�����������ǿ�ȱ仯����������Ӧ��ֵ����������Ϊ��λ���������뾶�ڵ��������ص�ƽ��ǿ����Ϊ�����ص�ǿ�ȣ�ʹ��������������������³���ԡ�
void MarkerDetector::performThreshold(const Mat& grayscale, Mat& thresholdImg)
{
	/*����ͼ��
	//���ͼ��
	//ʹ�� CV_THRESH_BINARY �� CV_THRESH_BINARY_INV �����ֵ
	//����Ӧ��ֵ�㷨ʹ�ã�CV_ADAPTIVE_THRESH_MEAN_C �� CV_ADAPTIVE_THRESH_GAUSSIAN_C
	//ȡ��ֵ���ͣ�����������֮һ
	//CV_THRESH_BINARY,
	//CV_THRESH_BINARY_INV
	//����������ֵ�����������С: 3, 5, 7, ...
	*/
	adaptiveThreshold(grayscale,//Input Image
		thresholdImg,//Result binary image
		255,
		ADAPTIVE_THRESH_GAUSSIAN_C,
		THRESH_BINARY_INV,
		7,
		7
	);
	//imshow("Threshold image.jpg", thresholdImg);
#ifdef SHOW_DEBUG_IMAGES
	imshow("Threshold image", thresholdImg);
	imwrite("Threshold image" + ".png", thresholdImg);
#endif
}
//���������Ķ�ֵͼ�������������һ��������б���ÿ������α�ʶһ��������С��������ע�����������...
void MarkerDetector::findContour(cv::Mat& thresholdImg, ContoursVector& contours, int minContourPointsAllowed) const
{
	ContoursVector allContours;
	/*����ͼ��image����Ϊһ��2ֵ��ͨ��ͼ��
	//�����������飬ÿһ��������һ��point���͵�vector��ʾ
	//�����ļ���ģʽ

	CV_RETR_EXTERNAL��ʾֻ���������
	CV_RETR_LIST���������������ȼ���ϵ
	CV_RETR_CCOMP���������ȼ��������������һ��Ϊ��߽磬�����һ��Ϊ�ڿ׵ı߽���Ϣ������ڿ��ڻ���һ����ͨ���壬�������ı߽�Ҳ�ڶ��㡣
	CV_RETR_TREE����һ���ȼ����ṹ������������ο�contours.c���demo

	//�����Ľ��ư취

	CV_CHAIN_APPROX_NONE�洢���е������㣬���ڵ������������λ�ò����1����max��abs��x1-x2����abs��y2-y1����==1
	CV_CHAIN_APPROX_SIMPLEѹ��ˮƽ���򣬴�ֱ���򣬶Խ��߷����Ԫ�أ�ֻ�����÷�����յ����꣬����һ����������ֻ��4����������������Ϣ
	CV_CHAIN_APPROX_TC89_L1��CV_CHAIN_APPROX_TC89_KCOSʹ��teh-Chinl chain �����㷨
	offset��ʾ�����������ƫ��������������Ϊ����ֵ����ROIͼ�����ҳ�����������Ҫ������ͼ���н��з���ʱ������������Ǻ����õġ�
	*/

	findContours(thresholdImg, allContours, CV_RETR_LIST, CV_CHAIN_APPROX_NONE);


	contours.clear();
	for (size_t i = 0; i<allContours.size(); i++)
	{
		int contourSize = allContours[i].size();
		if (contourSize > minContourPointsAllowed)
		{
			contours.push_back(allContours[i]);
		}
	}
	//Mat result(thresholdImg.size(),CV_8U,Scalar(0));
	//drawContours(result,contours,-1,Scalar(255),2); 
	//imshow("Contours",result);
#ifdef SHOW_DEBUG_IMAGES
	{
		Mat contoursImage(thresholdImg.size(), CV_8UC1);
		contoursImage = Scalar(0);
		drawContours(contoursImage, contours, -1, cv::Scalar(255), 2, CV_AA);
		imshow("Contours", contoursImage);
		imwrite("Contours" + ".png", contoursImage);
	}
#endif
}

//�������ǵı�����ı��Σ����ҵ�ͼ����������ϸ�ں󣬱�����Opencv����API������Σ�ͨ���ж϶���ζ��������Ƿ�Ϊ4���ı��θ�����֮���໥�����Ƿ�����Ҫ��(�ı����Ƿ��㹻��)�����˷Ǻ�ѡ����Ȼ���ٸ��ݺ�ѡ����֮������һ��ɸѡ���õ����յĺ�ѡ���򣬲�ʹ�ú�ѡ����Ķ���������ʱ�����С�
void MarkerDetector::findCandidates(const ContoursVector& contours, vector<Marker>& detectedMarkers)
{
	vector<Point> approxCurve;//���ؽ��Ϊ����Σ��õ㼯��ʾ//������״
	vector<Marker> possibleMarkers;//���ܵı��

								   //For each contour,�������ǲ������ʶ���ҵ���ѡ��//����ÿ����ǣ������һ�����Ʊ�ǵ�ƽ��������...
	for (size_t i = 0; i<contours.size(); i++)
	{
		/*����һ������αƽ���Ϊ�˼������������ء������ȽϺã���ɸѡ���Ǳ��������Ϊ������ܱ��ĸ�����Ķ���α�ʾ���������εĶ�����ڻ������ĸ����;��Բ��Ǳ���Ŀ��Ҫ�ı�ǡ�ͨ���㼯���ƶ���Σ�����������Ϊepsilon������Ƴ̶ȣ���ԭʼ���������ƶ����֮��ľ��룬���ĸ�������ʾ������Ǳպϵġ�*/
		double eps = contours[i].size()*0.05;
		//����ͼ���2ά�㼯�������������ƾ��ȣ��Ƿ�պϡ��������εĶ�����ɵĵ㼯//ʹ����α�Եƽ�����õ����ƵĶ���� 
		approxPolyDP(contours[i], approxCurve, eps, true);

		//���Ǹ���Ȥ�Ķ����ֻ���ĸ�����
		if (approxCurve.size() != 4)
			continue;

		//��������Ƿ���͹����
		if (!isContourConvex(approxCurve))
			continue;

		//ȷ��������֮��ľ������㹻��ġ�//ȷ�����ڵ������ľ��롰�㹻�󡱣�����һ���߶����Ƕ��߶ξ�����
		//float minDist = numeric_limits<float>::max();//����float���Ա�ʾ�����ֵ��numeric_limits����ģ���࣬�����ʾmax��float��;3.4e038
		float minDist = 1e10;//���ֵ�ͺܴ���

							 //��ǰ�ı��θ�����֮�����̾���
		for (int i = 0; i<4; i++)
		{
			Point side = approxCurve[i] - approxCurve[(i + 1) % 4];//����Ӧ����2ά�����
			float squaredSideLength = side.dot(side);//��2ά�����ĵ��������XxY
			minDist = min(minDist, squaredSideLength);//�ҳ���С�ľ���
		}

		//�������ǲ����ر�С��С�Ļ����˳�����ѭ������ʼ��һ��ѭ��
		if (minDist<m_minContourLengthAllowed)
			continue;

		//���еĲ���ͨ���ˣ������ʶ��ѡ�����ı��δ�С���ʣ��򽫸��ı���maker����possibleMarkers������ //�������Ƶı��   
		Marker m;
		for (int i = 0; i<4; i++)
			m.points.push_back(Point2f(approxCurve[i].x, approxCurve[i].y));//vectorͷ�ļ�����������push_back��������vector��������Ϊ��vectorβ������һ�����ݡ�

																			/*��ʱ�뱣����Щ��
																			//�Ӵ����Ʋ⣬marker�еĵ㼯�������������У�˳ʱ�����ʱ�룬����Ҫ��˳ʱ������иĳ���ʱ�룬�ڶ���αƽ�ʱ��������Ǳպϵģ�����˳ʱ�������ʱ��
																			//�ڵ�һ���͵ڶ�����֮����ٳ�һ���ߣ���������������ұߣ��������ʱ�뱣���//��ʱ��������Щ��,��һ����͵ڶ�����֮����һ����,������������ڱߣ���ô��Щ�������ʱ��*/
		Point v1 = m.points[1] - m.points[0];
		Point v2 = m.points[2] - m.points[0];

		/*����ʽ�ļ���������ʲô�أ����������ͣ�һ������������ʽ��������ʽ�е��л������������ɵĳ�ƽ�ж��������������������������һ�������Ǿ���A������ʽdetA�������Ա任A�µ�ͼ�������������������ӡ�
		//��������a=(a1,a2)��b=(b1,b2)Ϊ�ڱߵ�ƽ���ı��ε���������������ƽ���ı���������������ʱ�뷽��ת��b���õ��ģ����ȡ��ֵ�������ƽ���ı�����������a��˳ʱ�뷽��ת�����õ��ģ����ȡ��ֵ�� */
		double o = (v1.x * v2.y) - (v1.y * v2.x);

		if (o<0.0) //���������������ߣ���ô������һ����͵������㣬��ʱ�뱣��
			swap(m.points[1], m.points[3]);

		possibleMarkers.push_back(m);//�������ʶ�����ѡ��ʶ������
	}

	//�Ƴ���Щ�ǵ㻥�����̫�����ı���//�Ƴ��ǵ�̫�ӽ���Ԫ��  
	vector< pair<int, int> > tooNearCandidates;
	for (size_t i = 0; i<possibleMarkers.size(); i++)
	{
		const Marker& m1 = possibleMarkers[i];
		//��������maker�ı���֮��ľ��룬�����֮�����͵�ƽ��ֵ����ƽ��ֵ��С������Ϊ����maker�����,����һ���ı��η����Ƴ����С�//����ÿ���߽ǵ��������ܱ�ǵ�����߽ǵ�ƽ������
		for (size_t j = i + 1; j<possibleMarkers.size(); j++)
		{
			const Marker& m2 = possibleMarkers[j];
			float distSquared = 0;
			for (int c = 0; c<4; c++)
			{
				Point v = m1.points[c] - m2.points[c];
				//�����ĵ�ˣ�������ľ���
				distSquared += v.dot(v);
			}
			distSquared /= 4;

			if (distSquared < 100)
			{
				tooNearCandidates.push_back(pair<int, int>(i, j));
			}
		}
	}

	//�Ƴ������ڵ�Ԫ�ضԵı�ʶ
	//����������������marker�ڲ����ĸ���ľ���ͣ�������ͽ�С�ģ���removlaMask������ǣ�������Ϊ���յ�detectedMarkers 
	vector<bool> removalMask(possibleMarkers.size(), false);//����Vector���󣬲�������������һ���������������ڶ�����Ԫ�ء�

	for (size_t i = 0; i<tooNearCandidates.size(); i++)
	{
		//����һ�������ı��ε��ܳ�
		float p1 = perimeter(possibleMarkers[tooNearCandidates[i].first].points);
		float p2 = perimeter(possibleMarkers[tooNearCandidates[i].second].points);

		//˭�ܳ�С���Ƴ�˭
		size_t removalIndex;
		if (p1 > p2)
			removalIndex = tooNearCandidates[i].second;
		else
			removalIndex = tooNearCandidates[i].first;

		removalMask[removalIndex] = true;
	}

	//���غ�ѡ���Ƴ������ı������ܳ���С���Ǹ�������������ı��εĶ����С�//���ؿ��ܵĶ���
	detectedMarkers.clear();
	for (size_t i = 0; i<possibleMarkers.size(); i++)
	{
		if (!removalMask[i])
			detectedMarkers.push_back(possibleMarkers[i]);
	}

}

void MarkerDetector::recognizeMarkers(const Mat& grayscale, vector<Marker>& detectedMarkers)
{
	Mat canonicalMarkerImage;
	char name[20] = "";

	vector<Marker> goodMarkers;

	/*Identify the markersʶ���ʶ //����ÿһ�����񵽵ı�ǣ�ȥ��͸��ͶӰ���õ�ƽ�棯����ľ��Ρ�
	//Ϊ�˵õ���Щ���εı��ͼ�����ǲ��ò�ʹ��͸�ӱ任ȥ�ָ�(unwarp)�����ͼ���������Ӧ��ʹ��cv::getPerspectiveTransform�����������ȸ����ĸ���Ӧ�ĵ��ҵ�͸�ӱ任����һ�������Ǳ�ǵ����꣬�ڶ����������α��ͼ������ꡣ����ı任����ѱ��ת���ɷ��Σ��Ӷ��������Ƿ����� */
	for (size_t i = 0; i<detectedMarkers.size(); i++)
	{
		Marker& marker = detectedMarkers[i];
		//�ҵ�͸��ת�����󣬻�þ��������������ͼ// �ҵ�͸��ͶӰ�����ѱ��ת���ɾ��Σ�����ͼ���ı��ζ������꣬���ͼ�����Ӧ���ı��ζ������� 
		// Find the perspective transformation that brings current marker to rectangular form
		Mat markerTransform = getPerspectiveTransform(marker.points, m_markerCorners2d);//����ԭʼͼ��ͱ任֮���ͼ��Ķ�Ӧ4���㣬����Եõ��任����
																						/* Transform image to get a canonical marker image
																						// Transform image to get a canonical marker image
																						//�����ͼ��
																						//�����ͼ��
																						//3x3�任���� */
		warpPerspective(grayscale, canonicalMarkerImage, markerTransform, markerSize);//��ͼ�����͸�ӱ任,��͵õ��ͱ�ʶͼ��һ�������ͼ�񣬷�����ܲ�ͬ�����ĸ���������е��ˡ��о�����任�󣬾͵õ�ֻ�б�ʶͼ������ͼ

																					  // sprintf(name,"warp_%d.jpg",i);
																					  // imwrite(name,canonicalMarkerImage);
#ifdef SHOW_DEBUG_IMAGES
		{
			Mat markerImage = grayscale.clone();
			marker.drawContour(markerImage);
			Mat markerSubImage = markerImage(boundingRect(marker.points));

			imshow("Source marker" + ToString(i), markerSubImage);
			imwrite("Source marker" + ToString(i) + ".png", markerSubImage);

			imshow("Marker " + ToString(i), canonicalMarkerImage);
			imwrite("Marker " + ToString(i) + ".png", canonicalMarkerImage);
		}
#endif

		int nRotations;
		int id = Marker::getMarkerId(canonicalMarkerImage, nRotations);
		//cout << "ID: " << id << endl;

		if (id != -1)
		{
			marker.id = id;
			//sort the points so that they are always in the same order no matter the camera orientation  
			//Rotates the order of the elements in the range [first,last), in such a way that the element pointed by middle becomes the new first element.
			//�����������ת��������ǵ���̬
			rotate(marker.points.begin(), marker.points.begin() + 4 - nRotations, marker.points.end());//����һ��ѭ����λ

			goodMarkers.push_back(marker);
		}
	}

	//refine using subpixel accuracy the corners  �ǰ����б�ʶ���ĸ����㶼����һ����������С�
	if (goodMarkers.size() > 0)
	{
		//�ҵ����б�ǵĽǵ� 
		vector<Point2f> preciseCorners(4 * goodMarkers.size());//ÿ��marker�ĸ���
		for (size_t i = 0; i<goodMarkers.size(); i++)
		{
			Marker& marker = goodMarkers[i];

			for (int c = 0; c<4; c++)
			{
				preciseCorners[i * 4 + c] = marker.points[c];//i��ʾ�ڼ���marker��c��ʾĳ��marker�ĵڼ�����
			}
		}

		//Refines the corner locations.The function iterates to find the sub-pixel accurate location of corners or radial saddle points
		//����  
		/*
		CV_TERMCRIT_ITER ��������������Ϊ��ֹ����
		CV_TERMCRIT_EPS �þ�����Ϊ��������
		CV_TERMCRIT_ITER+CV_TERMCRIT_EPS ���������������߾�����Ϊ�����������������ĸ�����������
		//������������
		//�ض��ķ�ֵ */
		TermCriteria termCriteria = TermCriteria(TermCriteria::MAX_ITER | TermCriteria::EPS, 30, 0.01);//����ǵ�����ֹ�����������Ǵﵽ30�ε������ߴﵽ0.01������ֹ���ǵ㾫׼���������̵���ֹ����
																									   /*����ͼ��
																									   //����Ľǵ㣬Ҳ��Ϊ�������ȷ�Ľǵ�
																									   //�ӽ��Ĵ�С��Neighborhood size��
																									   //Aperture parameter for the Sobel() operator
																									   //���ص��������ţ��ķ��� */
		cornerSubPix(grayscale, preciseCorners, cvSize(5, 5), cvSize(-1, -1), termCriteria);//���������ؾ��ȵĽǵ�λ�ã��ڶ���������������Ľǵ�ĳ�ʼλ�ò������׼�������ꡣ�ڱ�Ǽ������ڵĽ׶�û��ʹ��cornerSubPix��������Ϊ���ĸ����ԣ�����������������������ʱ��ķѴ����Ĵ���ʱ�䣬�������ֻ�ڴ�����Ч���ʱʹ�á�

																							//copy back���ٰѾ�׼�������괫��ÿһ����ʶ��// �������µĶ���
		for (size_t i = 0; i<goodMarkers.size(); i++)
		{
			Marker& marker = goodMarkers[i];
			for (int c = 0; c<4; c++)
			{
				marker.points[c] = preciseCorners[i * 4 + c];
				//cout<<"X:"<<marker.points[c].x<<"Y:"<<marker.points[c].y<<endl;
			}
		}

	}

	//����ϸ����ľ���ͼƬ
	Mat markerCornersMat(grayscale.size(), grayscale.type());
	markerCornersMat = Scalar(0);
	for (size_t i = 0; i<goodMarkers.size(); i++)
	{
		goodMarkers[i].drawContour(markerCornersMat, Scalar(255));
	}

	//imshow("Markers refined edges",grayscale*0.5 + markerCornersMat);
	//imwrite("Markers refined edges" + ".png",grayscale*0.5 + markerCornersMat);   
	//imwrite("refine.jpg", grayscale*0.5 + markerCornersMat);

	detectedMarkers = goodMarkers;

}

//��ʱ�����Ѿ�֪�����ȥ����ͼ���ϵı�ǲ����������ڿռ�������������׼ȷλ�á���ʱ��ȥ���㶫���ˡ�����ղ�˵�ģ����ǽ���ʹ��opengl������Ⱦ������3ά���⻯����ǿ��ʵ���ĵĲ��֣�opengl�ṩ�����л���������ȥ��������������Ⱦ��

bool MarkerDetector::findMarkers(const Mat& frame, vector<Marker>& detectedMarkers)
{
	//MessageBox(NULL, L"Finding Markers", L"MarkerDetector", MB_OK);
	prepareImage(frame, m_grayscaleImage);
	//MessageBox(NULL, L"prepareImage completed", L"MarkerDetector", MB_OK);

	performThreshold(m_grayscaleImage, m_thresholdImg);
	//MessageBox(NULL, L"performThreshold completed", L"MarkerDetector", MB_OK);

	findContour(m_thresholdImg, m_contours, m_grayscaleImage.cols / 5);
	//MessageBox(NULL, L"findContour completed", L"MarkerDetector", MB_OK);

	findCandidates(m_contours, detectedMarkers);
	//MessageBox(NULL, L"findCandidates completed", L"MarkerDetector", MB_OK);

	recognizeMarkers(m_grayscaleImage, detectedMarkers);
	//MessageBox(NULL, L"recognizeMarkers completed", L"MarkerDetector", MB_OK);


	return false;
}
