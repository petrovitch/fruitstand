namespace Web.Code.Contracts.Entities.ApiModels
{
	public class FieldConfigModel
	{
		public string Key { get; set; }
		public bool? ReadOnly { get; set; }
		public object Value { get; set; }
	}
}