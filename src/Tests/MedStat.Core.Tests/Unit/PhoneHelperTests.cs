using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using FluentAssertions;
using MedStat.Core.Helpers;
using Xunit;

namespace MedStat.Core.Tests.Unit
{
	public class PhoneHelperTests
	{
		[Theory]
		[InlineData("89112223344", "+79112223344")] // convert "8" to the Russia international code
		[InlineData("8 (911) 333-44-55", "+79113334455")]
		[InlineData("4 aaa',[]~!`@#$%^&*()_= 44-55-66", "4445566")]
		public void NormalizePhoneNumber(string number, string expectedNormalizedNumber)
		{
			// Act
			var normalizedNumber = PhoneHelper.NormalizePhoneNumber(number);


			// Assert:
			
			normalizedNumber.Should().BeEquivalentTo(expectedNormalizedNumber);

			// We don't have a phone validation in this method now,
			// but output should contain only digital and plus char.
			var match = Regex.Match(normalizedNumber, @"[^\d|\+]");
			match.Success.Should().BeFalse("Phone number shouldn't contains any symbols except digital and plus");
		}
	}
}
