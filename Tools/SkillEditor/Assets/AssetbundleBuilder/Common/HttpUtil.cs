using System;
using System.Net;
using System.Threading;
using UnityEngine;

namespace Common
{
	public class HttpUtil:Singleton<HttpUtil>
	{
		// 默认超时时间
		private int timeout;

		/// <summary>
		/// 构造函数
		/// </summary>
		public HttpUtil ()
		{
			this.timeout = 5000; // 默认超时时间5s
		}

		public string Get(string url, Action<HttpWebResponse> callback) {
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create (url);
			request.Method = "GET";
			request.Timeout = timeout;

			request.BeginGetResponse (new AsyncCallback (OnRespone), callback);
			HttpWebResponse response = request.GetResponse () as HttpWebResponse;
			if (response.StatusCode == HttpStatusCode.OK) {
				return response.GetResponseStream ().ToString ();
			}
			return null;
		}

		public void OnRespone(IAsyncResult result) {
			
		}
	}

	/// <summary>
	/// Http client.
	/// </summary>
	public class HttpClient {
		private string m_Url;
		private Action<HttpWebResponse> m_CallBack;
		private int m_Timeout;

		public HttpClient(string url, Action<HttpWebResponse> callback, int timeout) {
			this.m_Url = url;
			this.m_CallBack = callback;
			this.m_Timeout = timeout;
		}

		public string Url {
			get { 
				return m_Url;
			}
		}

		public int Timeout {
			get {
				return m_Timeout;
			}
		}

		public void Start() {
			HttpWebRequest request = WebRequest.Create (m_Url) as HttpWebRequest;
			request.Timeout = m_Timeout;

			AsyncCallback callback = new AsyncCallback (OnRespone);
			request.BeginGetResponse (callback, request);
		}

		public void OnRespone(IAsyncResult result) {
			HttpWebRequest request = (HttpWebRequest)result.AsyncState;
			if (request == null) {
				return;
			}

			try {
				HttpWebResponse response = request.EndGetResponse(result) as HttpWebResponse;
				if (response == null) {
					return;
				}

				m_CallBack(response);
			} catch (Exception exception) {
				
			}
		}
	}
}

