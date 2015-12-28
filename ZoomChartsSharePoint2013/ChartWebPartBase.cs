using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace ZoomChartsSharePoint2013
{
    [System.Xml.Serialization.XmlRoot(Namespace = "https://sharepoint.zoomcharts.com/xml/")]
    public abstract class ChartWebPartBase : WebPart
    {
        private const string DefaultLibraryUrl = "https://cdn.zoomcharts-cloud.com/1/stable/zoomcharts.js";
        private const string _libraryScriptAddedKey = "zoomcharts.js_added";
        private readonly ChartType _chartType;

        /// <summary>
        /// Gets or sets the URL from which the zoomcharts.js library is included. 
        /// The default uses stable version from CDN.
        /// </summary>
        [Category("ZoomCharts")]
        [DefaultValue(DefaultLibraryUrl)]
        [WebDisplayName("Library URL")]
        [Personalizable(PersonalizationScope.Shared)]
        [Description("Specifies the URL from which the zoomcharts.js library is included")]
        [WebBrowsable(true)]
        [System.Xml.Serialization.XmlElement("libraryUrl")]
        public string LibraryUrl { get; set; }

        /// <summary>
        /// Gets or sets the value of the `ZoomChartsLicense` variable.
        /// </summary>
        [Category("ZoomCharts")]
        [WebDisplayName("License name")]
        [Personalizable(PersonalizationScope.Shared)]
        [Description("Specifies the name of the license to use")]
        [WebBrowsable(true)]
        [System.Xml.Serialization.XmlElement("licenseName")]
        public string LicenseName { get; set; }

        /// <summary>
        /// Gets or sets the value of the `ZoomChartsLicenseKey` variable.
        /// </summary>
        [Category("ZoomCharts")]
        [WebDisplayName("License key")]
        [Personalizable(PersonalizationScope.Shared)]
        [Description("Specifies the license key to use")]
        [WebBrowsable(true)]
        [System.Xml.Serialization.XmlElement("licenseKey")]
        public string LicenseKey { get; set; }

        /// <summary>
        /// Gets or sets the chart initialization code.
        /// </summary>
        [System.Xml.Serialization.XmlElement("initializationCode")]
        [Personalizable(PersonalizationScope.Shared)]
        [WebBrowsable(false)]
        public string InitializationCode { get; set; }

        /// <summary>
        /// Holds the panel that will be the chart container.
        /// </summary>
        private Panel panel;

        protected ChartWebPartBase(ChartType chartType)
        {
            this._chartType = chartType;
            this.LibraryUrl = DefaultLibraryUrl;
        }

        protected override void CreateChildControls()
        {
            this.panel = new Panel();
            panel.ID = "chart";
            this.Controls.Add(panel);

            panel.Controls.Add(new Label() { Text = "Please wait while the chart is initializing..." });

            base.CreateChildControls();
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            base.RenderContents(writer);

            if (!this.Page.Items.Contains(_libraryScriptAddedKey))
            {
                writer.AddAttribute("src", this.LibraryUrl);
                writer.RenderBeginTag(HtmlTextWriterTag.Script);
                writer.RenderEndTag();
                this.Page.Items[_libraryScriptAddedKey] = true;
            }


            writer.RenderBeginTag("script");
            if (!string.IsNullOrWhiteSpace(this.LicenseKey) && !string.IsNullOrWhiteSpace(this.LicenseName))
            {
                writer.WriteLine("window.ZoomChartsLicense = " + HttpUtility.JavaScriptStringEncode(this.LicenseName, true) + ";");
                writer.WriteLine("window.ZoomChartsLicenseKey = " + HttpUtility.JavaScriptStringEncode(this.LicenseKey, true) + ";");
            }

            writer.WriteLineNoTabs(@"(function() {
    var chart = new ZoomCharts." + this._chartType.ToString() + @"({ container: '" + panel.ClientID + @"' });
    " + this.InitializationCode + @";
})();");
            writer.RenderEndTag();
        }

        public override EditorPartCollection CreateEditorParts()
        {
            var def = base.CreateEditorParts();

            var merged = new EditorPartCollection(def, new[] { 
                new InitializationCodeEditor(this.ID) 
            });

            return merged;
        }
    }

    public class InitializationCodeEditor : EditorPart
    {
        private TextBox _input;

        public InitializationCodeEditor(string webPartId)
        {
            this.ID = "InitializationCodeEditor" + webPartId;
            this.Title = "Chart initialization code";
            this.Description = "The JavaScript code that initializes the chart. Note that the chart is already created in the `chart` variable.";
            this._input = new TextBox();
            this._input.TextMode = TextBoxMode.MultiLine;
            this._input.Rows = 30;
            this._input.Style[HtmlTextWriterStyle.Width] = "100%";
            this._input.Style["box-sizing"] = "border-box";
            this._input.Wrap = false;
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            Controls.Add(this._input);
        }
        public override bool ApplyChanges()
        {
            EnsureChildControls();
            ChartWebPartBase webPart = WebPartToEdit as ChartWebPartBase;

            if (webPart != null)
            {
                webPart.InitializationCode = this._input.Text;
            }

            return true;
        }

        public override void SyncChanges()
        {
            EnsureChildControls();
            ChartWebPartBase webPart = WebPartToEdit as ChartWebPartBase;

            if (webPart != null)
            {
                this._input.Text = webPart.InitializationCode;
            }
        }
    }
}
