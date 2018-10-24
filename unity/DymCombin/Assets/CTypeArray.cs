using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTypeArray <T> {

	public T[] ary; 
	private int _count = 0;
	private int arrayCount = 256;
	public int Count
	{
		get
		{
			return _count;
		}
	}
 
	public   CTypeArray( )
	{
		ary = new T[256];
	}
	public void Add(T t)
	{
		Extend ();
		ary [_count] = t;
		_count++;
	}
	public void Delete(int i)
	{
		for (int j = i; j < _count - 1 ; j++) {
			ary [j] = ary [j + 1];
		}
		_count--;
	}
	public void Extend()
	{
		if (_count >= arrayCount) {
			//这玩意有空封装.
			T [] _t = new  T[arrayCount+256];
			for (int j = 0; j < arrayCount; j++) {
				_t [j] = ary [j];
			}
			ary = _t;
			arrayCount += 256;
		}
	}

	public T this[int idx]
	{
		get {
			return ary[idx];
		}
		set { 
			ary[idx] = value;
		}
	}

}
