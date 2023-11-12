#nullable disable

#region

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

#endregion

namespace ToDoList.Utility.CustomTagHelpers;

[HtmlTargetElement("navbar-link")]
public class NavLink : TagHelper
{
    private readonly IUrlHelperFactory _urlHelperFactory;

    public NavLink(IUrlHelperFactory urlHelperFactory)
    {
        _urlHelperFactory = urlHelperFactory;
    }

    [HtmlAttributeName("asp-area")] public string Area { get; set; }

    [HtmlAttributeName("asp-controller")] public string Controller { get; set; }

    [HtmlAttributeName("asp-action")] public string Action { get; set; }

    [HtmlAttributeName("asp-page")] public string Page { get; set; }

    [ViewContext] [HtmlAttributeNotBound] public ViewContext ViewContext { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        base.Process(context, output);

        if (Area == null)
            throw new Exception();

        if (Page != null && Action == null && Controller == null)
            ProcessForPage(_urlHelperFactory.GetUrlHelper(ViewContext), output);
        else if (Action != null && Controller != null && Page == null)
            ProcessForView(_urlHelperFactory.GetUrlHelper(ViewContext), output);
        else
            throw new Exception();
    }

    private void ProcessForPage(IUrlHelper urlHelper, TagHelperOutput output)
    {
        var url = urlHelper.Page(Page, new { area = Area });
        var isActive = ViewContext.RouteData.Values["page"] != null &&
                       ViewContext.RouteData.Values["page"].ToString().StartsWith(Page.Replace("/Index", ""));
        ;
        BuildHtml(url, isActive, output);
    }

    private void ProcessForView(IUrlHelper urlHelper, TagHelperOutput output)
    {
        var url = urlHelper.Action(Action, Controller, new { area = Area });
        var isActive = ViewContext.RouteData.Values["controller"] != null &&
                       ViewContext.RouteData.Values["action"] != null &&
                       ViewContext.RouteData.Values["controller"]?.ToString() == Controller
                       && ViewContext.RouteData.Values["action"]?.ToString() == Action;

        BuildHtml(url, isActive, output);
    }

    private static void BuildHtml(string url, bool isActive, TagHelperOutput output)
    {
        output.TagName = "li";

        output.Attributes.SetAttribute("class", isActive ? "nav-item active" : "nav-item");

        TagBuilder linkTag = new("a");
        linkTag.Attributes.Add("href", url);
        linkTag.AddCssClass("nav-link text-white");
        linkTag.InnerHtml.AppendHtml(output.GetChildContentAsync().Result);

        output.Content.SetHtmlContent(linkTag);
    }
}