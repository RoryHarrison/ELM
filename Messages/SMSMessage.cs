using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using ELM.Exceptions;

namespace ELM
{
	[DataContract]
	public class SMSMessage : IMessageService
	{
		[DataMember(Name = "header", IsRequired = true, Order = 2)]
		private string _header;
		private string _body;
		private Dictionary<string, string> _textSpeak;
		private HashSet<Tuple<string, string>> output = new HashSet<Tuple<string,string>>();
		[DataMember(Name = "sender", IsRequired = true, Order = 3)]
		public string sender;
		[DataMember(Name = "message", IsRequired = true, Order = 4)]
		public string message;
		[DataMember(Name = "type", IsRequired = true, Order = 1)]
		public string type = "SMS";

		public SMSMessage(String Header, String Body, Dictionary<string, string> textSpeak)
		{
			this._header = Header;
			this._body = Body;
			this._textSpeak = textSpeak;
		}

		string IMessageService.Header
		{
			get { return this._header; }
		}

		public void Validate()
		{
			//Sender Validation
			//GB phone number regex
			Regex phone = new Regex(@"^(((\+44\s?\d{4}|\(?0\d{4}\)?)\s?\d{3}\s?\d{3})|((\+44\s?\d{3}|\(?0\d{3}\)?)\s?\d{3}\s?\d{4})|((\+44\s?\d{2}|\(?0\d{2}\)?)\s?\d{4}\s?\d{4}))(\s?\#(\d{4}|\d{3}))?$");
			Match match = phone.Match(sender);
			if(!match.Success)
			{
				throw new MessageValidationException("Invalid Phone Number: "+sender);
			}

			if(message.Length > 140)
			{
				throw new MessageValidationException("Message Length too long for SMS Message");
			}
		}

		public void ParseBody()
		{
			string body = this._body;
			try
			{
				//End of sender
				int x = body.ToLower().IndexOf("sender: ")+8;
				//Start of message/end of sender text
				int y = body.ToLower().IndexOf("message: ");
				//End of Message identifier
				int z = y + 9;

				this.sender = body.Substring(x, y - (x+1));
				this.message = body.Substring(z, body.Length - z);
			}
			catch(Exception e)
			{
				//Raise this exception to MainWindow
				throw e;
			}
		}

		public HashSet<Tuple<string,string>> GetOutput()
		{
			output.Add(new Tuple<string, string>("sender", sender));
			output.Add(new Tuple<string, string>("message", message));
			return output;
		}

		public void SanitiseBody()
		{
			foreach(string word in (Regex.Replace(message, @"[^\w\s@#]", "").Split(' ')))
			{
				if (_textSpeak.TryGetValue(word, out string value))
				{
					message = message.Insert((message.IndexOf(word) + word.Length), " <" + value + ">");
				}
			}
		}

		public override string ToString()
		{
			return "Header: " + _header + "\nType: " + type + "\nSender: " + sender + "\nMessage: " + message;
		}
	}
}
