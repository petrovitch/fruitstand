using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Code.Contracts.Entities.ApiModels
{
	public class ErrorResultCode
	{
		public int Code { get; set; }
		public string Key { get; set; }
		public string Description { get; set; }
	}
}