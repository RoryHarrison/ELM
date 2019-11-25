using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ELM.Exceptions
{
	public class MessageValidationException : Exception
	{

		public string ErrorMessage;

		public MessageValidationException()
		{

		}

		public MessageValidationException(string message) : base(message)
		{
			this.ErrorMessage = message;
		}

		public MessageValidationException(string message, Exception inner) : base(message, inner)
		{
			this.ErrorMessage = message;
		}

	}
}
