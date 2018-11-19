using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Rotativa.AspNetCore
{
    public class ViewAsPdf : AsPdfResultBase
    {
        private readonly object _model;
        private readonly ViewDataDictionary _viewData;

        public ViewAsPdf(WkHtmlToPdfDriver driver, Options.Options options, object model, ViewDataDictionary viewData = null) : base(driver, options)
        {
            _model = model;
            _viewData = viewData;
        }

        protected override string GetUrl(ActionContext context)
        {
            return string.Empty;
        }

        protected virtual ViewEngineResult GetView(ActionContext context, string viewName)
        {
            var engine = context.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
            return engine?.FindView(context, viewName, true);
        }

        protected override async Task<byte[]> CallTheDriver(ActionContext context)
        {
            string viewName = ViewName;
            if (string.IsNullOrEmpty(ViewName))
            {
                viewName = ((ControllerActionDescriptor) context.ActionDescriptor).ActionName;
            }

            var viewResult = GetView(context, viewName);
            var tempDataProvider = context.HttpContext.RequestServices.GetService(typeof(ITempDataProvider)) as ITempDataProvider;
            var viewDataDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = _model
            };
            if (_viewData != null)
            {
                foreach (var item in _viewData)
                {
                    viewDataDictionary.Add(item);
                }
            }

            StringBuilder html;
            using (var output = new StringWriter())
            {
                var view = viewResult.View;
                var tempDataDictionary = new TempDataDictionary(context.HttpContext, tempDataProvider);
                var viewContext = new ViewContext(context, viewResult.View, viewDataDictionary, tempDataDictionary, output, new HtmlHelperOptions());

                await view.RenderAsync(viewContext);

                html = output.GetStringBuilder();
            }

            string baseUrl = string.Format("{0}://{1}", context.HttpContext.Request.Scheme, context.HttpContext.Request.Host);
            var htmlForWkHtml = Regex.Replace(html.ToString(), "<head>", $"<head><base href=\"{baseUrl}\" />", RegexOptions.IgnoreCase);

            byte[] fileContent = Driver.ConvertHtml(GetConvertOptions(), htmlForWkHtml);
            return fileContent;
        }
    }
}