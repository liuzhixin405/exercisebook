#pragma checksum "G:\Users\GitHub\exercisebook\MicroService\MicroService\WebApi.TestConsul\Views\TestOne\GetListInConsul.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "41321f8696c25122626c79026485ee16de485438"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_TestOne_GetListInConsul), @"mvc.1.0.view", @"/Views/TestOne/GetListInConsul.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "G:\Users\GitHub\exercisebook\MicroService\MicroService\WebApi.TestConsul\Views\_ViewImports.cshtml"
using WebApi.TestConsul;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "G:\Users\GitHub\exercisebook\MicroService\MicroService\WebApi.TestConsul\Views\_ViewImports.cshtml"
using WebApi.TestConsul.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"41321f8696c25122626c79026485ee16de485438", @"/Views/TestOne/GetListInConsul.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"2709e41851d884eb7fae37821fda535d6dc0afe6", @"/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Views_TestOne_GetListInConsul : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<IEnumerable<WebApi.TestConsul.Models.User>>
    #nullable disable
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "Create", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 3 "G:\Users\GitHub\exercisebook\MicroService\MicroService\WebApi.TestConsul\Views\TestOne\GetListInConsul.cshtml"
  
    ViewData["Title"] = "UserList";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<h1>UserList</h1>\r\n\r\n<p>\r\n    ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("a", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "41321f8696c25122626c79026485ee16de4854383881", async() => {
                WriteLiteral("Create New");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_AnchorTagHelper.Action = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n</p>\r\n<table class=\"table\">\r\n    <thead>\r\n        <tr>\r\n            <th>\r\n                ");
#nullable restore
#line 16 "G:\Users\GitHub\exercisebook\MicroService\MicroService\WebApi.TestConsul\Views\TestOne\GetListInConsul.cshtml"
           Write(Html.DisplayNameFor(model => model.Id));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </th>\r\n            <th>\r\n                ");
#nullable restore
#line 19 "G:\Users\GitHub\exercisebook\MicroService\MicroService\WebApi.TestConsul\Views\TestOne\GetListInConsul.cshtml"
           Write(Html.DisplayNameFor(model => model.Name));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </th>\r\n            <th>\r\n                ");
#nullable restore
#line 22 "G:\Users\GitHub\exercisebook\MicroService\MicroService\WebApi.TestConsul\Views\TestOne\GetListInConsul.cshtml"
           Write(Html.DisplayNameFor(model => model.Account));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </th>\r\n            <th>\r\n                ");
#nullable restore
#line 25 "G:\Users\GitHub\exercisebook\MicroService\MicroService\WebApi.TestConsul\Views\TestOne\GetListInConsul.cshtml"
           Write(Html.DisplayNameFor(model => model.Password));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </th>\r\n            <th>\r\n                ");
#nullable restore
#line 28 "G:\Users\GitHub\exercisebook\MicroService\MicroService\WebApi.TestConsul\Views\TestOne\GetListInConsul.cshtml"
           Write(Html.DisplayNameFor(model => model.Email));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </th>\r\n            <th>\r\n                ");
#nullable restore
#line 31 "G:\Users\GitHub\exercisebook\MicroService\MicroService\WebApi.TestConsul\Views\TestOne\GetListInConsul.cshtml"
           Write(Html.DisplayNameFor(model => model.Role));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </th>\r\n            <th>\r\n                ");
#nullable restore
#line 34 "G:\Users\GitHub\exercisebook\MicroService\MicroService\WebApi.TestConsul\Views\TestOne\GetListInConsul.cshtml"
           Write(Html.DisplayNameFor(model => model.LoginTime));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n            </th>\r\n            <th></th>\r\n        </tr>\r\n    </thead>\r\n    <tbody>\r\n");
#nullable restore
#line 40 "G:\Users\GitHub\exercisebook\MicroService\MicroService\WebApi.TestConsul\Views\TestOne\GetListInConsul.cshtml"
         foreach (var item in Model)
        {

#line default
#line hidden
#nullable disable
            WriteLiteral("            <tr>\r\n                <td>\r\n                    ");
#nullable restore
#line 44 "G:\Users\GitHub\exercisebook\MicroService\MicroService\WebApi.TestConsul\Views\TestOne\GetListInConsul.cshtml"
               Write(Html.DisplayFor(modelItem => item.Id));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 47 "G:\Users\GitHub\exercisebook\MicroService\MicroService\WebApi.TestConsul\Views\TestOne\GetListInConsul.cshtml"
               Write(Html.DisplayFor(modelItem => item.Name));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 50 "G:\Users\GitHub\exercisebook\MicroService\MicroService\WebApi.TestConsul\Views\TestOne\GetListInConsul.cshtml"
               Write(Html.DisplayFor(modelItem => item.Account));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 53 "G:\Users\GitHub\exercisebook\MicroService\MicroService\WebApi.TestConsul\Views\TestOne\GetListInConsul.cshtml"
               Write(Html.DisplayFor(modelItem => item.Password));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 56 "G:\Users\GitHub\exercisebook\MicroService\MicroService\WebApi.TestConsul\Views\TestOne\GetListInConsul.cshtml"
               Write(Html.DisplayFor(modelItem => item.Email));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 59 "G:\Users\GitHub\exercisebook\MicroService\MicroService\WebApi.TestConsul\Views\TestOne\GetListInConsul.cshtml"
               Write(Html.DisplayFor(modelItem => item.Role));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 62 "G:\Users\GitHub\exercisebook\MicroService\MicroService\WebApi.TestConsul\Views\TestOne\GetListInConsul.cshtml"
               Write(Html.DisplayFor(modelItem => item.LoginTime));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n                <td>\r\n                    ");
#nullable restore
#line 65 "G:\Users\GitHub\exercisebook\MicroService\MicroService\WebApi.TestConsul\Views\TestOne\GetListInConsul.cshtml"
               Write(Html.ActionLink("Edit", "Edit", new { /* id=item.PrimaryKey */ }));

#line default
#line hidden
#nullable disable
            WriteLiteral(" |\r\n                    ");
#nullable restore
#line 66 "G:\Users\GitHub\exercisebook\MicroService\MicroService\WebApi.TestConsul\Views\TestOne\GetListInConsul.cshtml"
               Write(Html.ActionLink("Details", "Details", new { /* id=item.PrimaryKey */ }));

#line default
#line hidden
#nullable disable
            WriteLiteral(" |\r\n                    ");
#nullable restore
#line 67 "G:\Users\GitHub\exercisebook\MicroService\MicroService\WebApi.TestConsul\Views\TestOne\GetListInConsul.cshtml"
               Write(Html.ActionLink("Delete", "Delete", new { /* id=item.PrimaryKey */ }));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                </td>\r\n            </tr>\r\n");
#nullable restore
#line 70 "G:\Users\GitHub\exercisebook\MicroService\MicroService\WebApi.TestConsul\Views\TestOne\GetListInConsul.cshtml"
        }

#line default
#line hidden
#nullable disable
            WriteLiteral("    </tbody>\r\n</table>");
        }
        #pragma warning restore 1998
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<IEnumerable<WebApi.TestConsul.Models.User>> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591
