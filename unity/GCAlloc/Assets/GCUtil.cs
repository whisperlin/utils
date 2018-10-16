using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Engine.IntUtil;

public struct IntComparer : IEqualityComparer<int>
{
	public bool Equals(int x, int y)
	{
		return x == y;
	}

	public int GetHashCode(int obj)
	{
		// you need to do some thinking here,
		return obj;
	}
}

public struct StringComparer : IEqualityComparer<string>
{
	public bool Equals(string x, string y)
	{
		return x.CompareTo( y)==0;
	}

	public int GetHashCode(string obj)
	{
		// you need to do some thinking here,
		return obj.GetHashCode();
	}
}
public struct Vector3Comparer : IEqualityComparer<IntVector3>
{
	public bool Equals(IntVector3 x, IntVector3 y)
	{
		return x==y;
	}

	public int GetHashCode(IntVector3 obj)
	{
		// you need to do some thinking here,
		return obj.GetHashCode();
	}
}

public class GCUtil  {
	public static IntComparer int_cmp = new IntComparer();
	public static StringComparer str_cmp = new StringComparer();
	public static Vector3Comparer vec3_cmp = new Vector3Comparer();
}
