using System;

namespace Web.Code.Contracts.Exceptions
{
	public class UserException : Exception
	{
		public UserException() : base() { }
		public UserException(string msg) : base(msg) { }

	}
}