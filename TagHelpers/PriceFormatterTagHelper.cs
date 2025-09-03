using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ECommerceApp.Models;
using ECommerceApp.Models.Enums;


namespace ECommerceApp.TagHelpers
{
    [HtmlTargetElement("price-display")]
    public class PriceFormatterTagHelper : TagHelper
    {
        public decimal Price { get; set; }
        public string Size { get; set; } = "normal"; // small, normal, large
        public bool ShowCurrency { get; set; } = true;
        public string DiscountPrice { get; set; } = "";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";

            var sizeClass = Size switch
            {
                "large" => "h4",
                "small" => "small",
                _ => "h5"
            };

            output.Attributes.SetAttribute("class", $"price-display {sizeClass} text-primary mb-0");

            var priceContent = ShowCurrency ? Price.ToString("C") : Price.ToString("F2");

            if (!string.IsNullOrEmpty(DiscountPrice) && decimal.TryParse(DiscountPrice, out var discountValue))
            {
                var originalPrice = ShowCurrency ? discountValue.ToString("C") : discountValue.ToString("F2");
                output.Content.SetHtmlContent($@"
                    <span class=""text-decoration-line-through text-muted me-2"">{originalPrice}</span>
                    <span class=""text-danger fw-bold"">{priceContent}</span>");
            }
            else
            {
                output.Content.SetHtmlContent(priceContent);
            }
        }
    }
}
