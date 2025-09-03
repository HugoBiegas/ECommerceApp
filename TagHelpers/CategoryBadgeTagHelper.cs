using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ECommerceApp.Models;
using ECommerceApp.Models.Enums;


namespace ECommerceApp.TagHelpers
{
    [HtmlTargetElement("category-badge")]
    public class CategoryBadgeTagHelper : TagHelper
    {
        public BookCategory Category { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";

            var (badgeClass, icon) = Category switch
            {
                BookCategory.Fiction => ("badge bg-purple", "fas fa-magic"),
                BookCategory.NonFiction => ("badge bg-info", "fas fa-newspaper"),
                BookCategory.Science => ("badge bg-success", "fas fa-atom"),
                BookCategory.History => ("badge bg-warning text-dark", "fas fa-history"),
                BookCategory.Biography => ("badge bg-dark", "fas fa-user-circle"),
                BookCategory.Mystery => ("badge bg-secondary", "fas fa-search"),
                BookCategory.Romance => ("badge bg-pink", "fas fa-heart"),
                BookCategory.Fantasy => ("badge bg-primary", "fas fa-dragon"),
                BookCategory.SelfHelp => ("badge bg-light text-dark", "fas fa-lightbulb"),
                BookCategory.Technology => ("badge bg-cyan", "fas fa-laptop-code"),
                BookCategory.Business => ("badge bg-orange", "fas fa-briefcase"),
                BookCategory.Art => ("badge bg-gradient", "fas fa-palette"),
                _ => ("badge bg-secondary", "fas fa-book")
            };

            output.Attributes.SetAttribute("class", badgeClass);
            output.Content.SetHtmlContent($"<i class=\"{icon} me-1\"></i>{Category}");
        }
    }

}
