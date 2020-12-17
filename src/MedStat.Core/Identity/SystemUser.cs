using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using MedStat.Core.Resources;

namespace MedStat.Core.Identity
{
	public class SystemUser : IdentityUser<Int32>
	{
		[Required(ErrorMessage = Localizer.DataAnnotations.RequiredErrorMessage)]
		[StringLength(20)]
		[Display(Name = "FirstName")] // "Имя"
		public string FirstName { get; set; }

		[Required(ErrorMessage = Localizer.DataAnnotations.RequiredErrorMessage)]
		[StringLength(20)]
		[Display(Name = "Surname")] // "Фамилия"
		public string Surname { get; set; }

		[StringLength(20)]
		[Display(Name = "Patronymic")] // "Отчество"
		public string Patronymic { get; set; }
	}
}
