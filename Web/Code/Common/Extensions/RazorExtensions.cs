using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Web.Code.Web;

namespace Web.Code.Common.Extensions
{
	public static class RazorExtensions
	{
		/// <summary>
		///     Converts the given object to JSON format
		/// </summary>
		/// <param name="helper"></param>
		/// <param name="obj"></param>
		public static IHtmlString ToJSON(this HtmlHelper helper, object obj)
		{
			string json = JsonConvert.SerializeObject(obj);
			return helper.Raw(json);
		}

		/// <summary>
		///     Appends the version number to this content
		/// </summary>
		/// <param name="url"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string ContentWithVersion(this UrlHelper url, string path)
		{
			if (url == null)
			{
				return "";
			}

			path = path ?? "";

			// Requires URL rewrite to work

			path = path.TrimStart(new[] {'~', '/'});
			path = "~/" + VersionInfo.AssemblyVersion + "/" + path;
			path = new WebEnvironment().GetFullUrl(path);
			return path;
		}
	}
}