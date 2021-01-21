﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace MedStat.Core.Identity
{
	public class IdentityResultException : Exception
	{
		public IdentityResultException(IdentityResult result, string mainExMessage)
			: base(mainExMessage, FormatIdentityException(result))
		{
			if(result == null)
				throw new ArgumentNullException(nameof(result));
		}


		private static Exception FormatIdentityException(IdentityResult result)
		{
			return
				result?.Errors?.FirstOrDefault() != null
					? new Exception(result.Errors.FirstOrDefault().Description)
					: null;
		}
	}
}
