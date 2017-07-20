#ifndef GEOMETRYTYPES
#define GEOMETRYTYPES

#include <string>
#include <sstream>
#include <vector>

struct Matrix44
{
	union//������ͽṹ��ʮ�����ƣ�������������һ���ڴ�λ�ã��ڲ�ͬ��ʱ�䱣�治ͬ���������ͺͲ�ͬ���ȵı�����ͬһʱ��ֻ�ܴ�������һ����Ա������ֵ���䳤��Ϊ���������ı������ȵ���������
	{
		float data[16];
		float mat[4][4];
	};

	Matrix44 getTransposed() const;//�Խ����໥����
	Matrix44 getInvertedRT() const;//ȡ��ת��
	static Matrix44 identity();//�õ���λ����
};

struct Matrix33
{
	union
	{
		float data[9];
		float mat[3][3];
	};

	static Matrix33 identity();//�õ���λ����
	Matrix33 getTransposed() const;//�Խ��߻���
};

struct Vector4
{
	float data[4];
};

struct Vector3
{
	float data[3];

	static Vector3 zero();//������
	Vector3 operator-() const;//ȡ��
};

struct Transformation
{
	Transformation();
	Transformation(const Matrix33& r, const Vector3& t);

	Matrix33& r();
	Vector3&  t();

	const Matrix33& r() const;
	const Vector3&  t() const;

	Matrix44 getMat44() const;

	Transformation getInverted() const;

	std::string getMat44string() const;


	std::vector<float> getMat44vector() const;

private:
	Matrix33 m_rotation;
	Vector3  m_translation;
};

#endif