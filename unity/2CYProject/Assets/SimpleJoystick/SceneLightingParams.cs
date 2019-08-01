using System;
using UnityEngine;

namespace EasyExtend.Scene
{
	// Token: 0x0200001D RID: 29
	public class SceneLightingParams : MonoBehaviour
	{
		// Token: 0x06000068 RID: 104 RVA: 0x000065BC File Offset: 0x000047BC
		public void SetupParams()
		{
			Shader.SetGlobalMatrix("envSHR", this.envSHR);
			Shader.SetGlobalMatrix("envSHG", this.envSHG);
			Shader.SetGlobalMatrix("envSHB", this.envSHB);
			Shader.SetGlobalMatrix("envRot", Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(this.envRotation), Vector3.one));
			Shader.SetGlobalVector("shadow_light_new", this.shadowLightParam);
			Vector3 vector = Quaternion.Euler(this.shadowLightDir) * Vector3.forward;
			vector.Normalize();
			Shader.SetGlobalVector("shadow_light_newdir", new Vector4(vector.x, vector.y, vector.z, 1f));
			Shader.SetGlobalVectorArray("ShadowLightAttr", this.shadowLightAttr);
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00006691 File Offset: 0x00004891
		public void Start()
		{
			this.SetupParams();
		}

		// Token: 0x0600006A RID: 106 RVA: 0x0000669C File Offset: 0x0000489C
		private static Matrix4x4 FA2M4x4(float[] data)
		{
			Matrix4x4 result = default(Matrix4x4);
			result[0] = 1f;
			result[5] = 1f;
			result[10] = 1f;
			result[15] = 1f;
			for (int i = 0; i < 16; i++)
			{
				result[i] = data[i];
			}
			return result;
		}

		// Token: 0x040000A9 RID: 169
		public Matrix4x4 envSHR = SceneLightingParams.FA2M4x4(new float[]
		{
			0.15342f,
			0.113952f,
			0.268239f,
			0.078191f,
			0.113952f,
			-0.15342f,
			0.0361716f,
			0.142133f,
			0.268239f,
			0.0361716f,
			0.124804f,
			-0.075747f,
			0.078191f,
			0.142133f,
			-0.075747f,
			0.639728f
		});

		// Token: 0x040000AA RID: 170
		public Matrix4x4 envSHG = SceneLightingParams.FA2M4x4(new float[]
		{
			0.130166f,
			0.126413f,
			0.230749f,
			0.12969f,
			0.126413f,
			-0.130166f,
			0.049633f,
			0.175574f,
			0.230749f,
			0.049633f,
			0.0774299f,
			-0.00149216f,
			0.12969f,
			0.175574f,
			-0.00149216f,
			0.642535f
		});

		// Token: 0x040000AB RID: 171
		public Matrix4x4 envSHB = SceneLightingParams.FA2M4x4(new float[]
		{
			0.106006f,
			0.146624f,
			0.207921f,
			0.188139f,
			0.146624f,
			-0.106006f,
			0.0741966f,
			0.27712f,
			0.207921f,
			0.0741966f,
			0.0410959f,
			0.0841471f,
			0.188139f,
			0.27712f,
			0.0841471f,
			0.807898f
		});

		// Token: 0x040000AC RID: 172
		public Vector3 envRotation = new Vector3(0f, 0f, 0f);

		// Token: 0x040000AD RID: 173
		public Vector3 shadowLightParam = new Vector4(1.8f, 1.665f, 1.573f, 1f);

		// Token: 0x040000AE RID: 174
		public Vector3 shadowLightDir = Quaternion.FromToRotation(Vector3.forward, new Vector3(0.995565f, 0f, 0.0940723f)).eulerAngles;

		// Token: 0x040000AF RID: 175
		public Vector4[] shadowLightAttr = new Vector4[]
		{
			new Vector4(0f, 0f, 0f, 0f),
			new Vector4(2.47059f, 2.8f, 3.5f, 3f),
			new Vector4(0f, 0f, 0f, 0f),
			new Vector4(0.644067f, -0.737462f, -0.20329f, 0.001f)
		};
	}
}
