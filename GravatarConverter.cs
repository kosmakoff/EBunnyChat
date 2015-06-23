using System;
using System.Globalization;
using System.Windows.Data;

namespace EBunnyChat
{
	public class GravatarConverter : IValueConverter
	{
		public int? Size { get; set; }

		#region Implementation of IValueConverter

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var email = (string) value;
			return GravatarUtils.GetGravatarUri(email, Size);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
