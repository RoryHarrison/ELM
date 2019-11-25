using System;
using System.Collections.Generic;
using System.Text;

namespace ELM
{
	public class EmailFactory : MessageFactory
	{
		private string _header;
		private string _body;

		public EmailFactory(String header, String body)
		{
			this._header = header;
			this._body = body;
		}

		public override IMessageService GetMessage()
		{
			return new EmailMessage(_header, _body);
		}
	}
}
