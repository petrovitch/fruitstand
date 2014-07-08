using System;
using System.Configuration;
using System.Reflection;
using System.Web;
using Contracts.Exceptions;

namespace Web.Code.Common
{
	/// <summary>
	/// Abstraction of our configuration class
	/// </summary>
	public class Configuration
	{

		#region Static Methods

		public static Configuration Current
		{
			get
			{
				return new Configuration();
			}
		}

		#endregion

		#region Application status

		/// <summary>
		/// Gets the current web assembly
		/// </summary>
		public Assembly CurrentWebAssembly
		{
			get
			{
				if (_CurrentWebAssembly == null)
				{

					lock (_CurrentWebAssemblyLock)
					{
						// We want the 'web' assembly because that one is ALWAYS recompiled when we deploy
						var classInWebAssembly = new WebEnvironment();
						_CurrentWebAssembly = System.Reflection.Assembly.GetAssembly(classInWebAssembly.GetType());
					}
				}
				return _CurrentWebAssembly;
			}
		}

		private static Assembly _CurrentWebAssembly = null;
		private static string _CurrentWebAssemblyLock = "";

		public string AssemblyVersion
		{
			get
			{
				var result = "v" + BuildNumber;

				// The current assembly isn't necessarily rebuilt after every debug, so we timestamp it here
				if (IsDeveloperMode) result += DateTime.Now.ToString("hhmmfffff");

				return result;
			}
		}

		public string BuildNumber
		{
			get
			{
				var ass = CurrentWebAssembly;
				if (ass == null) return "0";
				var result = CurrentWebAssembly.GetName().Version.ToString();

				// Strip out the dots so that we have an integer to deal with
				var components = result.Split('.');
				var current = "";
				foreach (var component in components)
				{
					var s = component.PadLeft(5, '0');
					current += s;
				}
				return current;

			}
		}

		#endregion

		#region Configuration Accessors

		/// <summary>
		/// Returns a string representation of this application setting
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		private string GetString(string key)
		{
			var result = System.Configuration.ConfigurationManager.AppSettings[key] ?? "";
			return result;
		}

		/// <summary>
		/// Returns a boolean representation of this application setting
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		private bool GetBool(string key, bool defaultValue = false)
		{
			var result = GetString(key);
			if (string.IsNullOrEmpty(result)) { return defaultValue; }
			return bool.Parse(result);
		}

		/// <summary>
		/// Returns an integer representation of this application setting
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		private int GetInt(string key, int defaultValue = 0)
		{
			var val = GetString(key);
			if (string.IsNullOrEmpty(val)) { return defaultValue; }
			var result = 0;
			if (!int.TryParse(val, out result)) return defaultValue;
			return result;
		}

		#endregion

		#region Application Settings (usually stored in Web.config AppSettings)

		public bool IsDeveloperMode { get { return GetBool("IsDeveloperMode"); } }
		public string PushpayAPIBaseUrl { get { return GetString("PushpayAPIBaseUrl"); } }
		public string PushpayClientID { get { return GetString("PushpayClientID"); } }
		public string PushpayClientSecret { get { return GetString("PushpayClientSecret"); } }
		public string OAuth2TokenEndpoint { get { return GetString("OAuth2TokenEndpoint"); } }
		public string OAuth2AuthorizeEndpoint { get { return GetString("OAuth2AuthorizeEndpoint"); } }
		public int TaxPercentage { get { return GetInt("TaxPercentage"); } }

		#endregion

	}
}
