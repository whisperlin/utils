using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000002 RID: 2
[CustomEditor(typeof(CompressEdit))]
public class CompressEdit : EditorWindow
{
	// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
	[MenuItem("LayaAir3D/Account", false, 0)]
	public static void initExport()
	{
		CompressEdit.layaWindow = (CompressEdit)EditorWindow.GetWindow(typeof(CompressEdit));
		WWW www = new WWW("file://" + Application.dataPath + "/LayaAir3D/LayaTool/layabox.png");
		CompressEdit.www = new WWW("file://" + Application.dataPath + "/LayaAir3D/LayaTool/logo.png");
		CompressEdit.www2 = new WWW("file://" + Application.dataPath + "/LayaAir3D/LayaTool/logo.png");
		CompressEdit.wwwVIPIcon = new WWW("file://" + Application.dataPath + "/LayaAir3D/LayaTool/VIPY.png");
		CompressEdit.VipY = CompressEdit.wwwVIPIcon.texture;
		CompressEdit.wwwVIPIcon = new WWW("file://" + Application.dataPath + "/LayaAir3D/LayaTool/VIPM.png");
		CompressEdit.VipM = CompressEdit.wwwVIPIcon.texture;
		CompressEdit.wwwVIPIcon = new WWW("file://" + Application.dataPath + "/LayaAir3D/LayaTool/recharge.png");
		CompressEdit.rechargeTexture = CompressEdit.wwwVIPIcon.texture;
		CompressEdit.AriUrlTexture = CompressEdit.www2.texture;
		CompressEdit.LoginTexture = CompressEdit.www.texture;
		GUIContent titleContent = new GUIContent("LayaAir3D", www.texture);
		CompressEdit.layaWindow.titleContent = titleContent;
		CompressEdit.fontsize.fontSize =  15;
		CompressEdit.fontsize.fontStyle = (FontStyle)1;
		CompressEdit.fontsize.alignment = (TextAnchor)4;
		CompressEdit.fontsize.normal.textColor = EditorStyles.label.normal.textColor;
		CompressEdit.fontsize2.normal.textColor = (EditorGUIUtility.isProSkin ? new Color(0f, 190f, 200f) : Color.blue);
		CompressEdit.fontsize3.fontSize = 13;
		CompressEdit.fontsize3.normal.textColor = EditorStyles.label.normal.textColor;
		CompressEdit.fontsize4.fontSize = 15;
		CompressEdit.fontsize4.fontStyle = (FontStyle)1;
		CompressEdit.fontsize4.normal.textColor = EditorStyles.label.normal.textColor;
		CompressEdit.fontsize5.fontSize = 15;
		CompressEdit.fontsize5.fontStyle = (FontStyle)1;
		CompressEdit.fontsize5.alignment = (TextAnchor)4;
		CompressEdit.fontsize5.normal.textColor = Color.white;
		if (CompressEdit.ga == null)
		{
			CompressEdit.ga = SceneManager.GetActiveScene().GetRootGameObjects()[0];
		}
		if (CompressEdit.ga.GetComponent<HTTPClient>() == null)
		{
			CompressEdit.httpClient = CompressEdit.ga.AddComponent<HTTPClient>();
		}
		else
		{
			CompressEdit.httpClient = CompressEdit.ga.GetComponent<HTTPClient>();
		}
		CompressEdit.LoginName = "";
		CompressEdit.PassWorldName = "";
	}

