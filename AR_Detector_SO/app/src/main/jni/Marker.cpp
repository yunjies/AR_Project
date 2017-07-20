
#include "Marker.hpp"

using namespace cv;
using namespace std;

Marker::Marker()
	:id(-1)
{
}

bool operator<(const Marker& M1, const Marker& M2)
{
	return M1.id<M2.id;
}

Mat Marker::rotate(Mat in)//���ǰѾ�����ת90��
{
	Mat out;
	in.copyTo(out);
	for (int i = 0; i<in.rows; i++)
	{
		for (int j = 0; j<in.cols; j++)
		{
			out.at<uchar>(i, j) = in.at<uchar>(in.cols - j - 1, i);//at<uchar>����ָ��ĳ��λ�õ����أ�ͬʱָ���������͡����ǽ���Ԫ�أ���ô�����ģ�
		}
	}
	return out;
}

//����Ϣ���У������ȳ��ַ���֮��ĺ��������������ַ�����Ӧλ�õ��ַ���ͬ�ĸ��������仰˵�������ǽ�һ���ַ����任������һ���ַ�������Ҫ�滻���ַ�������  
int Marker::hammDistMarker(Mat bits)//��ÿ�����ܵı�Ƿ����ҵ��������룬�Ͳο���ʶһ�µ�Ϊ0,������ת��ʽ�ı�ǲ�Ϊ0,��Ϊ����͸��任��ֻ�ܵõ��ĸ�����ı�ǣ�����ת�ĴΣ��ҵ��Ͳο���ʶһ�µķ���
{
	int ids[4][5] =
	{
		{ 1,0,0,0,0 },
		{ 1,0,1,1,1 },
		{ 0,1,0,0,1 },
		{ 0,1,1,1,0 }
	};

	int dist = 0;

	for (int y = 0; y<5; y++)
	{
		int minSum = 1e5;//ÿ��Ԫ�صĺ�������

		for (int p = 0; p<4; p++)
		{
			int sum = 0;
			//now,count
			for (int x = 0; x<5; x++)
			{
				sum += bits.at<uchar>(y, x) == ids[p][x] ? 0 : 1;
			}
			if (minSum>sum)
				minSum = sum;
		}

		dist += minSum;
	}

	return dist;
}

int Marker::mat2id(const Mat& bits)//��λ���������λ���õ����յ�ID
{
	int val = 0;
	for (int y = 0; y<5; y++)
	{
		val <<= 1;//��λ����
		if (bits.at<uchar>(y, 1)) val |= 1;
		val <<= 1;
		if (bits.at<uchar>(y, 3)) val |= 1;
	}
	return val;
}

