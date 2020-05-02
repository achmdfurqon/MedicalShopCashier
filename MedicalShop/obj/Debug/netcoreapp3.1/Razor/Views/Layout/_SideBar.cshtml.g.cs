#pragma checksum "C:\Users\fvrqo\OneDrive\Documents\Visual Studio 2019\Projects\MedicalShop\MedicalShop\Views\Layout\_SideBar.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "efe469a2588b2283c4b19a41c228a2a04e50e9fe"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Layout__SideBar), @"mvc.1.0.view", @"/Views/Layout/_SideBar.cshtml")]
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
#line 1 "C:\Users\fvrqo\OneDrive\Documents\Visual Studio 2019\Projects\MedicalShop\MedicalShop\Views\_ViewImports.cshtml"
using MedicalShop;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\fvrqo\OneDrive\Documents\Visual Studio 2019\Projects\MedicalShop\MedicalShop\Views\_ViewImports.cshtml"
using MedicalShop.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 1 "C:\Users\fvrqo\OneDrive\Documents\Visual Studio 2019\Projects\MedicalShop\MedicalShop\Views\Layout\_SideBar.cshtml"
using Microsoft.AspNetCore.Http;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"efe469a2588b2283c4b19a41c228a2a04e50e9fe", @"/Views/Layout/_SideBar.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"a8d61f961965d1d2810b2e1d7cce41950280da95", @"/Views/_ViewImports.cshtml")]
    public class Views_Layout__SideBar : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n<div id=\"sidebar-menu\" class=\"main_menu_side hidden-print main_menu\">\r\n    <div class=\"menu_section\">\r\n        <h3>General</h3>\r\n        <ul class=\"nav side-menu\">\r\n");
#nullable restore
#line 10 "C:\Users\fvrqo\OneDrive\Documents\Visual Studio 2019\Projects\MedicalShop\MedicalShop\Views\Layout\_SideBar.cshtml"
             if (User.Identity.IsAuthenticated || 

#line default
#line hidden
#nullable disable
#nullable restore
#line 10 "C:\Users\fvrqo\OneDrive\Documents\Visual Studio 2019\Projects\MedicalShop\MedicalShop\Views\Layout\_SideBar.cshtml"
                                                                                        Context.Session.GetString("Role")!=null)
            {
                if (User.IsInRole("Admin") || Context.Session.GetString("Role").Contains("Admin"))
                {

#line default
#line hidden
#nullable disable
            WriteLiteral(@"                    <li>
                        <a href=""#""><i class=""fa fa-database""></i> Admin <span class=""fa fa-chevron-down""></span></a>
                        <ul class=""nav child_menu"">
                            <li><a href=""/Admin"">Dashboard</a></li>
                            <li><a href=""/Admin/Products"">Products</a></li>
                            <li><a href=""/Admin/Suppliers"">Suppliers</a></li>
                        </ul>
                    </li>
");
#nullable restore
#line 22 "C:\Users\fvrqo\OneDrive\Documents\Visual Studio 2019\Projects\MedicalShop\MedicalShop\Views\Layout\_SideBar.cshtml"
                }
                else if (User.IsInRole("Manager") || Context.Session.GetString("Role").Contains("Manager"))
                {

#line default
#line hidden
#nullable disable
            WriteLiteral(@"                    <li>
                        <a href=""#""><i class=""fa fa-cogs""></i> Manager <span class=""fa fa-chevron-down""></span></a>
                        <ul class=""nav child_menu"">
                            <li><a href=""/Manager"">Purchases</a></li>
                            <li><a href=""/Manager/Report"">Transactions</a></li>
                        </ul>
                    </li>
");
#nullable restore
#line 32 "C:\Users\fvrqo\OneDrive\Documents\Visual Studio 2019\Projects\MedicalShop\MedicalShop\Views\Layout\_SideBar.cshtml"
                }
                else if (User.IsInRole("Cashier") || Context.Session.GetString("Role").Contains("Cashier"))
                {

#line default
#line hidden
#nullable disable
            WriteLiteral(@"                    <li>
                        <a href=""#""><i class=""fa fa-money""></i> Cashier <span class=""fa fa-chevron-down""></span></a>
                        <ul class=""nav child_menu"">
                            <li><a href=""/Cashier"">Orders</a></li>
                            <li><a href=""/Cashier/Transactions"">Transactions</a></li>
                        </ul>
                    </li>
");
#nullable restore
#line 42 "C:\Users\fvrqo\OneDrive\Documents\Visual Studio 2019\Projects\MedicalShop\MedicalShop\Views\Layout\_SideBar.cshtml"
                }
                else
                {

#line default
#line hidden
#nullable disable
            WriteLiteral(@"                    <li>
                        <a href=""#""><i class=""fa fa-database""></i> Admin <span class=""fa fa-chevron-down""></span></a>
                        <ul class=""nav child_menu"">
                            <li><a href=""/Admin"">Dashboard</a></li>
                            <li><a href=""/Admin/Products"">Products</a></li>
                            <li><a href=""/Admin/Suppliers"">Suppliers</a></li>
                        </ul>
                    </li>
                    <li>
                        <a href=""#""><i class=""fa fa-cogs""></i> Manager <span class=""fa fa-chevron-down""></span></a>
                        <ul class=""nav child_menu"">
                            <li><a href=""/Manager"">Purchases</a></li>
                            <li><a href=""/Manager/Report"">Transactions</a></li>
                        </ul>
                    </li>
                    <li>
                        <a href=""#""><i class=""fa fa-money""></i> Cashier <span class=""fa fa-chevron-down""></span");
            WriteLiteral(@"></a>
                        <ul class=""nav child_menu"">
                            <li><a href=""/Cashier"">Orders</a></li>
                            <li><a href=""/Cashier/Transactions"">Transactions</a></li>
                        </ul>
                    </li>
");
#nullable restore
#line 67 "C:\Users\fvrqo\OneDrive\Documents\Visual Studio 2019\Projects\MedicalShop\MedicalShop\Views\Layout\_SideBar.cshtml"
                }
            }

#line default
#line hidden
#nullable disable
            WriteLiteral("        </ul>\r\n    </div>\r\n\r\n</div>\r\n");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591