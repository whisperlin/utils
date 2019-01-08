using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000003 RID: 3
public class HTTPClient : MonoBehaviour
{
	// Token: 0x06000006 RID: 6 RVA: 0x00002A6C File Offset: 0x00000C6C
	public void LoginPressed(string _accountNumber, string _passWorld)
	{
		HTTPClient.LoginInfo.Clear();
		HTTPClient.LoginInfo.Add("identifier", _accountNumber);
		HTTPClient.LoginInfo.Add("password", _passWorld);
		if (_accountNumber == "" || _passWorld == "")
		{
			HTTPClient._b_AccountUsed = 3;
			return;
		}
		if (_accountNumber[0] == '1')
		{
			HTTPClient.LoginInfo.Add("idtype", "3");
		}
		else
		{
			HTTPClient.LoginInfo.Add("idtype", "1");
		}
		base.StartCoroutine(this.GETLogin("http://api.masteropen.layabox.com/layapassport/login", HTTPClient.LoginInfo));
	}

	// Token: 0x06000007 RID: 7 RVA: 0x00002B10 File Offset: 0x00000D10
	public void textureInfo(string SavePath)
	{
		HTTPClient.LoadingTexture++;
		base.StartCoroutine(this.GETTexture("https://api.nodedevelopers.layabox.com/pvr/exchange", HTTPClient._texture, HTTPClient.format, HTTPClient.extension, SavePath));
	}

	// Token: 0x06000008 RID: 8 RVA: 0x00002B40 File Offset: 0x00000D40
	private IEnumerator GETLogin(string url, Dictionary<string, string> post)
	{
		string text = string.Concat(new string[]
		{
			"http://api.masteropen.layabox.com/layapassport/login?identifier=",
			post["identifier"].ToString(),
			"&password=",
			post["password"].ToString(),
			"&idtype=",
			post["idtype"].ToString()
		});
		UnityWebRequest www = UnityWebRequest.Get(text);
		yield return www.Send();
		if (www.error != null)
		{
			Debug.Log("error is :" + www.error);
		}
		else
		{
			JSONObject jsonobject = JSONObject.Create(www.downloadHandler.text, -2, false, false);
			if (jsonobject == null)
			{
				HTTPClient._b_AccountUsed = 1;
			}
			if (www.downloadHandler.text != "" && jsonobject["ret"].ToString() == "0")
			{
				HTTPClient._b_AccountUsed = 2;
				HTTPClient.UserName = jsonobject["data"]["mobile"].str;
				base.StartCoroutine(this.GETInfo(jsonobject));
			}
			else
			{
				HTTPClient._b_AccountUsed = 1;
			}
		}
		yield break;
	}

	// Token: 0x06000009 RID: 9 RVA: 0x00002B56 File Offset: 0x00000D56
	private IEnumerator GETInfo(JSONObject json)
	{
		string text = "http://api.masteropen.layabox.com/sso?access_token=" + json["data"]["token"].str;
		UnityWebRequest www = UnityWebRequest.Get(text);
		yield return www.Send();
		if (www.error != null)
		{
			Debug.Log("error is :" + www.error);
		}
		else
		{
			JSONObject jsonobject = JSONObject.Create(www.downloadHandler.text.Replace("\\", "").Replace(":\"{", ":{").Replace("}\"}", "}}"), 3, false, false);
			if (jsonobject.GetField("ret") != null)
			{
				string str = jsonobject["data"]["userInfo"]["userId"].str;
				string str2 = jsonobject["data"]["userInfo"]["channelExt"]["username"].str;
				HTTPClient.Ftoken = jsonobject["data"]["userInfo"]["token"].str;
				base.StartCoroutine(this.GETInfo2(jsonobject));
			}
		}
		yield break;
	}

