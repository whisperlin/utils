using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace yw
{
    public class CTypeArray <T> {

		const int NUM = 16;
    	public T[] ary; 
    	private int _count = 0;
    	private int arrayCount = 16;
    	public int Count
    	{
    		get
    		{
    			return _count;
    		}
    	}
		public void Clear( )
		{
			_count = 0;
			 
		}
    	public   CTypeArray( )
    	{
    		ary = new T[16];
    	}
    	public void Add(T t)
    	{
    		Extend ();
    		ary [_count] = t;
    		_count++;
    	}

		public int AddOne( )
		{
			Extend ();
			_count++;
			return _count-1;
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
    			T [] _t = new  T[arrayCount+16];
    			for (int j = 0; j < arrayCount; j++) {
    				_t [j] = ary [j];
    			}
    			ary = _t;
    			arrayCount += 16;
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
}
