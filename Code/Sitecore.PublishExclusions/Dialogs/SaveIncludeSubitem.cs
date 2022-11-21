using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Resources;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Pages;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.XmlControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;

namespace Sitecore.PublishExclusions.Dialogs
{
    public class SaveIncludeSubitem : DialogForm
    {
        protected XmlControl Dialog;

        protected Literal CustomLiteral;

        protected Checkbox CustomCheckbox;

        protected Border MainBorder;

        private Dictionary<string, string> IncludeSubitems = new Dictionary<string, string>();

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Context.ClientPage.IsEvent || Context.ClientPage.IsPostBack)
            {
                SheerResponse.CloseWindow();
                return;
            }

            var selectedContents = WebUtil.GetQueryString("sc_selectedcontent");

            var customLiteral = new Literal($"This is the custom literal : {selectedContents}");

            Context.ClientPage.FindControl("borderId").Controls.Add(customLiteral);

            HtmlTextWriter output = new HtmlTextWriter(new StringWriter());
            
            this.RenderItems(output);

            UrlHandle urlHandle = UrlHandle.Get();

            if (!string.IsNullOrEmpty(urlHandle["title"]))
            {
                this.Dialog["Header"] = (object)urlHandle["title"];
            }

            if (!string.IsNullOrEmpty(urlHandle["text"]))
            {
                this.Dialog["text"] = (object)urlHandle["text"];
            }

            this.Dialog["icon"] = (object)urlHandle["icon"];
        }

        protected override void OnOK(object sender, EventArgs args)
        {
            var controls = Context.ClientPage.FindControl("borderId").Controls;

            base.OnOK(sender, args);
        }

        private void RenderItems(HtmlTextWriter output)
        {
            Assert.ArgumentNotNull((object)output, nameof(output));
            var selectedContents = WebUtil.GetQueryString("sc_selectedcontent");
            string str1 = selectedContents;
            char[] chArray = new char[1] { '|' };
            foreach (string str2 in str1.Split(chArray))
            {
                if (!string.IsNullOrEmpty(str2))
                {
                    Item obj = Factory.GetDatabase("master").GetItem(str2, Language.Parse("en"));

                    //var border = new Border
                    //{
                    //    ID = $"main_{str2}"
                    //};

                    var border = new Border();

                    var spacer = new Space
                    {
                        Width = 5
                    };

                    var checkbox = new Checkbox
                    {
                        Name = obj.Name,
                        ID = obj.ID.ToString(),
                        Header = obj.Name,
                        Checked = true,
                        Value = "1",
                        //Click = $"{obj.ID}.SetIncludeItems"
                    };

                    border.Controls.Add(spacer);
                    border.Controls.Add(checkbox);

                    Context.ClientPage.FindControl("borderId").Controls.Add(border);

                    //ImageBuilder imageBuilder = new ImageBuilder
                    //{
                    //    Width = 16,
                    //    Height = 16,
                    //    Margin = "0px 4px 0px 0px",
                    //    Align = "absmiddle"
                    //};
                    //if (obj == null)
                    //{
                    //    imageBuilder.Src = "Applications/16x16/forbidden.png";
                    //    output.Write("<div>");
                    //    imageBuilder.Render(output);
                    //    output.Write(Translate.Text("Item not found: {0}", (object)HttpUtility.HtmlEncode(str2)));
                    //    output.Write("</div>");
                    //}
                    //else
                    //{
                    //    imageBuilder.Src = obj.Appearance.Icon;
                    //    output.Write("<div title=\"" + obj.Paths.ContentPath + "\">");
                    //    imageBuilder.Render(output);
                    //    output.Write(obj.GetUIDisplayName());
                    //    output.Write("</div>");
                    //}
                }
            }
        }
    }
}
