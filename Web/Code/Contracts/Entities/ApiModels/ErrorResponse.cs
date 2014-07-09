using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Code.Contracts.Entities.ApiModels
{
	public class ErrorResponse : BaseResponse
	{
		public string Message { get; set; }
		public Dictionary<string, string[]> ValidationFailures { get; set; }
		public Guid? Id { get; set; }
		public ErrorResultCode ResultCode { get; set; }
	}
}