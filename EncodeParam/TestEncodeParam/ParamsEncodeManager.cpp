#include "ParamsEncodeManager.h"

#include <iostream> 
#include <stdlib.h> 
#include <time.h>  
#include <mutex>

namespace param_encode
{
	static std::vector<char*> buffer;
	static int freeBufferCount = 0;
	static std::mutex g_lock;
	static float Mark[] = { 
		-299.2f,
		26.3f,
		-150.6f,
		3.5f,
		277.4f,
		-113.8f,
		-143.9f,
		555.2f,
		777.7f };

	static unsigned short MarkShort[] = {
		0xFEFF,
		0x9FFF,
		0x11FF,
		0x73FF,
		0xf3FF,
		0xfaFF,
		0x5FF,
		0xf7FF,
		0x45FF };


	static short curPosition = 0;
	static int MaskCount = 9;

	union FLOAT_BYTE
	{
		char  _char[4];
		float _float;
	};

	union BYTE_SHORT
	{
		unsigned short  _short;
		unsigned short  _char[2];
	};
	void Update()
	{
		curPosition++;
		curPosition %= MaskCount;
	}
	void RequestBuffer()
	{
		srand((unsigned)time(NULL));
		buffer.push_back(new char);
		freeBufferCount++;
		for (int i = 1; i < 400; i++)
		{
			buffer.insert(buffer.begin() + (rand() % i), new char);
			freeBufferCount++;
		}
	}

	EFLoat& EFLoat::operator=(const EFLoat& v)
	{
		id = v.id;
		for (int i = 0; i < 4; i++)
		{
			*data[i] = *v.data[i] ;
		}
		return *this;
	}
	EFLoat& EFLoat::operator=(const float& v)
	{
		id = curPosition;
		//id++;
		//id %= MaskCount;
		FLOAT_BYTE _data;
		_data._float = Mark[id]-v;
		for (int i = 0; i < 4; i++)
		{
			*data[i] = _data._char[i]  ;
		}
		return *this;
	}
	EFLoat& EFLoat::operator=(const int& v)
	{
		id = curPosition;
		//id++;
		//id %= MaskCount;
		FLOAT_BYTE _data;
		_data._float = Mark[id] - v;
		for (int i = 0; i < 4; i++)
		{
			*data[i] = _data._char[i]  ;
		}

		return *this;
	}
	EFLoat::EFLoat()
	{
		g_lock.lock();
		if (freeBufferCount < 4)
		{
			RequestBuffer();
		}
		for (int i = 0; i < 4; i++)
		{
			char *p = buffer[freeBufferCount - 1];
			freeBufferCount--;
			buffer.pop_back();
			data[i] = p;
		}
		g_lock.unlock();


		id = curPosition;
		//id++;
		//id %= MaskCount;
		FLOAT_BYTE _data;
		_data._float = Mark[id] ;
		for (int i = 0; i < 4; i++)
		{
			*data[i] = _data._char[i]  ;
		}
	}
	EFLoat::EFLoat(float v)
	{
		g_lock.lock();
		if (freeBufferCount < 4)
		{
			RequestBuffer();
		}
		for (int i = 0; i < 4; i++)
		{
			char *p = buffer[freeBufferCount - 1];
			freeBufferCount--;
			buffer.pop_back();
			data[i] = p;
		}
		g_lock.unlock();

		id = curPosition;
		//id++;
		//id %= MaskCount;
		FLOAT_BYTE _data;
		_data._float = Mark[id] - v;
		for (int i = 0; i < 4; i++)
		{
			*data[i] = _data._char[i] ;
		}
	}
	EFLoat::~EFLoat()
	{
		g_lock.lock();
		for (int i = 0; i < 4; i++)
		{
			buffer.push_back(data[i]);
			freeBufferCount++;
		}
		g_lock.unlock();
	}
	EFLoat::operator float() const
	{
		FLOAT_BYTE _data;
		for (int i = 0; i < 4; i++)
		{
			_data._char[i] = *data[i] ;
		}
		return Mark[id] - _data._float;
	}
	float EFLoat::getFloat() const
	{

		FLOAT_BYTE _data;
		for (int i = 0; i < 4; i++)
		{
			_data._char[i] = *data[i]  ;
		}
		return Mark[id] - _data._float;
	}
	void EFLoat::setFloat(float f)
	{
		id = curPosition;
		//id++;
		//id %= MaskCount;
		FLOAT_BYTE _data;
		_data._float = Mark[id] - f;
		for (int i = 0; i < 4; i++)
		{
			*data[i] = _data._char[i] ;
		}
	}
	 
	// byte

	EByte::EByte()
	{
		g_lock.lock();
		if (freeBufferCount < 2)
		{
			RequestBuffer();
		}
		for (int i = 0; i < 2; i++)
		{
			char *p = buffer[freeBufferCount - 1];
			freeBufferCount--;
			buffer.pop_back();
			data[i] = (unsigned short*)p;
		}
		g_lock.unlock();
		id = curPosition;
		BYTE_SHORT _data;
		_data._short = MarkShort[id];
		for (int i = 0; i < 2; i++)
		{
			*data[i] = _data._char[i];
		}
	}
	EByte::EByte(e_byte v)
	{
		g_lock.lock();
		if (freeBufferCount < 2)
		{
			RequestBuffer();
		}
		for (int i = 0; i < 2; i++)
		{
			char *p = buffer[freeBufferCount - 1];
			freeBufferCount--;
			buffer.pop_back();
			data[i] = (unsigned short*)p;
		}
		g_lock.unlock();
		id = curPosition;
		BYTE_SHORT _data;
		_data._short = MarkShort[id] - v;
		for (int i = 0; i < 2; i++)
		{
			*data[i] = _data._char[i];
		}
	}
	EByte::~EByte()
	{
		g_lock.lock();
		for (int i = 0; i < 2; i++)
		{
			buffer.push_back((char*)data[i]);
			freeBufferCount++;
		}
		g_lock.unlock();
	}
	EByte::operator e_byte() const
	{
		BYTE_SHORT _data;
		for (int i = 0; i < 2; i++)
		{
			_data._char[i] = *data[i];
		}
		return (e_byte)(MarkShort[id] - _data._short);
	}
	e_byte EByte::getByte() const
	{
		BYTE_SHORT _data;
		for (int i = 0; i < 2; i++)
		{
			_data._char[i] = *data[i];
		}
		return (e_byte)(MarkShort[id] - _data._short);
	}
	void EByte::setByte(e_byte f)
	{
		id = curPosition;
		BYTE_SHORT _data;
		_data._short = MarkShort[id] - f;
		for (int i = 0; i < 2; i++)
		{
			*data[i] = _data._char[i];
		}
	}
	EByte& EByte::operator=(const EByte& v)
	{
		id = v.id;
		for (int i = 0; i < 2; i++)
		{
			*data[i] = *v.data[i];
		}
		return *this;
	}
	EByte& EByte::operator=(const e_byte& v)
	{
		id = curPosition;
		BYTE_SHORT _data;
		_data._short = MarkShort[id] - v;
		for (int i = 0; i < 2; i++)
		{
			*data[i] = _data._char[i];
		}
		return *this;
	}

	 
	
}
