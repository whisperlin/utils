
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using yw;
public static class GeometryUtilityUser
{
 
	enum EPlaneSide
	{
		Left,
		Right,
		Bottom,
		Top,
		Near,
		Far
	}

	static float[] RootVector = new float[4];
	static float[] ComVector = new float[4];

	public static void CalculateFrustumPlanes(Camera InCamera, ref Plane[] OutPlanes)
	{
		Matrix4x4 projectionMatrix = InCamera.projectionMatrix;
		Matrix4x4 worldToCameraMatrix = InCamera.worldToCameraMatrix;
		Matrix4x4 worldToProjectionMatrix = projectionMatrix * worldToCameraMatrix;

		RootVector[0] = worldToProjectionMatrix[3, 0];
		RootVector[1] = worldToProjectionMatrix[3, 1];
		RootVector[2] = worldToProjectionMatrix[3, 2];
		RootVector[3] = worldToProjectionMatrix[3, 3];

		ComVector[0] = worldToProjectionMatrix[0, 0];
		ComVector[1] = worldToProjectionMatrix[0, 1];
		ComVector[2] = worldToProjectionMatrix[0, 2];
		ComVector[3] = worldToProjectionMatrix[0, 3];

		CalcPlane(ref OutPlanes[(int)EPlaneSide.Left], ComVector[0] + RootVector[0], ComVector[1] + RootVector[1], ComVector[2] + RootVector[2], ComVector[3] + RootVector[3]);
		CalcPlane(ref OutPlanes[(int)EPlaneSide.Right], -ComVector[0] + RootVector[0], -ComVector[1] + RootVector[1], -ComVector[2] + RootVector[2], -ComVector[3] + RootVector[3]);

		ComVector[0] = worldToProjectionMatrix[1, 0];
		ComVector[1] = worldToProjectionMatrix[1, 1];
		ComVector[2] = worldToProjectionMatrix[1, 2];
		ComVector[3] = worldToProjectionMatrix[1, 3];

		CalcPlane(ref OutPlanes[(int)EPlaneSide.Bottom], ComVector[0] + RootVector[0], ComVector[1] + RootVector[1], ComVector[2] + RootVector[2], ComVector[3] + RootVector[3]);
		CalcPlane(ref OutPlanes[(int)EPlaneSide.Top], -ComVector[0] + RootVector[0], -ComVector[1] + RootVector[1], -ComVector[2] + RootVector[2], -ComVector[3] + RootVector[3]);

		ComVector[0] = worldToProjectionMatrix[2, 0];
		ComVector[1] = worldToProjectionMatrix[2, 1];
		ComVector[2] = worldToProjectionMatrix[2, 2];
		ComVector[3] = worldToProjectionMatrix[2, 3];

		CalcPlane(ref OutPlanes[(int)EPlaneSide.Near], ComVector[0] + RootVector[0], ComVector[1] + RootVector[1], ComVector[2] + RootVector[2], ComVector[3] + RootVector[3]);
		CalcPlane(ref OutPlanes[(int)EPlaneSide.Far], -ComVector[0] + RootVector[0], -ComVector[1] + RootVector[1], -ComVector[2] + RootVector[2], -ComVector[3] + RootVector[3]);

	}

	static void CalcPlane(ref Plane InPlane, float InA, float InB, float InC, float InDistance)
	{
		Vector3 Normal = new Vector3(InA, InB, InC);

		float InverseMagnitude = 1.0f / (float)System.Math.Sqrt(Normal.x * Normal.x + Normal.y * Normal.y + Normal.z * Normal.z);

		InPlane.normal = new Vector3(Normal.x * InverseMagnitude, Normal.y * InverseMagnitude, Normal.z * InverseMagnitude);

		InPlane.distance = InDistance * InverseMagnitude;
	}
}