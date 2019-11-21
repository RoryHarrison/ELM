using System;
using System.Collections.Generic;
using System.Text;

namespace ELM
{
	class TweetFactory : MessageFactory
	{
		private string _header;
		private string _body;
		private Dictionary<string,string> _textSpeak;
		private Dictionary<string, string> _hashtags;
		private Dictionary<string, string> _mentions;

		public TweetFactory(String header, String body, Dictionary<string,string> textSpeak, Dictionary<string, string> hashtags, Dictionary<string,string> mentions)
		{
			this._header = header;
			this._body = body;
			this._textSpeak = textSpeak;
			this._hashtags = hashtags;
			this._mentions = mentions;
		}

		public override IMessageService GetMessage()
		{
			return new TweetMessage(_header, _body, _textSpeak, _hashtags, _mentions);
		}
	}
}
