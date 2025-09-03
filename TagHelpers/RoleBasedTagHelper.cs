using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ECommerceApp.Models;
using ECommerceApp.Models.Enums;


namespace ECommerceApp.TagHelpers
{
    [HtmlTargetElement("*", Attributes = "asp-role")]
    public class RoleBasedTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-role")]
        public UserRole RequiredRole { get; set; }

        [ViewContext]
        public ViewContext ViewContext { get; set; } = null!;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // Récupérer le rôle utilisateur depuis la session
            var session = ViewContext.HttpContext.Session;
            var userRoleString = session.GetString("UserRole");

            if (string.IsNullOrEmpty(userRoleString))
            {
                // Utilisateur non connecté, masquer l'élément
                output.SuppressOutput();
                return;
            }

            if (Enum.TryParse<UserRole>(userRoleString, out var userRole))
            {
                // Vérifier si l'utilisateur a le rôle requis ou supérieur
                if ((int)userRole < (int)RequiredRole)
                {
                    output.SuppressOutput();
                }
            }
            else
            {
                // Rôle invalide, masquer l'élément
                output.SuppressOutput();
            }
        }
    }

}
