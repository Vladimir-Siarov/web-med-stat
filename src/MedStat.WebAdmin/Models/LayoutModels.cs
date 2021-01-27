
namespace MedStat.WebAdmin.Models
{
	public interface IPopupLayoutData
	{
		bool EntityWasUpdated { get; set; }

		bool EntityWasCreated { get; set; }

		bool EntityWasDeleted { get; set; }
	}

	// Model for "_LayoutStaticResourcesPartial.cshtml" partial.
	public class LayoutStaticResourcesModel
	{
		public enum EnResourcesType
		{
			Scripts,
			CssFiles
		}

		public EnResourcesType Type { get; }

		public bool IsPopupLayout { get; }


		public LayoutStaticResourcesModel(EnResourcesType type, bool isPopupLayout = false)
		{
			this.Type = type;
			this.IsPopupLayout = isPopupLayout;
		}
	}
}
