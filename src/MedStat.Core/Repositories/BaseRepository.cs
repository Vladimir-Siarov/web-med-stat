using System;
using System.Reflection;
using System.Resources;
using MedStat.Core.DAL;
using Microsoft.Extensions.Logging;

namespace MedStat.Core.Repositories
{
	public abstract class BaseRepository
	{
		protected ILogger Logger { get; }

		protected MedStatDbContext DbContext { get; }

		protected string UserUid { get; }


		private ResourceManager _messagesManager;
		protected ResourceManager MessagesManager
		{
			get
			{
				if (_messagesManager == null)
				{
					_messagesManager = new ResourceManager("MedStat.Core.Resources.Messages",
						Assembly.GetExecutingAssembly());
				}

				return _messagesManager;
			}
		}


		protected BaseRepository(MedStatDbContext dbContext, ILogger logger, string userUid)
		{
			this.Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			this.DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
			this.UserUid = userUid;
		}
	}
}
