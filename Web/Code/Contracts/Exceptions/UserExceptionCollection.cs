using System.Collections.Generic;

namespace Contracts.Exceptions
{
	/// <summary>
	/// Summary description for UserExceptionCollection
	/// </summary>
	public class UserExceptionCollection : UserException
	{
		public List<UserException> Exceptions = new List<UserException>();
	}
}