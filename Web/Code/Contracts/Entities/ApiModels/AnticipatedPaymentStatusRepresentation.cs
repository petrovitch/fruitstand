namespace Web.Code.Contracts.Entities.ApiModels
{
	public class AnticipatedPaymentStatusRepresentation : BaseResponse
	{
		public string Status { get; set; }
		public bool Completed { get; set; }
	}
}