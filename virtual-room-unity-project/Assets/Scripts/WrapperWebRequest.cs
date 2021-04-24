using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;


public class WrapperWebRequest {

	public enum ErroType { None, Network, Http, Other }

	public bool Requesting { get; private set; }
	public string ResponseText { get; private set; }
	public int ResponseCode { get; private set; }
	public ErroType ErrorStatus { get; private set; }
	public string Error { get; private set; }
	public string ContentType { get; set; }
	public string PostData { get; set; }

	public WrapperWebRequest(string name, string url, string method) {
		url_ = url;
		headers_ = new Dictionary<string, string>();
		method_ = method;
		name_ = name;
	}

	public void SetRequestHeader(string name, string value) {
		if (!headers_.ContainsKey(name)) {
			headers_.Add(name, value);
		} else {
			headers_[name] = value;
		}
	}

	public void Send() {
		Requesting = true;
		switch (method_) {
			case "GET":
				Get_Process();
				break;
			case "POST":
				Post_Process();
				break;
			default:
				break;
		}
	}

	public void SendAsync() {
		ResponseCode = 0;
		ResponseText = "";
		ErrorStatus = ErroType.None;
		Error = "";
		Thread thread;
		Requesting = true;
		switch (method_) {
			case "GET":
				thread = new Thread(new ThreadStart(Get_Process));
				thread.Start();
				break;
			case "POST":
				thread = new Thread(new ThreadStart(Post_Process));
				thread.Start();
				break;
			default:
				break;
		}
	}

	void Get_Process() {
		try {
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url_);
			request.Method = "GET";
			request.AutomaticDecompression = DecompressionMethods.GZip;
			if (!string.IsNullOrEmpty(ContentType))
				request.ContentType = ContentType;
			foreach (var item in headers_) {
				request.Headers[item.Key] = item.Value;
			}
			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			using (Stream stream = response.GetResponseStream())
			using (StreamReader reader = new StreamReader(stream)) {
				ResponseText = reader.ReadToEnd();
				ResponseCode = (int)response.StatusCode;
			}
			ErrorStatus = ErroType.None;
			Error = "";
		} catch (WebException e) {
			if (e.Status == WebExceptionStatus.ProtocolError)
				ResponseCode = (int)((HttpWebResponse)e.Response).StatusCode;
			else
				ResponseCode = 0;
			ErrorStatus = e.Status == WebExceptionStatus.ProtocolError ? ErroType.Http :
				(e.Status == WebExceptionStatus.ConnectFailure ? ErroType.Network : ErroType.Other);
			Error = e.Message;
			ResponseText = "";
		} catch (Exception e) {
			ResponseCode = 0;
			ErrorStatus = ErroType.Other;
			Error = e.Message;
			ResponseText = "";
		}
		Requesting = false;
	}

	void Post_Process() {
		try {
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url_);
			request.Method = "POST";
			request.AutomaticDecompression = DecompressionMethods.GZip;
			if (!string.IsNullOrEmpty(ContentType))
				request.ContentType = ContentType;
			foreach (var item in headers_) {
				request.Headers[item.Key] = item.Value;
			}
			byte[] byteArray = Encoding.UTF8.GetBytes(PostData);
			request.ContentLength = byteArray.Length;
			using (Stream dataStream = request.GetRequestStream()) {
				dataStream.Write(byteArray, 0, byteArray.Length);
			}
			using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
			using (Stream stream = response.GetResponseStream())
			using (StreamReader reader = new StreamReader(stream)) {
				ResponseText = reader.ReadToEnd();
				ResponseCode = (int)response.StatusCode;
			}
			ErrorStatus = ErroType.None;
			Error = "";
		} catch (WebException e) {
			if (e.Status == WebExceptionStatus.ProtocolError)
				ResponseCode = (int)((HttpWebResponse)e.Response).StatusCode;
			else
				ResponseCode = 0;
			ErrorStatus = e.Status == WebExceptionStatus.ProtocolError ? ErroType.Http :
				(e.Status == WebExceptionStatus.ConnectFailure ? ErroType.Network : ErroType.Other);
			Error = e.Message;
			ResponseText = "";
		} catch (Exception e) {
			ResponseCode = 0;
			ErrorStatus = ErroType.Other;
			Error = e.Message;
			ResponseText = "";
		}
		Requesting = false;
	}

	Dictionary<string, string> headers_;
	readonly string name_;
	readonly string url_;
	readonly string method_;
}