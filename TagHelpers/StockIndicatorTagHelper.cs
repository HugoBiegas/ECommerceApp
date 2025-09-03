using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ECommerceApp.Models;
using ECommerceApp.Models.Enums;

namespace ECommerceApp.TagHelpers
{
    [HtmlTargetElement("stock-indicator")]
    public class StockIndicatorTagHelper : TagHelper
    {
        public int Stock { get; set; }
        public bool IsAvailable { get; set; } = true;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "stock-indicator");

            if (!IsAvailable)
            {
                output.Content.SetHtmlContent(@"
                    <div class=""alert alert-danger alert-sm mb-0"">
                        <i class=""fas fa-times-circle me-1""></i>
                        Indisponible
                    </div>");
                return;
            }

            var (alertClass, icon, message) = Stock switch
            {
                0 => ("alert-danger", "fas fa-times-circle", "Rupture de stock"),
                <= 5 => ("alert-warning", "fas fa-exclamation-triangle", $"Stock faible ({Stock} restants)"),
                _ => ("alert-success", "fas fa-check-circle", $"En stock ({Stock} disponibles)")
            };

            output.Content.SetHtmlContent($@"
                <div class=""alert {alertClass} alert-sm mb-0"">
                    <i class=""{icon} me-1""></i>
                    {message}
                </div>");
        }
    }

}
