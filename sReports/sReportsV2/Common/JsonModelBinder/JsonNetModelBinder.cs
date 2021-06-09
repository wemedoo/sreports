using Newtonsoft.Json;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Extensions;
using sReportsV2.DTOs.Field.DataIn;
using sReportsV2.DTOs.Form.DataIn;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace sReportsV2.Common.JsonModelBinder
{
    public  class JsonNetModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            Ensure.IsNotNull(controllerContext, nameof(controllerContext));
            Ensure.IsNotNull(bindingContext, nameof(bindingContext));

            controllerContext.HttpContext.Request.InputStream.Position = 0;
            var stream = controllerContext.RequestContext.HttpContext.Request.InputStream;
            string result = null;
            using (StreamReader readStream = new StreamReader(stream, Encoding.UTF8))
            {
                result = readStream.ReadToEnd();
            }
            return JsonConvert.DeserializeObject(result, bindingContext.ModelType);
        }
    }
}