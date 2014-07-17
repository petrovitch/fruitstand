using System.Collections.Generic;

namespace Web.Code.Contracts.Entities.ApiModels
{
	public class AnticipatedPaymentRepresentation : BaseResponse
	{
		public AnticipatedPaymentRepresentation()
		{
			Fields = new List<FieldConfigModel>();
		}

		/// <summary>
		///     Helper function to retrieve the payment url from our link collection, if it exists
		/// </summary>
		public string PaymentUrl
		{
			get
			{
				if (Links == null || !Links.ContainsKey("pay")) return "";
				return Links["pay"].Href;
			}
		}

		public string Description { get; set; }
		public string DescriptionTitle { get; set; }
		public List<FieldConfigModel> Fields { get; set; }
		public int MerchantId { get; set; }
		public PayerDetails Payer { get; set; }
		public string Reference { get; set; }
		public string ReturnTitle { get; set; }
		public string ReturnUrl { get; set; }
		public string Token { get; set; }
	}
}