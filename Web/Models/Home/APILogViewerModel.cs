using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.Code.Contracts.Entities;

namespace Web.Models.Home
{
	public class APILogViewerModel : BaseModel
	{
		public Guid? CurrentUserID = Guid.Empty;
	}
}