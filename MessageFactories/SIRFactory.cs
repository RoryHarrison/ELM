using System;
using System.Collections.Generic;
using System.Text;

namespace ELM
{
	public class SIRFactory : MessageFactory
	{
		private string _header;
		private string _body;

		public SIRFactory(String header, String body)
		{
			this._header = header;
			this._body = body;
		}

		public override IMessageService GetMessage()
		{
			return new SIRMessage(_header, _body);
		}
	}
}
