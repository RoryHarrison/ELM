using System;
using System.Collections.Generic;
using System.Text;

namespace ELM
{
	public class MessageProcessor
	{
		private IMessageService _service;
		public MessageProcessor(IMessageService service)
		{
			this._service = service;
		}
		public void Process() { this._service.Process(); }
	}
}
