using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MaterialComparer : IEqualityComparer<Material>
{
	public bool Equals(Material x, Material y)
	{
		return x == y;
	}
	public int GetHashCode(Material obj)
	{
		return obj.GetHashCode();
	}
}
