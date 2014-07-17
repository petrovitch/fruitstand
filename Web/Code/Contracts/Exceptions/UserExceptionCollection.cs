using System.Collections.Generic;

namespace Web.Code.Contracts.Exceptions
{
	public class UserExceptionCollection : UserException
	{
		public List<UserException> Exceptions = new List<UserException>();
	}
}