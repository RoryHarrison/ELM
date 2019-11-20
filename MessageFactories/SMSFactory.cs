using System;
using System.Collections.Generic;
using System.Text;

namespace ELM
{
	class SMSFactory : MessageFactory
	{
		private string _header;
		private string _body;

		public SMSFactory(string header, string body)
		{
			this._header = header;
			this._body = body;
		}

		public override IMessageService GetMessage()
		{
			return new SMSMessage(_header, _body);
		}
	}
}
