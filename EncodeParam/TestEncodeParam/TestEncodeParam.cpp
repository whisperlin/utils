// TestEncodeParam.cpp : 定义控制台应用程序的入口点。
//

#include "stdafx.h"
#include <time.h>
#include "ParamsEncodeManager.h"

#include <memory>
struct TestStructEqual
{
	std::shared_ptr<param_encode::EFLoat> mValue = std::make_shared<param_encode::EFLoat>();
};

struct TestStructEqual2
{
	param_encode::EFLoat mValue;
};
int main()
{
	#include <time.h>

	clock_t start, finish;
	clock_t init0, init1;
	long elapsed_time;
	init0 = clock();
	param_encode::RequestBuffer();
	init1 = clock();
	
	printf("request buffer used %d ms\n", init1 - init0);
	start = clock();
	param_encode::Update();
	for (int i = 0; i < 100000; i++)
	{
		param_encode::EFLoat _v;	//create
		_v = i;						//write
		float _v2 = _v;				//read
		//printf("%f\n", _v2);
									//release
		
	}
	finish = clock();
	elapsed_time = finish - start;

	printf("elapsed_time = %d ms (%fs)\n", elapsed_time,((float)elapsed_time)/1000);
	

	/*TestStructEqual t1;
	
	{
		TestStructEqual t2;
		*t2.mValue = 7.44f;
		t1 = t2;
		*t2.mValue = 1.22f;
	}
	printf("final %f\n",t1.mValue->getFloat());
	*/


	TestStructEqual2 I1;

	{
		TestStructEqual2 I2;
		I2.mValue = 7.44f;
		I1 = I2;
		I2.mValue = 1.22f;
	}
	printf("final %f\n", I1.mValue.getFloat());

	start = clock();
	param_encode::Update();
	for (int i = 0; i < 1000; i++)
	{
		param_encode::EByte _v;	//create
		_v = unsigned short(i%0xff);						//write
		unsigned short _v2 = _v;				//read
									//printf("%d\n", _v2);
									//release
	}
	

	finish = clock();
	elapsed_time = finish - start;
	printf("elapsed_time = %d ms (%fs)\n", elapsed_time, ((float)elapsed_time) / 1000);

	const param_encode::EByte _vt;
	unsigned short _vt2 = _vt + 123;
	
	return 0;
}

