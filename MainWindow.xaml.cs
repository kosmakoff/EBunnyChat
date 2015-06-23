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

namespace EBunnyChat
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			// config
			ConfigWindow configWindow;

			while (true)
			{
				configWindow = new ConfigWindow();
				if (configWindow.ShowDialog() == true)
					break;
				
				MessageBox.Show("You better input some correct data");
			}

			ChatClient.Instance.Config(configWindow.Server, configWindow.Username, configWindow.Email);

			ChatClient.Instance.Start();

			Closing += (sender, args) => { ChatClient.Instance.Stop(); };

			ChatClient.Instance.MessageArrived += InstanceOnMessageArrived;
		}

		private void InstanceOnMessageArrived(object sender, ChatMessage chatMessage)
		{
			Dispatcher.Invoke(() =>
			{
				LstChatEntries.Items.Add(chatMessage);
				LstChatEntries.ScrollIntoView(chatMessage);
				
			});
		}

		private void BtnSendText_OnClick(object sender, RoutedEventArgs e)
		{
			SendMessage();
		}

		private void SendMessage()
		{
			ChatClient.Instance.SendMessage(new ChatMessage
			{
				Email = ChatClient.Instance.LocalUserEmail,
				Username = ChatClient.Instance.LocalUserName,
				Text = TxtTheTextToSend.Text
			});

			TxtTheTextToSend.Clear();
			TxtTheTextToSend.Focus();
		}

		private void TxtTheTextToSend_OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
				{
					TxtTheTextToSend.Text += "\n";
					TxtTheTextToSend.SelectionStart = TxtTheTextToSend.Text.Length;
				}
				else
					SendMessage();
			}
		}
	}
}
