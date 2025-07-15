using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using static Api.Logger.Logger;

namespace Api
{

    public class LogClientInfoFilter : System.Web.Mvc.ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            var url = request.Url.ToString();
            var clientIp = request.UserHostAddress;
            var userAgent = request.UserAgent;
            var referrer = request.UrlReferrer;

            List<LogLines> lines = new List<LogLines>();
            lines = Add2Log(lines, $"URL: {url}, Client IP: {clientIp}, Browser: {userAgent}, Referrer: {referrer}", 100, "httplog");
            Write2File(lines);

            base.OnActionExecuting(filterContext);
        }
    }

    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new LogClientInfoFilter());
        }
    }
}

