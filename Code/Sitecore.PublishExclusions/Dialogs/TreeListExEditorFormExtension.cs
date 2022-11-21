namespace Sitecore.PublishExclusions.Dialogs
{
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.Globalization;
    using Sitecore.Resources;
    using Sitecore.Shell.Applications.ContentEditor;
    using Sitecore.Text;
    using Sitecore.Web;
    using Sitecore.Web.UI.Sheer;
    using System;
    using System.IO;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    public class TreeListExEditorFormExtension : WebControl, IContentField, IMessageHandler
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is read only.
        /// </summary>
        /// <value><c>true</c> if  this instance is read only; otherwise, <c>false</c>.</value>
        public virtual bool ReadOnly
        {
            get
            {
                bool result = false;
                bool.TryParse(this.ViewState[nameof(ReadOnly)].ToString(), out result);
                return result;
            }
            set
            {
                this.ViewState[nameof(ReadOnly)] = (object)value;
                if (value)
                {
                    this.Attributes["readonly"] = "readonly";
                    this.Disabled = true;
                }
                else
                    this.Attributes.Remove("readonly");
            }
        }

        protected override object SaveViewState()
        {
            return base.SaveViewState();
        }

        /// <summary>Gets or sets the item ID.</summary>
        /// <value>The item ID.</value>
        /// <contract>
        ///   <requires name="value" condition="not null" />
        /// </contract>
        public string ItemID
        {
            get => StringUtil.GetString(this.ViewState[nameof(ItemID)]);
            set
            {
                Assert.ArgumentNotNull((object)value, nameof(value));
                this.ViewState[nameof(ItemID)] = (object)value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Sitecore.Shell.Applications.ContentEditor.FieldTypes.TreelistEx" /> is disabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if disabled, otherwise, <c>false</c>.
        /// </value>
        public bool Disabled
        {
            get => !this.Enabled;
            set
            {
                this.Enabled = !value;
                this.ViewState["Enabled"] = (object)!value;
            }
        }

        /// <summary>Gets the database.</summary>
        /// <value>The database.</value>
        public Database Database
        {
            get
            {
                UrlString urlString = new UrlString(this.Source);
                return !string.IsNullOrEmpty(urlString["databasename"]) ? Factory.GetDatabase(urlString["databasename"]) : Sitecore.Context.ContentDatabase;
            }
        }

        /// <summary>Gets or sets the item language.</summary>
        /// <value>The item language.</value>
        public string ItemLanguage
        {
            get => StringUtil.GetString(this.ViewState[nameof(ItemLanguage)]);
            set
            {
                Assert.ArgumentNotNull((object)value, nameof(value));
                this.ViewState[nameof(ItemLanguage)] = (object)value;
            }
        }

        /// <summary>Gets or sets the source.</summary>
        /// <value>The source.</value>
        public string Source
        {
            get => StringUtil.GetString(this.ViewState[nameof(Source)]);
            set
            {
                Assert.ArgumentNotNull((object)value, nameof(value));
                this.ViewState[nameof(Source)] = (object)value;
            }
        }

        /// <summary>Gets or sets the value.</summary>
        /// <value>The value.</value>
        public string Value
        {
            get => StringUtil.GetString(this.ViewState[nameof(Value)]);
            set
            {
                Assert.ArgumentNotNull((object)value, nameof(value));
                this.ViewState[nameof(Value)] = (object)value;
            }
        }

        /// <summary>Renders the control to the specified HTML writer.</summary>
        /// <param name="output">The <see cref="T:System.Web.UI.HtmlTextWriter" /> object that receives the control content. </param>
        protected override void Render(HtmlTextWriter output)
        {
            Assert.ArgumentNotNull((object)output, nameof(output));
            output.Write("<div id=\"" + this.ID + "\" class=\"scContentControl scTreelistEx\" ondblclick=\"javascript:return scForm.postEvent(this,event,'treelist:edit(id=" + this.ID + ")')\" onactivate=\"javascript:return scForm.activate(this,event)\" ondeactivate=\"javascript:return scForm.activate(this,event)\">");
            this.RenderItems(output);
            output.Write("</div>");
        }

        /// <summary>Edits the specified args.</summary>
        /// <param name="args">The arguments.</param>
        protected void Edit(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull((object)args, nameof(args));
            if (this.Disabled)
                return;
            if (args.IsPostBack)
            {
                if (args.Result == null || !(args.Result != "undefined"))
                    return;
                string str = args.Result;
                if (str == "-")
                    str = string.Empty;
                if (this.Value != str)
                    Sitecore.Context.ClientPage.Modified = true;
                this.Value = str;
                HtmlTextWriter output = new HtmlTextWriter((TextWriter)new StringWriter());
                this.RenderItems(output);
                SheerResponse.SetInnerHtml(this.ID, output.InnerWriter.ToString());

                var x = Sitecore.Context.ClientPage.FindControl(this.ID.ToString());

                SheerResponse.Refresh();

                var exclusionItem = Factory.GetDatabase("master").GetItem(new Data.ID(this.ItemID), Language.Parse("en"));

               // exclusionItem.Editing.EndEdit(true, false);

                exclusionItem.Editing.BeginEdit();

                MultilistField x = exclusionItem.Fields["Teestt"];
                x.Add(str);
                exclusionItem.Editing.EndEdit();
            }
            else
            {
                UrlString urlString = new UrlString(UIUtil.GetUri("control:CustomTreeListExEditor"));
                urlString.Parameters.Add("filterItem", this.Source);
                UrlHandle urlHandle = new UrlHandle();
                string empty = this.Value;
                if (empty == "__#!$No value$!#__")
                    empty = string.Empty;
                urlHandle["value"] = empty;
                urlHandle["source"] = this.Source;
                urlHandle["language"] = this.ItemLanguage;
                urlHandle["itemID"] = this.ItemID;
                urlHandle.Add(urlString);
                urlString.Append("sc_content", WebUtil.GetQueryString("sc_content"));
                SheerResponse.ShowModalDialog(urlString.ToString(), "1200px", "700px", string.Empty, true);
                args.WaitForPostBack();
            }
        }

        /// <summary>Renders the items.</summary>
        /// <param name="output">The output.</param>
        private void RenderItems(HtmlTextWriter output)
        {
            Assert.ArgumentNotNull((object)output, nameof(output));
            string str1 = this.Value;
            char[] chArray = new char[1] { '|' };
            foreach (string str2 in str1.Split(chArray))
            {
                if (!string.IsNullOrEmpty(str2))
                {
                    Item obj = this.Database.GetItem(str2, Language.Parse(this.ItemLanguage));
                    ImageBuilder imageBuilder = new ImageBuilder
                    {
                        Width = 16,
                        Height = 16,
                        Margin = "0px 4px 0px 0px",
                        Align = "absmiddle"
                    };
                    if (obj == null)
                    {
                        imageBuilder.Src = "Applications/16x16/forbidden.png";
                        output.Write("<div>");
                        imageBuilder.Render(output);
                        output.Write(Translate.Text("Item not found: {0}", (object)HttpUtility.HtmlEncode(str2)));
                        output.Write("</div>");
                    }
                    else
                    {
                        imageBuilder.Src = obj.Appearance.Icon;
                        output.Write("<div title=\"" + obj.Paths.ContentPath + "\">");
                        imageBuilder.Render(output);
                        output.Write(obj.GetUIDisplayName());
                        output.Write("</div>");
                    }
                }
            }
        }

        /// <summary>Gets the value.</summary>
        /// <returns>The value of the field.</returns>
        string IContentField.GetValue() => this.Value;

        /// <summary>Sets the value.</summary>
        /// <param name="value">The value of the field.</param>
        void IContentField.SetValue(string value)
        {
            Assert.ArgumentNotNull((object)value, nameof(value));
            this.Value = value;
        }

        /// <summary>Handles the message.</summary>
        /// <param name="message">The message.</param>
        void IMessageHandler.HandleMessage(Message message)
        {
            Assert.ArgumentNotNull((object)message, nameof(message));
            if (!(message["id"] == this.ID) || !(message.Name == "treelist:edit"))
                return;
            Sitecore.Context.ClientPage.Start((object)this, "Edit");
        }
    }
}
