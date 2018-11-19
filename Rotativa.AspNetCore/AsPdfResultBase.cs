using System;
using System.Text;
using Rotativa.AspNetCore.Options;

namespace Rotativa.AspNetCore
{
    public abstract class AsPdfResultBase : AsResultBase
    {
        protected readonly WkHtmlToPdfDriver Driver;

        protected AsPdfResultBase(WkHtmlToPdfDriver driver)
        {
            Driver = driver;
            PageMargins = new Margins();
        }

        protected override byte[] WkHtmlConvert(string switches)
        {
            return Driver.Convert(switches);
        }

        protected override string GetContentType()
        {
            return "application/pdf";
        }

        /// <summary>
        /// Path to wkhtmltopdf binary.
        /// </summary>
        [Obsolete("Use WkhtmlPath instead of CookieName.", false)]
        public string WkhtmltopdfPath
        {
            get => WkHtmlPath;
            set => WkHtmlPath = value;
        }

        protected override string GetConvertOptions()
        {
            var result = new StringBuilder();

            if (PageMargins != null) result.Append(PageMargins);

            result.Append(" ");
            result.Append(base.GetConvertOptions());

            return result.ToString().Trim();
        }
    }
}