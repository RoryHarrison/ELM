using ELM.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace ELM
{
	[DataContract]
	public class SIRMessage : EmailMessage
	{
		//Sport Centre Code
		[DataMember(Name="SCC", IsRequired = true, Order = 6)]
		private string SCC;
		//Nature of Incident
		[DataMember(Name="NOI", IsRequired = true, Order = 7)]
		private string NOI;
		private string outputMessage;
		private string[] validNOI = new string[]
		{"Theft of Property", "Staff Attack", "Device Damage",
		 "Raid", "Customer Attack", "Staff Abuse", "Bomb Threat",
		 "Terrorism", "Suspicious Incident", "Sport Injury",
		 "Personal Info Leak" };
		new public string type = "SIR";

		public SIRMessage(string header, string body) : base(header, body)
		{
			this._header = header;
			this._body = body;
		}

		public override void ParseBody()
		{
			if (!_body.ToLower().Contains("sport centre code: ") || !_body.ToLower().Contains("nature of incident: "))
			{
				throw new MessageValidationException("Please provide both a Sport Centre Code and Nature of Incident for SIR message");
			}
			string body = this._body;
			try
			{
				int subjectStart = body.ToLower().IndexOf("subject: ");
				int sccStart = body.ToLower().IndexOf("sport centre code: ");
				int noiStart = body.ToLower().IndexOf("nature of incident: ");
				int messageStart = body.ToLower().IndexOf("message: ");

				int senderEnd = body.ToLower().IndexOf("sender: ") + 8;
				int subjectEnd = subjectStart + 9;
				int sccEnd = sccStart + 19;
				int noiEnd = noiStart + 20;
				int messageEnd = messageStart + 9;
				
				this.sender = body.Substring(senderEnd, subjectStart - (senderEnd + 1));
				this.subject = body.Substring(subjectEnd, sccStart - (subjectEnd + 1));
				this.SCC = body.Substring(sccEnd, noiStart - (sccEnd + 1));
				this.NOI = body.Substring(noiEnd, messageStart - (noiEnd + 1));
				this.message = body[sccStart..];
				this.outputMessage = body[messageEnd..];

				checkNOI();
				checkSCC();
				writeSIR();
			}catch(Exception e)
			{
				throw e;
			}
		}

		private void writeSIR()
		{
			using(var writer = new StreamWriter(@"C:\Users\roryh\source\repos\ELM\Data\SIRs.csv", append:true))
			{
				writer.WriteLine("Sport Centre Code: {0}, Nature of Incident: {1}",SCC, NOI);
				writer.Flush();
			}
		}

		private void checkNOI()
		{
			if (!validNOI.Contains(NOI))
			{
				throw new MessageValidationException("Please Enter a Valid Nature of Incident");
			}
		}

		private void checkSCC()
		{
			Regex regex = new Regex(@"\d{2}-\d{3}-\d{2}");
			Match match = regex.Match(SCC);
			if (!match.Success)
			{
				throw new MessageValidationException("Please Enter a Valid Sport Centre Code");
			}
		}

		public override string ToString()
		{
			return "Header: " + _header + "\nType: " + type + "\nSender: " + sender + "\nSubject: " + subject + "\nSport Centre Code: " + SCC + "\nNature of Incident: " + NOI + "\nMessage: " + outputMessage;
		}
	}
}
