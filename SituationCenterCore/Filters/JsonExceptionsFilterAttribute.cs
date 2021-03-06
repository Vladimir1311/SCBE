﻿using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SituationCenter.Shared.Exceptions;
using SituationCenter.Shared.ResponseObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SituationCenterCore.Filters
{
    public class JsonExceptionsFilterAttribute : Attribute, IExceptionFilter
    {
        private static int eventId;
        private readonly ILogger<JsonExceptionsFilterAttribute> _logger;

        public JsonExceptionsFilterAttribute(ILogger<JsonExceptionsFilterAttribute> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogWarning(new EventId(eventId++), context.Exception,
                JsonConvert.SerializeObject(new
                {
                    Action = context.ActionDescriptor.DisplayName
                }, Formatting.Indented));

            ResponseBase responseObj = null;
            switch (context.Exception)
            {
                case StatusCodeException scException:
                    responseObj = ResponseBase.BadResponse(scException.StatusCode);
                    break;

                case MultiStatusCodeException mscException:
                    responseObj = ResponseBase.BadResponse(mscException.Codes);
                    break;

                case ArgumentException argException:
                    responseObj = ResponseBase.BadResponse(StatusCode.ArgumentsIncorrect);
                    break;

                case NotImplementedException niException:
                    responseObj = ResponseBase.BadResponse(StatusCode.NotImplementFunction);
                    break;

                default:
                    responseObj = ResponseBase.BadResponse(StatusCode.UnknownError);
                    break;
            }
            var toWrite = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(responseObj));
            context.HttpContext.Response.ContentType = "application/json; charset=utf-8";
            context.HttpContext.Response.Body.Write(toWrite, 0, toWrite.Length);
            context.ExceptionHandled = true;
        }
    }
}
