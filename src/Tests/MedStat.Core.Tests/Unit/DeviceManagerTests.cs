using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using FluentAssertions;
using MedStat.Core.Helpers;
using Xunit;

namespace MedStat.Core.Tests.Unit
{
	public class DeviceManagerTests
	{
		[Theory]
		[InlineData("AA-BB-CC-DD-00-11", "AABBCCDD0011")]
		[InlineData("AA:BB:CC:DD:00:22", "AABBCCDD0022")]
		[InlineData("4 abCD',[]~!`@#$%^&*()_= 55-66", "4abCD5566")]
		public void NormalizeMacAddress(string mac, string expectedNormalizedMac)
		{
			// Act
			var normalizedMac = DeviceManager.NormalizeMacAddress(mac);


			// Assert:
			
			normalizedMac.Should().BeEquivalentTo(expectedNormalizedMac);

			// We don't have a more accurate MAC address validation in this method now,
			// but output normalized address should contain only alphanumeric.
			var match = Regex.Match(normalizedMac, @"[^\d|\a-zA-Z]");
			match.Success.Should().BeFalse("Phone mac shouldn't contains any symbols except digital and plus");
		}
	}
}
