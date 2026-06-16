using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

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
        public string Welcome(string name, int numTimes = 1, int id = 4)
        {
            return HtmlEncoder.Default.Encode($"Hello {name}, NumTimes is: {numTimes}. ID: {id}"); // /HelloWorld/Welcome?name=Rick&numTimes=4 - Passing parameter information from the URL to the controller
                                                                                                   // HtmlEncoder.Default.Encode() is used to prevent malicious input, such as cross-site scripting or through JavaScript.
                                                                                                   // /HelloWorld/Welcome/3?name=Rick&numTimes=4 - ID is default convention, comes before the ? since its defined in Program.cs as part of the default routing format (/id?)
        }
    }
}
