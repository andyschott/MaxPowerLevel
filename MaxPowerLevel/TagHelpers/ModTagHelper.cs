using MaxPowerLevel.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace MaxPowerLevel.TagHelpers
{
    [HtmlTargetElement("mod", TagStructure = TagStructure.WithoutEndTag)]
    public class ModTagHelper : TagHelper
    {
        public ModData Mod { get; set; }

        private const string ModClass = "modIcon";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "img";
            output.Attributes.SetAttribute("src", Mod.IconUrl);
            output.Attributes.SetAttribute("class", GetCssClass());
        }

        private string GetCssClass()
        {
            var elementClass = Mod.Element switch
            {
                ModElement.Arc => "arc",
                ModElement.Solar => "solar",
                ModElement.Void => "void",
                _ => "general"
            };

            var collectionsClass = Mod.IsUnlocked ? string.Empty : "locked";
            return string.Join(' ', ModClass, elementClass, collectionsClass);
        }
    }
}