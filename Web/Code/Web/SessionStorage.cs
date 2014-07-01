using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using Contracts.Cache;
using Web.Code.Contracts.Cache;

namespace Web.Code.Web
{
	/// <summary>
	/// Stores and retrieves the given items in HttpContext session
	/// </summary>
	public class SessionStorage :IUserDataManager
	{

		public string UniqueBaseKey { get; set; }

		private Dictionary<string, string> CurrentValues = new Dictionary<string, string>();

		/// <summary>
		/// Records this item in state
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public void Set<T>(string name, T value) where T : ICachable
		{
			if (HttpContext.Current == null || HttpContext.Current.Session == null) return;

			try
			{
				if (value == null)
				{
					if (HttpContext.Current.Session[name] != null) HttpContext.Current.Session.Remove(name);
					return;
				}

				HttpContext.Current.Session[name] = value;
			}
			finally
			{
				// Save to memory as well for quick access later
				if (value == null)
				{
					if (CurrentValues.ContainsKey(name)) CurrentValues.Remove(name);
				}
				else
				{
					CurrentValues[name] = value.ToString();
				}
			}
		}

		/// <summary>
		/// Returns this value from our state bag
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <returns></returns>
		public T Get<T>(string name) where T : ICachable
		{
			bool isNullable = false;
			try
			{
				Type newType = typeof(T);
				// ChangeType throws exception for null types, so just throw it here
				if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
				{
					newType = new NullableConverter(typeof(T)).UnderlyingType;
					isNullable = true;
				}

				// Already recorded in memory?
				if (CurrentValues.ContainsKey(name)) return (T)Convert.ChangeType(CurrentValues[name], newType);

				// Check session
				if (HttpContext.Current != null && HttpContext.Current.Session != null)
				{
					var result = HttpContext.Current.Session[name];
					if (result != null) return (T) result;
				}
			}
			catch (Exception ex)
			{
				var x = ex;
			}

			// Return default
			if (isNullable) return (T)(object)null;
			return default(T);
		}

	}
}

