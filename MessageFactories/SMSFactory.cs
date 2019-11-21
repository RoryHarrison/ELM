using System;
using System.Collections.Generic;
using System.Text;

namespace ELM
{
	class SMSFactory : MessageFactory
	{
		private string _header;
		private string _body;
		private Dictionary<string,string> _textSpeak;

		public SMSFactory(string header, string body, Dictionary<string,string> textSpeak)
		{
			this._header = header;
			this._body = body;
			this._textSpeak = textSpeak;
		}

		public override IMessageService GetMessage()
		{
			return new SMSMessage(_header, _body, _textSpeak);
		}
	}
}
