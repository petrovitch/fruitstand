namespace Web.Code.Contracts.Cache
{
	public interface IUserDataManager
	{
		/// <summary>
		/// Records this item in state
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		void Set<T>(string name, T value) where T : ICachable;

		/// <summary>
		/// Returns this value from our state bag
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <returns></returns>
		T Get<T>(string name) where T : ICachable;

		string UniqueBaseKey { get; set; }
	}
}
