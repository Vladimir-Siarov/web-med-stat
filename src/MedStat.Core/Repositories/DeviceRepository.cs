using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MedStat.Core.BE.Device;
using MedStat.Core.DAL;
using MedStat.Core.Helpers;
using MedStat.Core.Info;
using MedStat.Core.Info.Company;
using MedStat.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MedStat.Core.Repositories
{
	public partial class DeviceRepository : BaseRepository, IDeviceRepository
	{
		public DeviceRepository(MedStatDbContext dbContext, 
			ILogger<CompanyRepository> logger, string userUid)
			: base(dbContext, logger, userUid)
		{
		}
	}
}
