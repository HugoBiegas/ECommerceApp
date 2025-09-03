using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ECommerceApp.Models;
using ECommerceApp.Models.Enums;


namespace ECommerceApp.TagHelpers
{
    [HtmlTargetElement("role-badge")]
    public class RoleBadgeTagHelper : TagHelper
    {
        public UserRole Role { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";

            var (badgeClass, icon, text) = Role switch
            {
                UserRole.Admin => ("badge bg-danger", "fas fa-crown", "Admin"),
                UserRole.Librarian => ("badge bg-primary", "fas fa-book", "Libraire"),
                UserRole.User => ("badge bg-secondary", "fas fa-user", "Utilisateur"),
                _ => ("badge bg-secondary", "fas fa-question", "Inconnu")
            };

            output.Attributes.SetAttribute("class", badgeClass);
            output.Content.SetHtmlContent($"<i class=\"{icon} me-1\"></i>{text}");
        }
    }

}
