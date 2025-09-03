using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ECommerceApp.Models;
using ECommerceApp.Models.Enums;


namespace ECommerceApp.TagHelpers
{
    [HtmlTargetElement("cart-summary")]
    public class CartSummaryTagHelper : TagHelper
    {
        public int ItemsCount { get; set; }
        public decimal TotalAmount { get; set; }
        public bool ShowDetails { get; set; } = false;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "cart-summary");

            if (ItemsCount == 0)
            {
                output.Content.SetHtmlContent(@"
                    <div class=""text-center text-muted"">
                        <i class=""fas fa-shopping-cart fa-2x mb-2""></i>
                        <p>Votre panier est vide</p>
                    </div>");
                return;
            }

            var detailsHtml = ShowDetails ? $@"
                <div class=""mt-2"">
                    <small class=""text-muted"">Total: {TotalAmount:C}</small>
                </div>" : "";

            var itemText = ItemsCount == 1 ? "article" : "articles";

            output.Content.SetHtmlContent($@"
                <div class=""d-flex align-items-center"">
                    <i class=""fas fa-shopping-cart text-primary me-2""></i>
                    <span class=""fw-bold"">{ItemsCount} {itemText}</span>
                    {detailsHtml}
                </div>");
        }
    }

}
