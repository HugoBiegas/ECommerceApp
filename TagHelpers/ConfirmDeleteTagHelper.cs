using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Antiforgery;

namespace ECommerceApp.TagHelpers
{
    [HtmlTargetElement("confirm-delete")]
    public class ConfirmDeleteTagHelper : TagHelper
    {
        public string ItemName { get; set; } = "cet élément";
        public string DeleteUrl { get; set; } = "#";
        public string ButtonText { get; set; } = "Supprimer";
        public string ButtonClass { get; set; } = "btn btn-danger";
        public string ModalId { get; set; } = "deleteModal";

        [ViewContext]
        public ViewContext ViewContext { get; set; } = null!;

        private readonly IAntiforgery _antiforgery;

        public ConfirmDeleteTagHelper(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = null; // Remove the wrapper tag

            // FIXED: Generate antiforgery token properly
            var tokens = _antiforgery.GetAndStoreTokens(ViewContext.HttpContext);
            var tokenInput = $"<input type=\"hidden\" name=\"{tokens.HeaderName}\" value=\"{tokens.RequestToken}\" />";

            output.Content.SetHtmlContent($@"
                <button type=""button"" class=""{ButtonClass}"" data-bs-toggle=""modal"" data-bs-target=""#{ModalId}"">
                    <i class=""fas fa-trash""></i> {ButtonText}
                </button>

                <div class=""modal fade"" id=""{ModalId}"" tabindex=""-1"" aria-hidden=""true"">
                    <div class=""modal-dialog"">
                        <div class=""modal-content"">
                            <div class=""modal-header"">
                                <h5 class=""modal-title"">Confirmer la suppression</h5>
                                <button type=""button"" class=""btn-close"" data-bs-dismiss=""modal"" aria-label=""Close""></button>
                            </div>
                            <div class=""modal-body"">
                                <div class=""d-flex align-items-center"">
                                    <i class=""fas fa-exclamation-triangle text-warning me-3"" style=""font-size: 2rem;""></i>
                                    <div>
                                        <p class=""mb-1"">Êtes-vous sûr de vouloir supprimer {ItemName} ?</p>
                                        <small class=""text-muted"">Cette action est irréversible.</small>
                                    </div>
                                </div>
                            </div>
                            <div class=""modal-footer"">
                                <button type=""button"" class=""btn btn-secondary"" data-bs-dismiss=""modal"">
                                    <i class=""fas fa-times""></i> Annuler
                                </button>
                                <form method=""post"" action=""{DeleteUrl}"" class=""d-inline"">
                                    {tokenInput}
                                    <button type=""submit"" class=""btn btn-danger"">
                                        <i class=""fas fa-trash""></i> Confirmer la suppression
                                    </button>
                                </form>
                            </div>
                        </div>
                    </div>
                </div>");
        }
    }
}