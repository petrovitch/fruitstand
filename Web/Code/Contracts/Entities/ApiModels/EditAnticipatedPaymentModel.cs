using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Code.Contracts.Entities.ApiModels
{
	public class EditAnticipatedPaymentModel
	{
		public EditAnticipatedPaymentModel()
		{
			Fields = new List<FieldConfigModel>();
		}

		public string Description { get; set; }
		public string DescriptionTitle { get; set; }
		public List<FieldConfigModel> Fields { get; set; }
		public string Merchant { get; set; }
		public PayerDetails Payer { get; set; }
		public string Reference { get; set; }
		public string ReturnTitle { get; set; }
		public string ReturnUrl { get; set; }
		public string Token { get; set; }
	}
}