using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ECommerceApp.Models;
using ECommerceApp.Models.Enums;

namespace ECommerceApp.TagHelpers
{
    [HtmlTargetElement("credit-display")]
    public class CreditDisplayTagHelper : TagHelper
    {
        public decimal Amount { get; set; }
        public string Size { get; set; } = "normal"; // normal, large, small
        public bool ShowIcon { get; set; } = true;
        public string AdditionalClass { get; set; } = "";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";

            var sizeClass = Size switch
            {
                "large" => "h4",
                "small" => "small",
                _ => ""
            };

            var iconHtml = ShowIcon ? "<i class=\"fas fa-coins text-warning me-1\"></i>" : "";

            output.Attributes.SetAttribute("class", $"credit-amount {sizeClass} {AdditionalClass}".Trim());
            output.Content.SetHtmlContent($"{iconHtml}{Amount:C}");
        }
    }

}