int Marker::getMarkerId(Mat& markerImage, int &nRotations)
{
	assert(markerImage.rows == markerImage.cols);//��������������ش�������ֹ����ִ��
	assert(markerImage.type() == CV_8UC1);

	Mat grey = markerImage;

	//Threshold imageʹ��Otsu�㷨�Ƴ���ɫ�����أ�ֻ���º�ɫ�Ͱ�ɫ���ء�
	//���ǹ̶���ֵ����  
	//����ͼ��image����Ϊһ��2ֵ��ͨ��ͼ��  
	//�����������飬ÿһ��������һ��point���͵�vector��ʾ  
	//��ֵ  
	//max_value ʹ�� CV_THRESH_BINARY �� CV_THRESH_BINARY_INV �����ֵ  
	//type 
	threshold(grey, grey, 125, 255, THRESH_BINARY | THRESH_OTSU);//�Ժ�ѡ�������ĻҶ�ͼʹ�ô���OSTU�㷨����ȡ��ֵ��ͼ����ΧͼƬ������㷨��Ӱ�����ܡ�

#ifdef SHOW_DEBUG_IMAGES
	imshow("Binary marker", grey);
	imwrite("Binary marker" + ".png", grey);
#endif
	//imshow("Binary marker", grey);

	//��ʹ�õı�Ƕ���һ���ڲ���5x5���룬���õ��Ǽ��޸ĵĺ����롣�򵥵�˵������5bits��ֻ��2bits��ʹ�ã�������λ���Ǵ����ʶ���룬Ҳ����˵����������1024�ֲ�ͬ�ı�ʶ�����ǵĺ��������Ĳ�ͬ�ǣ�������ĵ�һλ����żУ��λ��3��5)�Ƿ���ġ�����ID 0(�ں�������00000)����������10000��Ŀ���Ǽ��ٻ�����ɵ�Ӱ��.
	//��ʶ������Ϊ7x7�������ڲ���5x5��ʾ��ʶ���ݣ�������Ǻ�ɫ�߽磬�������������������ߵ������Ƿ��Ǻ�ɫ�ģ����в��Ǻ�ɫ����ô�Ͳ��Ǳ�ʶ��
	int cellSize = markerImage.rows / 7;

	for (int y = 0; y<7; y++)
	{
		int inc = 6;

		if (y == 0 || y == 6) inc = 1;//�Ե�һ�к����һ�У���������߽�

		for (int x = 0; x<7; x += inc)
		{
			int cellX = x*cellSize;
			int cellY = y*cellSize;
			Mat cell = grey(Rect(cellX, cellY, cellSize, cellSize));

			int nZ = countNonZero(cell);//ͳ�������ڷ�0�ĸ�����

			if (nZ >(cellSize*cellSize) / 2)
			{
				return -1;//����߽���Ϣ���Ǻ�ɫ�ģ��Ͳ���һ����ʶ��
			}
		}
	}

	Mat bitMatrix = Mat::zeros(5, 5, CV_8UC1);

	//�õ���Ϣ�������ڲ������񣬾����Ƿ��Ǻ�ɫ���ɫ�ģ������ж��ڲ�5x5��������ʲô��ɫ�ģ��õ�һ��������Ϣ�ľ���bitMatrix��
	for (int y = 0; y<5; y++)
	{
		for (int x = 0; x<5; x++)
		{
			int cellX = (x + 1)*cellSize;
			int cellY = (y + 1)*cellSize;
			Mat cell = grey(Rect(cellX, cellY, cellSize, cellSize));

			int nZ = countNonZero(cell);
			if (nZ >(cellSize*cellSize) / 2)
				bitMatrix.at<uchar>(y, x) = 1;
		}
	}

	//������е���ת
	Mat rotations[4];
	int distances[4];

	rotations[0] = bitMatrix;
	distances[0] = hammDistMarker(rotations[0]);//��û����ת�ľ���ĺ������롣

	pair<int, int> minDist(distances[0], 0);//����õĺ����������ת�Ƕ���Ϊ��С��ʼֵ�ԣ�ÿ��pair������������ֵfirst��second

	for (int i = 1; i<4; i++)//�����ж����������ο�������ת���ٶȡ�
	{
		//��������Ŀ���Ԫ�صĺ�������
		rotations[i] = rotate(rotations[i - 1]);//ÿ����ת90��
		distances[i] = hammDistMarker(rotations[i]);

		if (distances[i] < minDist.first)
		{
			minDist.first = distances[i];
			minDist.second = i;//���pair�ĵڶ���ֵ�Ǵ�����ת���Σ�ÿ��90�ȡ�
		}
	}

	nRotations = minDist.second;//����ǽ����ص���ת�Ƕ�ֵ
	if (minDist.first == 0)//����������Ϊ0,����������ת��ľ�������ʶID
	{
		return mat2id(rotations[minDist.second]);
	}

	return -1;
}

void Marker::drawContour(Mat& image, Scalar color) const//��ͼ���ϻ��ߣ�����������
{
	float thickness = 2;

	line(image, points[0], points[1], color, thickness, CV_AA);
	line(image, points[1], points[2], color, thickness, CV_AA);
	line(image, points[2], points[3], color, thickness, CV_AA);//thickness�߿�
	line(image, points[3], points[0], color, thickness, CV_AA);//CV_AA�ǿ����
}
