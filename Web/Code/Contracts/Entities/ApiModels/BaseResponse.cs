using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Web.Code.Contracts.Entities.ApiModels
{
	/// <summary>
	/// Base class for our response methods, containing common properties
	/// </summary>
	public abstract class BaseResponse
	{
		public string StatusCode = "";
	}
}