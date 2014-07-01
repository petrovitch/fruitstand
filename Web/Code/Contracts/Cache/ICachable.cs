using System.Collections.Generic;

namespace Web.Code.Contracts.Cache
{
	public interface ICachable
	{
		string CacheKey { get; set; }
		List<string> Tags { get; set; }
	}

	
}
