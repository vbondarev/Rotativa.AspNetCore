using System;
using System.Text;

namespace Rotativa.AspNetCore
{
    public abstract class AsPdfResultBase : AsResultBase
    {
        protected readonly WkHtmlToPdfDriver Driver;
        private readonly Options.Options _options;

        protected AsPdfResultBase(WkHtmlToPdfDriver driver, Options.Options options)
        {
            Driver = driver;
            _options = options;
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

            if (_options != null) result.Append(_options);

            result.Append(" ");
            result.Append(base.GetConvertOptions());

            return result.ToString().Trim();
        }
    }
}