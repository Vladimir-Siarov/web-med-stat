using System;
using MedStat.Core.DAL;
using Microsoft.Extensions.Logging;

namespace MedStat.Core.Repositories
{
	public abstract class BaseRepository
	{
		protected ILogger Logger { get; }

		protected MedStatDbContext DbContext { get; }

		protected string UserUid { get; }


		protected BaseRepository(MedStatDbContext dbContext, ILogger logger, string userUid)
		{
			this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			this.DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			this.UserUid = userUid;
		}
	}
}
