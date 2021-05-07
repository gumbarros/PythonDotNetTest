using IronPython.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Scripting.Hosting;
using PythonDotNetTest.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PythonDotNetTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string code)
        {

            var result = RunIronPython(code);

            ViewBag.Result = result.Result;
            ViewBag.Error = result.Error;
            ViewBag.Code = code;

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public PythonResult RunPython(string code)
        {
            ProcessStartInfo start = new()
            {
                FileName = "python",
                Arguments = string.Format("-c \"{0}\"", code),
                UseShellExecute = false,
                CreateNoWindow = false,
                Verb = "runas",
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using Process process = Process.Start(start);
            using StreamReader reader = process.StandardOutput;

            var result = new PythonResult
            {
                Error = process.StandardError.ReadToEnd(), 
                Result = reader.ReadToEnd()
            };

            return result;
        }

        public PythonResult RunIronPython(string code)
        {
            var engine = Python.CreateEngine(); // Extract Python language engine from their grasp
            var scope = engine.CreateScope(); // Introduce Python namespace (scope)
            //var d = new Dictionary<string, object>
            //{
            //    { "serviceid", serviceid},
            //    { "parameter", parameter}
            //}; // Add some sample parameters. Notice that there is no need in specifically setting the object type, interpreter will do that part for us in the script properly with high probability

            //scope.SetVariable("params", d); // This will be the name of the dictionary in python script, initialized with previously created .NET Dictionary
            ScriptSource source = engine.CreateScriptSourceFromString(code);
            object result = source.Execute(scope);
            //parameter = scope.GetVariable<string>("parameter"); // To get the finally set variable 'parameter' from the python script
            var pythonResult = new PythonResult
            {
                Result = scope.GetVariable("a")
            };
            return pythonResult;
        }
    }

}
