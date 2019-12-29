using Serilog;
using Serilog.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;

namespace WebAPI.App_Start
{
    public class CustomExceptionFilter: ExceptionFilterAttribute
    {      
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            string exceptionMessage = string.Empty;

            if (actionExecutedContext.Exception.InnerException == null)
            {
                exceptionMessage = actionExecutedContext.Exception.Message;
            }
            else
            {
                exceptionMessage = actionExecutedContext.Exception.InnerException.Message;
            }

            string errorFileLogPath = ConfigurationManager.AppSettings.Get("errorFileLogPath");

            var log  = new LoggerConfiguration()
                 .WriteTo.File(@errorFileLogPath, Serilog.Events.LogEventLevel.Error)
                      //.WriteTo.File(@"F:\NC-29-Sept\NoorCare.Api-master\NoorCare.Api\WebAPI\logger.txt", Serilog.Events.LogEventLevel.Error)
                      // .WriteTo.File(Server.MapPath(ConfigurationManager.AppSettings["logFile"].ToString()), Serilog.Events.LogEventLevel.Error)
                      //.WriteTo.RollingFile(@"Warnings_{Date}.txt", DateTime.Now.ToString())
                      .CreateLogger();
            
            log.Write(Serilog.Events.LogEventLevel.Error, actionExecutedContext.Exception,"json");

            //We can log this exception message to the file or database.  
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("An unhandled exception was thrown by service."),  
                    ReasonPhrase = "Internal Server Error.Please Contact your Administrator."
            };
            actionExecutedContext.Response = response;
        }
    }
}