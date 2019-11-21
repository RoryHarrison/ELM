using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
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
			textSpeak = readCSV(@"C:\Users\roryh\source\repos\ELM\Data\textwords.csv");
			hashtagList = readCSV(@"C:\Users\roryh\source\repos\ELM\Data\hashtags.csv");
			mentionList = readCSV(@"C:\Users\roryh\source\repos\ELM\Data\mentions.csv");
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

		private void ProcessMessageButton_Click(object sender, RoutedEventArgs e)
		{

			//Validation
			if (MessageHeader == null || MessageBody == null)
			{
				MessageBox.Show("Please Enter a Message Header and Body");
				return;
			}

			if(!(MessageHeader.Text.Length==10))
			{
				MessageBox.Show("Invalid Message Header Size");
				return;
			}

			if (!MessageBody.Text.ToLower().Contains("sender: ") || !MessageBody.Text.ToLower().Contains("message: "))
			{
				MessageBox.Show("Please provide a sender and message within the mssage body");
				return;
			}
			//Validation End

			//Create Appropriate Message Object based on message header
			IMessageService message = null;
			switch (MessageHeader.Text[0])
			{
				case ('S'):
					if(ContainsSubject())
					{
						MessageBox.Show("Subject is not allowed for this message type");
						return;
					}
					message = new SMSFactory(MessageHeader.Text, MessageBody.Text, this.textSpeak).GetMessage();
					break;
				case ('E'):
					if (!ContainsSubject())
					{
						MessageBox.Show("Subject is required for this message type");
					}
					message = new EmailFactory(MessageHeader.Text, MessageBody.Text).GetMessage();
					break;
				case ('T'):
					if (ContainsSubject())
					{
						MessageBox.Show("Subject is not allowed for this message type");
						return;
					}
					message = new TweetFactory(MessageHeader.Text, MessageBody.Text, this.textSpeak, this.hashtagList, this.mentionList).GetMessage();
					break;
				default:
					MessageBox.Show("Invalid Message Type in Header");
					break;
			}

			try
			{
				message.Process();
			}catch(MessageValidationException ex)
			{
				MessageBox.Show(ex.ErrorMessage);
				return;
			}catch(Exception ex)
			{
				MessageBox.Show("There was an error with your message: "+ex.Message);
				return;
			}

			PopulateOutput(message);
			try
			{
				WriteToFile(message);
			}catch(Exception ex)
			{
				MessageBox.Show("There was an error writing this message to a file: "+ex.Message);
			}

			//Write to File
		}

		public bool ContainsSubject()
		{
			if (MessageBody.Text.ToLower().Contains("subject: "))
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
