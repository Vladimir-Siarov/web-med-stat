using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using MedStat.Core.BE.Device;
using MedStat.Core.DAL;
using MedStat.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace MedStat.Core.Tests.Repositories
{
	public class DeviceModelsRepositoryTests : BaseRepositoryTests, IClassFixture<DatabaseFixture>
	{
		public DeviceModelsRepositoryTests(ITestOutputHelper outputHelper, DatabaseFixture fixture)
			: base(outputHelper, fixture)
		{
		}


		// Get

		[Theory]
		[InlineData(EnDeviceType.Gateway)]
		[InlineData(EnDeviceType.SmartWatch)]
		public void GetDeviceModelsByType(EnDeviceType type)
		{
			// Arrange:

			// All device models are defined in JSON file.


			// Act:

			DeviceModel[] models = null;
			using (var dbContext = new MedStatDbContext(this.Fixture.ContextOptions))
			{
				var sp = this.GetServiceProvider(dbContext);
				var dvRepository = sp.GetRequiredService<IDeviceModelsRepository>();

				models = dvRepository.GetDeviceModelsByType(type).ToArray();
			}


			// Assert:

			models.Should().NotBeNull();

			Assert.True(models.Length > 0);
			Assert.True(models.All(m => m.Type == type));
		}


		// TODO: Tests for FindDeviceModels
	}
}
