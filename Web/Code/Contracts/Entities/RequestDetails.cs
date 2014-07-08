using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Code.Contracts.Entities
{
	/// <summary>
	/// Object containing information about an API request. Also used for message broadcasting
	/// </summary>
	public class RequestDetails
	{
		public string Method = "GET";
		public string BaseUrl = "";
		public string RelativeUrl = "";
		private Dictionary<string, object> Params { get; set; }
		public string SerializedContent = "";

		/// <summary>
		/// Concatenates our base and relative URLs
		/// </summary>
		public string FullUrl
		{
			get { return BaseUrl + RelativeUrl + "?" + QueryString; }
		}

		/// <summary>
		/// Values required for our query string
		/// </summary>
		public Dictionary<string, object> QueryStringValues
		{
			get
			{
				if (this.Method == "GET" || this.Method == "DELETE") return Params;
				return new Dictionary<string, object>();
			}
		}

		/// <summary>
		/// Formats our parameters into a query string appropriate for a URL
		/// </summary>
		public string QueryString
		{
			get
			{
				var queryParameters = HttpUtility.ParseQueryString("");
				this.QueryStringValues.Where(x => x.Value != null).ToList().ForEach(kvp => queryParameters[kvp.Key] = kvp.Value.ToString());
				return queryParameters.ToString();
			}
		}

		/// <summary>
		/// Values required to be encoded into our form request body
		/// </summary>
		public Dictionary<string, object> FormValues
		{
			get
			{
				if (this.Method == "POST") return Params;
				return new Dictionary<string, object>();
			}
		}

		/// <summary>
		/// Clears our parameter list
		/// </summary>
		public void ClearParameters()
		{
			this.Params = new Dictionary<string, object>();
		}

		/// <summary>
		/// Allows our users to set the private parameter source
		/// </summary>
		/// <param name="value"></param>
		/// <param name="obj"></param>
		public void SetParameter(string value, object obj)
		{
			this.Params[value] = obj;
		}
	}
}