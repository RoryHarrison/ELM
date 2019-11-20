using ELM.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ELM
{
	public class TweetMessage : IMessageService
	{
		private string _header;
		private string _body;
		private HashSet<Tuple<string, string>> output = new HashSet<Tuple<string, string>>();
		public string sender;
		public string message;

		public TweetMessage(String header, String body)
		{
			this._header = header;
			this._body = body;
		}

		public void Validate()
		{
			if(sender.Length > 15)
			{
				throw new MessageValidationException("Sender Length too long for Tweet Message");
			}
			if(message.Length > 140)
			{
				throw new MessageValidationException("Message Length too long for Tweet Message");
			}
		}

		public void ParseBody()
		{
			string body = this._body;
			try
			{
				//End of sender
				int x = body.ToLower().IndexOf("sender: ") + 8;
				//Start of message/end of sender text
				int y = body.ToLower().IndexOf("message: ");
				//End of Message identifier
				int z = y + 9;

				this.sender = body.Substring(x, y - (x + 1));
				this.message = body.Substring(z, body.Length - z);
			}
			catch (Exception e)
			{
				//Raise this exception to MainWindow
				throw e;
			}
		}

		public void SanitiseBody()
		{

		}

		public HashSet<Tuple<string, string>> GetOutput()
		{
			output.Add(new Tuple<string, string>("sender", sender));
			output.Add(new Tuple<string, string>("message", message));
			return output;
		}

		public override string ToString()
		{
			return "Tweet Message - header: " + this._header + " body: " + this._body;
		}
	}
}
