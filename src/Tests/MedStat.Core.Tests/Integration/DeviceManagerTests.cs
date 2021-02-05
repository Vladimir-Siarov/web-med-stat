using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using MedStat.Core.BE.Device;
using MedStat.Core.Helpers;
using Xunit;

namespace MedStat.Core.Tests.Integration
{
	public class DeviceManagerTests
	{
		[Fact]
		public void GetDeviceModels()
		{
			// Act
			var deviceModels = DeviceManager.GetDeviceModels();


			// Assert:

			deviceModels.Should().NotBeNull();
			
			Assert.Contains(deviceModels, m => m.Type == EnDeviceType.SmartWatch);
			Assert.Contains(deviceModels, m => m.Type == EnDeviceType.Gateway);
		}
	}
}
