using System;
using System.Collections.Generic;
using System.Text;

namespace ELM
{
	class TweetFactory : MessageFactory
	{
		private string _header;
		private string _body;

		public TweetFactory(String header, String body)
		{
			this._header = header;
			this._body = body;
		}

		public override IMessageService GetMessage()
		{
			return new TweetMessage(_header, _body);
		}
	}
}
