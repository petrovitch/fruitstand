using Web.Code.Contracts.Entities.ApiModels;

namespace Web.Models.Home
{
	public class PaymentCompleteModel : BaseModel
	{
		public string ErrorMessage = "";
		public bool IsError = false;
		public AnticipatedPaymentRepresentation PaymentInfo = new AnticipatedPaymentRepresentation();
		public AnticipatedPaymentStatusRepresentation StatusInfo;
	}
}