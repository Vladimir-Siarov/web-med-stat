using System.Text.RegularExpressions;

namespace MedStat.Core.Helpers
{
	public static class PhoneHelper
	{
		public static string NormalizePhoneNumber(string number)
		{
			number = Regex.Replace(number, @"[^\d|\+]", "");

			// TODO: It's valid for Russia only! 
			// 89112299153 -> +79112299153
			if (number.Length == 11 && number.StartsWith('8'))
			{
				number = "+7" + number.Substring(1);
			}

			return number;
		}
	}
}
