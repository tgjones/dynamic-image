using System.Web.Mvc;
using System.Web.Mvc.Html;
using SoundInTheory.DynamicImage.Fluent;
using System.Web;

namespace SoundInTheory.DynamicImage.Website.Html
{
	public static class HtmlHelperExtensions
	{
		public static MvcHtmlString NavListItem(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName)
		{
			var tagBuilder = new TagBuilder("li");

			var url = new UrlHelper(htmlHelper.ViewContext.RequestContext).Action(actionName, controllerName);
			var currentUrl = htmlHelper.ViewContext.HttpContext.Request.RawUrl;
			if (currentUrl.StartsWith(url))
				tagBuilder.AddCssClass("active");
			tagBuilder.InnerHtml = htmlHelper.ActionLink(linkText, actionName, controllerName).ToString();
			return MvcHtmlString.Create(tagBuilder.ToString());
		}

		public static MvcHtmlString ResizedImageTag(this HtmlHelper htmlHelper, string imageHref, int width)
		{
			var tagBuilder = new TagBuilder("img");
			tagBuilder.Attributes["src"] = new CompositionBuilder()
				.WithLayer(
					LayerBuilder.Image.SourceFile(imageHref).WithFilter(
						FilterBuilder.Resize.ToWidth(width)
						)
				).Url;
			return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.SelfClosing));
		}
	}
}