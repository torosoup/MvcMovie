using Microsoft.AspNetCore.Mvc;

namespace MvcMovie.Controllers
{
    public class HelloWorldController : Controller
    {
        // GET: /HelloWorld/
        public string Index() // Every public method is callable as an HTTP endpoint, so the method name is used as the URL path. The default routing logic is /[Controller]/[ActionName]/[Parameters], so the URL for this action method is /HelloWorld/Index (parameters not used yet). If you omit the action name, it will default to Index, so /HelloWorld will also work.
        {
            return "This is my default action...";
        }
        // GET: /HelloWorld/Welcome/
        public string Welcome()
        {
            return "This is the Welcome action method...";
        }
    }
}
