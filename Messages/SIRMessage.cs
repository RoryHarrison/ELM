using System;
using System.Collections.Generic;
using System.Text;

namespace ELM
{
	public class SIRMessage : IMessageService //inherit from Email Message
	{
		private string _header;
		private string _body;
		private HashSet<Tuple<string, string>> output = new HashSet<Tuple<string, string>>();
		public string sender;
		public string message;

		public SIRMessage(string header, string body)
		{
			this._header = header;
			this._body = body;
		}

		public void Validate() {/*TO DO*/}

		public void ParseBody()
		{

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
			return "SMS Message - header: " + this._header + " body: " + this._body;
		}
	}
}
