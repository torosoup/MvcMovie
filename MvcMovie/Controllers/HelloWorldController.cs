using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace MvcMovie.Controllers
{
    public class HelloWorldController : Controller
    {
        // GET: /HelloWorld/
        public IActionResult Index() // Every public method is callable as an HTTP endpoint, so the method name is used as the URL path. The default routing logic is /[Controller]/[ActionName]/[Parameters], so the URL for this action method is /HelloWorld/Index (parameters not used yet). If you omit the action name, it will default to Index, so /HelloWorld will also work.
        {
            return View(); // Calls the controller's View() method, and uses view tempelate to generate a HTML response.
        }
        // GET: /HelloWorld/Welcome/
        public IActionResult Welcome(string name, int numTimes = 1)
        {
            ViewData["Message"] = "Hello " + name; // Adding parameter values to the ViewData dictionary, which is used to pass data from the controller to the view. The view can access these values using the keys "Message" and "NumTimes".  Dynamic object.
            ViewData["NumTimes"] = numTimes;
            return View();
        }
    }
}