	// Token: 0x0600000A RID: 10 RVA: 0x00002B6C File Offset: 0x00000D6C
	private IEnumerator GETInfo2(JSONObject json)
	{
		string str = json["data"]["userInfo"]["userId"].str;
		string str2 = json["data"]["userInfo"]["channelExt"]["mobile"].str;
		string str3 = json["data"]["userInfo"]["channelExt"]["email"].str;
		string text;
		if (json["data"]["userInfo"]["channelExt"]["mobile"].str == "")
		{
			if (json["data"]["userInfo"]["channelExt"]["mobile"].str == "")
			{
				text = str;
			}
			else
			{
				text = json["data"]["userInfo"]["channelExt"]["mobile"].str;
			}
		}
		else
		{
			text = json["data"]["userInfo"]["channelExt"]["mobile"].str;
		}
		if (json["data"]["userInfo"]["channelExt"].Count == 11)
		{
			HTTPClient.UserName = text;
		}
		else
		{
			HTTPClient.UserName = json["data"]["userInfo"]["channelExt"]["username"].str;
		}
		string text2 = "";
		string text3 = string.Concat(new string[]
		{
			"http://developers.masteropen.layabox.com/auth/reg_login?userId=",
			str,
			"&username=",
			text,
			"&mobile=",
			str2,
			"&email=",
			str3,
			"&avatarUrl=",
			text2
		});
		UnityWebRequest www = UnityWebRequest.Get(text3);
		yield return www.Send();
		if (www.error != null)
		{
			Debug.Log("error is :" + www.error);
		}
		else
		{
			JSONObject jsonobject = JSONObject.Create(www.downloadHandler.text, -2, false, false);
			if (jsonobject["data"]["year_vip"].ToString() == "1")
			{
				if (jsonobject["data"]["is_vip"].ToString() == "1")
				{
					HTTPClient.VipInfo = "Year  VIP";
					HTTPClient.vip = true;
				}
				else
				{
					HTTPClient.VipInfo = "Not  Open";
					HTTPClient.vip = false;
				}
			}
			else if (jsonobject["data"]["is_vip"].ToString() == "1")
			{
				HTTPClient.VipInfo = "Month VIP";
			}
			else
			{
				HTTPClient.VipInfo = "Not  Open";
			}
			HTTPClient.userID = jsonobject["data"]["uid"].n.ToString();
			HTTPClient.token = jsonobject["data"]["token"].str;
		}
		yield break;
	}

	// Token: 0x0600000B RID: 11 RVA: 0x00002B7B File Offset: 0x00000D7B
	private IEnumerator GETTexture(string url, string tex, string format, string extension, string SavePath)
	{
		WWWForm wwwform = new WWWForm();
		FileStream fileStream = new FileStream(tex, FileMode.OpenOrCreate, FileAccess.Read);
		this.fssize = new byte[fileStream.Length];
		BinaryReader binaryReader = new BinaryReader(fileStream);
		this.fssize = binaryReader.ReadBytes(this.fssize.Length);
		wwwform.AddBinaryData("file", this.fssize, format + "." + HTTPClient._texture.Substring(HTTPClient._texture.Length - 3));
		wwwform.AddField("format", format);
		wwwform.AddField("extension", extension);
		wwwform.AddField("developer_uid", HTTPClient.userID);
		wwwform.AddField("other", HTTPClient.OtherSetting);
		wwwform.AddField("token", HTTPClient.token);
		UnityWebRequest www = UnityWebRequest.Post(url, wwwform);
		yield return www.Send();
		HTTPClient.LoadingTexture--;
		if (HTTPClient.LoadingTexture == 0)
		{
			Debug.Log("Compress texture finish");
		}
		if (www.error != null)
		{
			Debug.Log("error is :" + www.error);
		}
		else
		{
			string text = www.downloadHandler.text;
			if (text != "")
			{
				Debug.Log("Error to compress texture");
				Debug.Log(text);
			}
			else
			{
				this.textureBytes = www.downloadHandler.data;
				File.WriteAllBytes(SavePath, this.textureBytes);
			}
		}
		yield break;
	}

	// Token: 0x04000017 RID: 23
	public static int LoadingTexture = 0;

	// Token: 0x04000018 RID: 24
	public static string OtherSetting;

	// Token: 0x04000019 RID: 25
	public static int MipMap;

	// Token: 0x0400001A RID: 26
	public static bool vip = false;

	// Token: 0x0400001B RID: 27
	public static string UserName;

	// Token: 0x0400001C RID: 28
	public static string VipInfo;

	// Token: 0x0400001D RID: 29
	public static string userID = "";

	// Token: 0x0400001E RID: 30
	public static string token = "";

	// Token: 0x0400001F RID: 31
	public static string Ftoken;

	// Token: 0x04000020 RID: 32
	public static string _texture;

	// Token: 0x04000021 RID: 33
	public static string format;

	// Token: 0x04000022 RID: 34
	public static string extension;

	// Token: 0x04000023 RID: 35
	public static Dictionary<string, string> LoginInfo = new Dictionary<string, string>();

	// Token: 0x04000024 RID: 36
	public static int _b_AccountUsed = 0;

	// Token: 0x04000025 RID: 37
	private byte[] textureBytes;

	// Token: 0x04000026 RID: 38
	private byte[] fssize;
}
