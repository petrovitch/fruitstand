using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Code.Contracts.Entities.ApiModels
{
	public class PayerDetails
	{
		public string EmailAddress { get; set; }
		public string FullName { get; set; }
		public int? Id { get; set; }
		public string MobileNumber { get; set; }
	}
}