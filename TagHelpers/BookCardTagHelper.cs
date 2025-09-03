using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using ECommerceApp.Models;
using ECommerceApp.Models.Enums;


namespace ECommerceApp.TagHelpers
{
    [HtmlTargetElement("book-card")]
    public class BookCardTagHelper : TagHelper
    {
        public Book Book { get; set; } = null!;
        public bool ShowManageButtons { get; set; } = false;
        public bool ShowAddToCart { get; set; } = true;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "col-lg-3 col-md-4 col-sm-6 mb-4");

            var imageUrl = !string.IsNullOrEmpty(Book.ImageUrl) ? Book.ImageUrl : "/images/book-placeholder.jpg";
            var availabilityBadge = Book.InStock ?
                "<span class=\"badge bg-success\">Disponible</span>" :
                "<span class=\"badge bg-danger\">Rupture de stock</span>";

            var manageButtons = ShowManageButtons ? $@"
                <div class=""mt-2"">
                    <a href=""/Books/Edit/{Book.Id}"" class=""btn btn-outline-primary btn-sm"">
                        <i class=""fas fa-edit""></i> Modifier
                    </a>
                    <a href=""/Books/Delete/{Book.Id}"" class=""btn btn-outline-danger btn-sm"">
                        <i class=""fas fa-trash""></i> Supprimer
                    </a>
                </div>" : "";

            var addToCartButton = ShowAddToCart && Book.InStock ? $@"
                <button class=""btn btn-primary w-100 add-to-cart-btn"" data-book-id=""{Book.Id}"">
                    <i class=""fas fa-shopping-cart""></i> Ajouter au panier
                </button>" : "";

            output.Content.SetHtmlContent($@"
                <div class=""card h-100 shadow-sm"">
                    <div class=""position-relative"">
                        <img src=""{imageUrl}"" class=""card-img-top"" alt=""{Book.Title}"" style=""height: 250px; object-fit: cover;"">
                        <div class=""position-absolute top-0 end-0 m-2"">
                            {availabilityBadge}
                        </div>
                    </div>
                    <div class=""card-body d-flex flex-column"">
                        <h5 class=""card-title"">{Book.Title}</h5>
                        <p class=""card-text text-muted"">par {Book.Author?.Name ?? "Auteur inconnu"}</p>
                        <p class=""card-text""><small class=""text-muted"">{Book.Category}</small></p>
                        <div class=""mt-auto"">
                            <div class=""d-flex justify-content-between align-items-center mb-2"">
                                <span class=""h5 text-primary mb-0"">{Book.Price:C}</span>
                                <small class=""text-muted"">Stock: {Book.Stock}</small>
                            </div>
                            <a href=""/Books/Details/{Book.Id}"" class=""btn btn-outline-secondary w-100 mb-2"">
                                <i class=""fas fa-info-circle""></i> Voir détails
                            </a>
                            {addToCartButton}
                            {manageButtons}
                        </div>
                    </div>
                </div>");
        }
    }
}
