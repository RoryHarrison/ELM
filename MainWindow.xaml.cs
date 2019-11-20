using System;
using System.Collections.Generic;
using System.Linq;
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
		public MainWindow()
		{
			InitializeComponent();
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
					message = new SMSFactory(MessageHeader.Text, MessageBody.Text).GetMessage();
					break;
				case ('E'):
					message = new EmailFactory(MessageHeader.Text, MessageBody.Text).GetMessage();
					break;
				case ('T'):
					message = new TweetFactory(MessageHeader.Text, MessageBody.Text).GetMessage();
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
