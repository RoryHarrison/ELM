using System;
using System.Collections.Generic;
using System.Text;

namespace ELM
{
	abstract class MessageFactory
	{
		public abstract IMessageService GetMessage();
	}
}
