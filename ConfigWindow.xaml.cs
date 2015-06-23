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
using System.Windows.Shapes;

namespace EBunnyChat
{
	/// <summary>
	/// Interaction logic for ConfigWindow.xaml
	/// </summary>
	public partial class ConfigWindow : Window
	{
		public string Server { get; set; }
		public string Username { get; set; }
		public string Email { get; set; }

		public ConfigWindow()
		{
			InitializeComponent();

			Server = "192.168.169.33";
		}

		private void BtnGo_OnClick(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}
