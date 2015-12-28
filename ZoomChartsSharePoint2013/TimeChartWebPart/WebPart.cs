using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace ZoomChartsSharePoint2013.TimeChartWebPart
{
    [ToolboxItemAttribute(false)]
    public class WebPart : ChartWebPartBase
    {
        public WebPart() : base(ChartType.TimeChart)
        {
            this.InitializationCode = @"
// this is a simple example that you should replace with your own code.
// for examples and documentation, see https://zoomcharts.com/en/documentation/

// note that the variable `chart` will always be initialized with a fresh chart instance.

chart.updateSettings({
    data: {
        units: [""M""],
        timestampInSeconds: true,
        preloaded: { 
            dataLimitFrom: 1279408157,
            dataLimitTo: 1384253671,
            unit: ""M"",
            values: [[1280062860, 3], [1282209412, 7], [1284577510, 5]]
        }
    },
    series: [{
        style: {
            fillColor: '#09c'
        }
    }]
});
";
        }
    }
}
