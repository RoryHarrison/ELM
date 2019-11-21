using System;
using System.Collections.Generic;
using System.Text;

namespace ELM
{
	public interface IMessageService
	{
		string Header { get; }

		//Method for handling all the necessary functions
		void Process()
		{
			try
			{
				ParseBody();
				Validate();
				SanitiseBody();
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		//Validate that the message is the correct size
		void Validate();

		//Break up Body into meaningful sections
		void ParseBody();

		//Remove URLs expand Textspeak abbreviations
		void SanitiseBody();

		HashSet<Tuple<string,string>> GetOutput();
	}
}
