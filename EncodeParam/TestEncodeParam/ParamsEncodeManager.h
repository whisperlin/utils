#pragma once
#include <string>
#include <vector>
namespace param_encode
{

	typedef unsigned char e_byte;
	void Update();
	void RequestBuffer();
	class EFLoat
	{
	public:
		EFLoat();
		EFLoat(float v);
		virtual ~EFLoat();

		float getFloat() const;
		void setFloat(float f);

		EFLoat& operator=(const EFLoat&);
		EFLoat& operator=(const float&);
		EFLoat& operator=(const int&);
		operator float() const;
	private:
		short id = 0;

		char * data[4];
	};

	class EByte
	{
	public:
		EByte();
		EByte(e_byte v);
		virtual ~EByte();
		e_byte getByte() const;
		void setByte(e_byte f);
		EByte& operator=(const EByte&);
		EByte& operator=(const e_byte&);
		operator e_byte() const;
	private:
		short id = 0;
		unsigned short * data[2];
	};


	//byte mCount = 0
};
