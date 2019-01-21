using System.Net;
using System.Web.Mvc;

namespace RLCodeTest.Library.Attributes
{
    // CustomHandleErrorAttribute allows us to catch all exceptions in one place so they can be logged accordingly
    public class CustomHandleErrorAttribute : HandleErrorAttribute
    {
        //private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // Custom errors is set to ON, for demo purposes some error information will display in Views/Shared/Error.cshtml
        public override void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.HttpContext.IsCustomErrorEnabled)
            {
                return;
            }

            if (!ExceptionType.IsInstanceOfType(filterContext.Exception))
            {
                return;
            }

            var controllerName = (string)filterContext.RouteData.Values["controller"];
            var actionName = (string)filterContext.RouteData.Values["action"];
            var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);

            filterContext.Result = new ViewResult
            {
                ViewName = View,
                MasterName = Master,
                ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                TempData = filterContext.Controller.TempData
            };

            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;

            // This is where we would Log Errors
            // _log.Error("Internal server error occurred while handling web request.", filterContext.Exception);
        }
    }
}
