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


		[ProtectedPersonalData]
		[Required(ErrorMessage = Localizer.DataAnnotations.RequiredErrorMessage)]
		[Phone(ErrorMessage = Localizer.DataAnnotations.InvalidPhoneNumber)]
		[StringLength(20)]
		[Display(Name = "Mobile phone number")] // "Номер мобильного телефона"
		public override string PhoneNumber { get; set; }

		[Required]
		[StringLength(20)]
		public string NormalizedPhoneNumber { get; set; }


		public bool IsPasswordChangeRequired { get; set; }
	}
}
