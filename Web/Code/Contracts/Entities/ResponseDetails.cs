using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Code.Contracts.Entities
{
	/// <summary>
	/// Object containing information about an API request. Also used for message broadcasting
	/// </summary>
	public class ResponseDetails
	{
		public string StatusCode = "";
		public string JSON = "";
	}
}