	// Token: 0x06000002 RID: 2 RVA: 0x000022FC File Offset: 0x000004FC
	private void OnGUI()
	{
		if (HTTPClient._b_AccountUsed != 2)
		{
			CompressEdit.page = 0;
		}
		else
		{
			CompressEdit.page = 2;
		}
		if (CompressEdit.page == 0)
		{
			EditorGUILayout.Space();
			if (CompressEdit.LoginTexture == null)
			{
				CompressEdit.www = new WWW("file://" + Application.dataPath + "/logo.png");
				CompressEdit.LoginTexture = CompressEdit.www.texture;
				CompressEdit.fontsize.fontSize = 15;
			}
			GUI.DrawTexture(new Rect(base.position.width / 2f - 64f, 70f, 128f, 128f), CompressEdit.LoginTexture, (UnityEngine.ScaleMode)2, true);
			if (HTTPClient._b_AccountUsed == 1)
			{
				GUIStyle guistyle = new GUIStyle();
				guistyle.normal.textColor = Color.red;
				GUI.Label(new Rect(base.position.width / 2f - 90f, 210f, 150f, 30f), "Account or password error", guistyle);
			}
			else if (HTTPClient._b_AccountUsed == 3)
			{
				GUIStyle guistyle2 = new GUIStyle();
				guistyle2.normal.textColor = Color.red;
				GUI.Label(new Rect(base.position.width / 2f - 90f, 210f, 150f, 30f), "Account password can not be empty", guistyle2);
			}
			EditorGUI.LabelField(new Rect(base.position.width / 2f - 90f, 225f, 50f, 30f), "Account", CompressEdit.fontsize4);
			EditorGUI.LabelField(new Rect(base.position.width / 2f - 90f, 285f, 50f, 30f), "Password", CompressEdit.fontsize4);
			CompressEdit.LoginName = EditorGUI.TextField(new Rect(base.position.width / 2f - 90f, 250f, 185f, 20f), CompressEdit.LoginName);
			CompressEdit.PassWorldName = EditorGUI.PasswordField(new Rect(base.position.width / 2f - 90f, 310f, 185f, 20f), CompressEdit.PassWorldName);
			if (GUI.Button(new Rect(base.position.width / 2f - 90f, 350f, 185f, 30f), new GUIContent("Login")) && CompressEdit.httpClient != null)
			{
				CompressEdit.httpClient.LoginPressed(CompressEdit.LoginName, CompressEdit.PassWorldName);
			}
			GUI.backgroundColor = new Color(0.76f, 0.76f, 0.76f);
			if (GUI.Button(new Rect(base.position.width / 2f - 35f, 390f, 150f, 20f), new GUIContent("Forget your password?"), CompressEdit.fontsize2))
			{
				Application.OpenURL("http://developers.masteropen.layabox.com/develope/min_forget.html");
			}
			if (GUI.Button(new Rect(base.position.width / 2f - 90f, 390f, 150f, 20f), new GUIContent("register"), CompressEdit.fontsize2))
			{
				Application.OpenURL("http://developers.masteropen.layabox.com/develope/min_register.html");
				return;
			}
		}
		else if (CompressEdit.page == 2)
		{
			GUI.Label(new Rect(base.position.width / 2f - 50f, 40f, 100f, 100f), CompressEdit.AriUrlTexture);
			GUI.Button(new Rect(base.position.width / 2f - 100f, 150f, 200f, 30f), HTTPClient.UserName, CompressEdit.fontsize);
			GUI.Box(new Rect(-1f, 190f, base.position.width + 1f, 1f), "");
			if (HTTPClient.VipInfo == "Not  Open")
			{
				GUI.Label(new Rect(base.position.width / 2f - 115f, 220f, 60f, 30f), " VIP  ", CompressEdit.fontsize4);
			}
			else if (HTTPClient.VipInfo == "Month VIP")
			{
				GUI.Label(new Rect(base.position.width / 2f - 115f, 202f, 100f, 100f), CompressEdit.VipM, CompressEdit.fontsize4);
			}
			else
			{
				GUI.Label(new Rect(base.position.width / 2f - 115f, 202f, 100f, 100f), CompressEdit.VipY, CompressEdit.fontsize4);
			}
			GUI.Label(new Rect(base.position.width / 2f - 50f, 215f, 100f, 30f), HTTPClient.VipInfo, CompressEdit.fontsize);
			if (GUI.Button(new Rect(base.position.width / 2f + 70f, 195f, 150f, 75f), CompressEdit.rechargeTexture, CompressEdit.fontsize2))
			{
				Application.OpenURL("http://developers.masteropen.layabox.com/develope/product_pvrtool.html?token=" + HTTPClient.Ftoken);
			}
			GUI.Label(new Rect(base.position.width / 2f + 85f, 203f, 100f, 50f), "Recharge", CompressEdit.fontsize5);
			GUI.Box(new Rect(-1f, 270f, base.position.width + 1f, 1f), "");
			GUI.Label(new Rect(60f, 290f, 150f, 30f), "VIP Function", CompressEdit.fontsize4);
			GUI.Label(new Rect(70f, 310f, 150f, 30f), "Andriod/IOS Asset Paltform(Texture Compression)", CompressEdit.fontsize3);
			if (GUI.Button(new Rect(base.position.width - 80f, 10f, 60f, 30f), "Sign Out"))
			{
				this.signout();
			}
			if (GUI.Button(new Rect(base.position.width - 80f, 50f, 60f, 30f), "Refresh") && CompressEdit.httpClient != null)
			{
				CompressEdit.httpClient.LoginPressed(CompressEdit.LoginName, CompressEdit.PassWorldName);
			}
		}
	}

