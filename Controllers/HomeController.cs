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

            var result = RunPython(code);

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
            try {
                var engine = Python.CreateEngine(); // Extract Python language engine from their grasp
                var scope = engine.CreateScope(); // Introduce Python namespace (scope)

                ICollection<string> searchPaths = engine.GetSearchPaths();

                searchPaths.Add("C:\\Python34\\Lib");
                engine.SetSearchPaths(searchPaths);

                scope.ImportModule("random");

                ScriptSource source = engine.CreateScriptSourceFromString(code);

                //parameter = scope.GetVariable<string>("parameter"); // To get the finally set variable 'parameter' from the python script

                //scope.ImportModule("random");
                var pythonResult = new PythonResult
                {
                    Result = source.Execute(scope)
                };
                return pythonResult;
            }
            catch (Exception e) {
                return new PythonResult { Error = e.ToString()};
            }

            
        }
    }

}
