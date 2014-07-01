using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
	public class ErrorModel : BaseModel
	{
		public List<string> Exceptions = new List<string>();
		public bool ShowHelpLink = true;
		public string MoreInfoUrl = "";
	}
}