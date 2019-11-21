using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ELM
{
	[DataContract]
	public class SIRMessage : IMessageService //inherit from Email Message
	{
		[DataMember(Name = "header", IsRequired = true, Order = 2)]
		private string _header;
		private string _body;
		private HashSet<Tuple<string, string>> output = new HashSet<Tuple<string, string>>();
		[DataMember(Name = "sender", IsRequired = true, Order = 3)]
		public string sender;
		[DataMember(Name = "message", IsRequired = true, Order = 4)]
		public string message;
		[DataMember(Name = "type", IsRequired = true, Order = 1)]
		public string type = "SIR";

		public SIRMessage(string header, string body)
		{
			this._header = header;
			this._body = body;
		}

		string IMessageService.Header
		{
			get { return this._header; }
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