	// Token: 0x06000003 RID: 3 RVA: 0x000029DC File Offset: 0x00000BDC
	public void signout()
	{
		HTTPClient._b_AccountUsed = 0;
		CompressEdit.LoginName = "";
		CompressEdit.PassWorldName = "";
		HTTPClient.vip = false;
		HTTPClient.LoginInfo.Clear();
	}

	// Token: 0x04000001 RID: 1
	private static GameObject ga;

	// Token: 0x04000002 RID: 2
	public static CompressEdit.TextureFormat textureFromat;

	// Token: 0x04000003 RID: 3
	public static string LoginName;

	// Token: 0x04000004 RID: 4
	public static string PassWorldName;

	// Token: 0x04000005 RID: 5
	public static HTTPClient httpClient;

	// Token: 0x04000006 RID: 6
	public static string SAVEPATH = "";

	// Token: 0x04000007 RID: 7
	public static string OPENFILEPATH = "";

	// Token: 0x04000008 RID: 8
	private static CompressEdit layaWindow;

	// Token: 0x04000009 RID: 9
	public static int page = 0;

	// Token: 0x0400000A RID: 10
	private static WWW www;

	// Token: 0x0400000B RID: 11
	private static Texture LoginTexture;

	// Token: 0x0400000C RID: 12
	private static WWW www2;

	// Token: 0x0400000D RID: 13
	private static Texture AriUrlTexture;

	// Token: 0x0400000E RID: 14
	private static WWW wwwVIPIcon;

	// Token: 0x0400000F RID: 15
	private static Texture VipY;

	// Token: 0x04000010 RID: 16
	private static Texture VipM;

	// Token: 0x04000011 RID: 17
	private static Texture rechargeTexture;

	// Token: 0x04000012 RID: 18
	private static GUIStyle fontsize = new GUIStyle();

	// Token: 0x04000013 RID: 19
	private static GUIStyle fontsize2 = new GUIStyle();

	// Token: 0x04000014 RID: 20
	private static GUIStyle fontsize3 = new GUIStyle();

	// Token: 0x04000015 RID: 21
	private static GUIStyle fontsize4 = new GUIStyle();

	// Token: 0x04000016 RID: 22
	private static GUIStyle fontsize5 = new GUIStyle();

	// Token: 0x0200000D RID: 13
	public enum TextureFormat
	{
		// Token: 0x04000097 RID: 151
		ETC1,
		// Token: 0x04000098 RID: 152
		PVRTC1_2,
		// Token: 0x04000099 RID: 153
		PVRTC1_2_RGB,
		// Token: 0x0400009A RID: 154
		PVRTC1_4,
		// Token: 0x0400009B RID: 155
		PVRTC1_4_RGB
	}
}
