using ELM.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace ELM
{
	[DataContract]
	public class TweetMessage : IMessageService
	{
		[DataMember(Name = "header", IsRequired = true, Order = 2)]
		private string _header;
		private string _body;
		private Dictionary<string,string> _textSpeak;
		private Dictionary<string, string> _hashtags;
		private Dictionary<string, string> _mentions;
		private HashSet<Tuple<string, string>> output = new HashSet<Tuple<string, string>>();
		[DataMember(Name = "sender", IsRequired = true, Order = 3)]
		public string sender;
		[DataMember(Name = "message", IsRequired = true, Order = 4)]
		public string message;
		[DataMember(Name = "type", IsRequired = true, Order = 1)]
		public string type = "Tweet";

		public TweetMessage(String header, String body, Dictionary<string,string> textSpeak, Dictionary<string, string> hashtags, Dictionary<string, string> mentions)
		{
			this._header = header;
			this._body = body;
			this._textSpeak = textSpeak;
			this._hashtags = hashtags;
			this._mentions = mentions;
		}

		string IMessageService.Header
		{
			get { return this._header; }
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
			Regex mention = new Regex(@"(^|\B)@(?![0-9_]+\b)([a-zA-Z0-9_]{1,30})(\b|\r)");
			Regex hashtag = new Regex(@"(^|\B)#(?![0-9_]+\b)([a-zA-Z0-9_]{1,30})(\b|\r)");
			foreach (string word in (Regex.Replace(message, @"[^\w\s@#]", "").Split(' ')))
			{
				if (_textSpeak.TryGetValue(word, out string value))
				{
					message = message.Insert((message.IndexOf(word) + word.Length), " <" + value + ">");
				}

				Match mentionMatch = mention.Match(word);
				if (mentionMatch.Success)
				{
					if(_mentions.TryGetValue(word, out string mvalue))
					{
						int.TryParse(_mentions[word], out int val);
						_mentions[word] = (val + 1).ToString();
					}
					else
					{
						_mentions.Add(word, "1");
					}
				}

				Match hashtagMatch = hashtag.Match(word);
				if (hashtagMatch.Success)
				{
					if (_hashtags.TryGetValue(word, out string mvalue))
					{
						int.TryParse(_hashtags[word], out int val);
						_hashtags[word] = (val + 1).ToString();
					}
					else
					{
						_hashtags.Add(word, "1");
						Console.Out.Write("YES");
					}
				}
			}
			WriteTrending();
		}

		public void WriteTrending()
		{
			using (var writer = new StreamWriter(@"C:\Users\roryh\source\repos\ELM\Data\mentions.csv"))
			{
				foreach(var pair in _mentions)
				{
					writer.WriteLine("{0},{1},", pair.Key, pair.Value);
				}
				writer.Flush();
			}
			using (var writer = new StreamWriter(@"C:\Users\roryh\source\repos\ELM\Data\hashtags.csv"))
			{
				foreach (var pair in _hashtags)
				{
					writer.WriteLine("{0},{1},", pair.Key, pair.Value);
				}
				writer.Flush();
			}
		}

		public HashSet<Tuple<string, string>> GetOutput()
		{
			output.Add(new Tuple<string, string>("sender", sender));
			output.Add(new Tuple<string, string>("message", message));
			return output;
		}

		public override string ToString()
		{
			return "Header: " + _header + "\nType: " + type + "\nSender: " + sender + "\nMessage: " + message;
		}
	}
}
