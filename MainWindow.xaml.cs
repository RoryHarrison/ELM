using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ELM.Exceptions;

namespace ELM
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		Dictionary<string, string> textSpeak = null;
		Dictionary<string, string> hashtagList = null;
		Dictionary<string, string> mentionList = null;

		public MainWindow()
		{
			InitializeComponent();
			updateData();
			this.Closed += new EventHandler(MainWindow_Closed);
		}

		void updateData()
		{
			textSpeak = readCSV(@"C:\Users\roryh\source\repos\ELM\Data\textwords.csv");
			hashtagList = readCSV(@"C:\Users\roryh\source\repos\ELM\Data\hashtags.csv");
			mentionList = readCSV(@"C:\Users\roryh\source\repos\ELM\Data\mentions.csv");
		}

		void MainWindow_Closed(object sender, EventArgs e)
		{
			updateData();
			//Hashtags
			string hashtagString = string.Join("\n", hashtagList.Select(x => string.Format("{0}, {1}", x.Key, x.Value)));
			MessageBox.Show("Hashtags:\n"+hashtagString);
	
			//Mentions
			string mentionString = string.Join("\n", mentionList.Select(x => string.Format("{0}, {1}", x.Key, x.Value)));
			MessageBox.Show("Mentions:\n" + mentionString);

			//Serious Incident Report
			string[][]SIRArray = File.ReadAllLines(@"C:\Users\roryh\source\repos\ELM\Data\SIRs.csv").Select(l => l.Split(',').ToArray()).ToArray();
			StringBuilder SIRString = new StringBuilder();
			foreach(string[] row in SIRArray)
			{
				SIRString.Append(string.Format("{0}, {1}\n", row[0], row[1]));
			}
			MessageBox.Show("Serious Incident Reports:\n"+SIRString);
		}

		public Dictionary<string,string> readCSV(string path)
		{
			try
			{
				return File.ReadLines(path).Select(line => line.Split(',')).ToDictionary(line => line[0], line => line[1]);
			}catch(Exception ex)
			{
				MessageBox.Show("Unable to read file at: '"+ path+"' Error: "+ex.Message);
				return null;
			}
		}

		private IMessageService GetMessage(string header, string body)
		{

			//Validation
			if (header == null || body == null)
			{
				MessageBox.Show("Please Enter a Message Header and Body");
				return null;
			}

			if (!(header.Length == 10))
			{
				MessageBox.Show("Invalid Message Header Size");
				return null;
			}

			if (!body.ToLower().Contains("sender: ") || !body.ToLower().Contains("message: "))
			{
				MessageBox.Show("Message requires a sender and message within the mssage body");
				return null;
			}
			//Validation End


			switch (header[0])
			{
				case ('S'):
					if (ContainsSubject(body))
					{
						MessageBox.Show("Subject is not allowed for this message type");
						return null;
					}
					return new SMSFactory(header, body, this.textSpeak).GetMessage();
				case ('E'):
					if (!ContainsSubject(body))
					{
						MessageBox.Show("Subject is required for this message type");
						return null;
					}
					return new EmailFactory(header, body).GetMessage();
				case ('T'):
					if (ContainsSubject(body))
					{
						MessageBox.Show("Subject is not allowed for this message type");
						return null;
					}
					return new TweetFactory(header, body, this.textSpeak, this.hashtagList, this.mentionList).GetMessage();
				default:
					MessageBox.Show("Invalid Message Type in Header");
					return null;
			}
		}

		private IMessageService ProcessMessage(IMessageService message, string header, string body)
		{
			try
			{
				message.Process();
				if (message.GetType() == typeof(EmailMessage))
				{
					string subject = "";

					HashSet<Tuple<string, string>> output = message.GetOutput();
					foreach (Tuple<string, string> x in output)
					{
						if (x.Item1 == "subject")
						{
							if (x.Item2.Contains("SIR"))
							{
							subject = x.Item2.Substring(0, 12);
							}
						}
					}

					Regex sir = new Regex(@"(SIR\s\d{2}\/\d{2}\/\d{2})");
					Match match = sir.Match(subject);
					if (match.Success)
					{
						message = new SIRMessage(header, body);
						message.Process();
					}
					return message;
				}
				else
				{
					return message;
				}
			}
			catch (MessageValidationException ex)
			{
				MessageBox.Show(ex.ErrorMessage);
				return null;
			}
			catch (Exception ex)
			{
				MessageBox.Show("There was an error with your message: " + ex.ToString());
				return null;
			}
		}

		private void ProcessMessageButton_Click(object sender, RoutedEventArgs e)
		{
			//Create Appropriate Message Object based on message header
			IMessageService message = GetMessage(MessageHeader.Text, MessageBody.Text);
			if (message != null)
			{
				message = ProcessMessage(message, MessageHeader.Text, MessageBody.Text);
			}
			else
			{
				return;
			}

			if (message != null)
			{
				PopulateOutput(message);
				try
				{
					WriteToFile(message);
				}
				catch (Exception ex)
				{
					MessageBox.Show("There was an error writing this message to a file: " + ex.Message);
				}
			}
		}

		private void ProcessFileButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				string line;
				using (var reader = new System.IO.StreamReader(@PathInput.Text))
				{
					while ((line = reader.ReadLine()) != null)
					{
						string[] values = line.Split(';');
						IMessageService message = GetMessage(values[0], values[1]);
						if(message != null)
						{
							message = ProcessMessage(message, values[0], values[1]);
							if (message != null)
							{
								MessageBox.Show(message.ToString());
							}
						}
					}
				}
			}catch(Exception ex)
			{
				MessageBox.Show("There was a problem reading that message file: "+ex.Message);
			}
		}

		public bool ContainsSubject(string body)
		{
			if (body.ToLower().Contains("subject: "))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public void WriteToFile(IMessageService message)
		{
			Type type = message.GetType();
			DataContractJsonSerializer js = new DataContractJsonSerializer(type);
			using (var stream = File.Create(@"C:\Users\roryh\source\repos\ELM\json\"+message.Header+".json"))
			{
				using (var writer = JsonReaderWriterFactory.CreateJsonWriter(stream))
				{
					js.WriteObject(writer, message);
					writer.Flush();
				}
			}
		}

		public void PopulateOutput(IMessageService message)
		{
			HashSet<Tuple<string, string>> output = message.GetOutput();
			foreach (Tuple<string, string> x in output)
			{
				switch (x.Item1)
				{
					case ("sender"):
						OutputSender.Text = x.Item2;
						break;
					case ("subject"):
						OutputSubject.Text = x.Item2;
						break;
					case ("message"):
						OutputMessage.Text = x.Item2;
						break;
					default:
						break;
				}
			}
		}
	}
}
