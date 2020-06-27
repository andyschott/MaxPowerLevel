using MaxPowerLevel.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace MaxPowerLevel.TagHelpers
{
    [HtmlTargetElement("mod", TagStructure = TagStructure.WithoutEndTag)]
    public class ModTagHelper : TagHelper
    {
        public ModData Mod { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "img";
            output.Attributes.SetAttribute("src", Mod.IconUrl);
            output.Attributes.SetAttribute("class", GetCssClass(Mod.Element));
        }

        private static string GetCssClass(ModElement element)
        {
            return element switch
            {
                ModElement.Arc => "arc",
                ModElement.Solar => "solar",
                ModElement.Void => "void",
                _ => "general"
            };
        }
    }
}