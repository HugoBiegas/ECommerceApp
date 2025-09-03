using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ECommerceApp.Models;
using ECommerceApp.Models.Enums;


namespace ECommerceApp.TagHelpers
{
    [HtmlTargetElement("order-status")]
    public class OrderStatusBadgeTagHelper : TagHelper
    {
        public OrderStatus Status { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";

            var (badgeClass, icon) = Status switch
            {
                OrderStatus.Pending => ("badge bg-warning text-dark", "fas fa-clock"),
                OrderStatus.Processing => ("badge bg-info", "fas fa-cog fa-spin"),
                OrderStatus.Shipped => ("badge bg-primary", "fas fa-shipping-fast"),
                OrderStatus.Delivered => ("badge bg-success", "fas fa-check-circle"),
                OrderStatus.Cancelled => ("badge bg-danger", "fas fa-times-circle"),
                _ => ("badge bg-secondary", "fas fa-question")
            };

            output.Attributes.SetAttribute("class", badgeClass);
            output.Content.SetHtmlContent($"<i class=\"{icon} me-1\"></i>{Status}");
        }
    }

}
