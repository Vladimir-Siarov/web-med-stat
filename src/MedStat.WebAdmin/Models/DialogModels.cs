
namespace MedStat.WebAdmin.Models
{
	/// <summary>
	/// Define data model for "_DeleteConfirmPartial.cshtml" partial
	/// </summary>
	public class DeleteConfirmationModel
	{
		public string ControlId { get; set; }

		public string ConfirmMessage { get; set; }


		// Ajax Form properties:

		public string AjaxUrl { get; set; }

		public string AjaxSuccess { get; set; }

		public string AjaxFailed { get; set; }


		// Parent "ConfirmCommand" control properties

		public string ConfirmCommandCtrlId { get; set; }
		public string ConfirmCommandCtrlName { get; set; }
	}
}
