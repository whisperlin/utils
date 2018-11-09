using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParamHelper : MonoBehaviour {
	public static void long2doubleInt(long a,out int a1,out int a2) {
		a1 = (int)(a & uint.MaxValue);
		a2 = (int)(a >> 32);
	}
	public static long doubleInt2long(int a1, int a2)
	{
		long b = a2;
		b = b << 32;
		b = b | (uint)a1;
		return b;
	}

}
