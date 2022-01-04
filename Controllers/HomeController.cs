using IronPython.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Scripting.Hosting;
using PythonDotNetTest.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

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
                var engine = Python.CreateEngine(); 
                var scope = engine.CreateScope();

                ICollection<string> searchPaths = engine.GetSearchPaths();

                string user = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                string assemblyVersion = Assembly.GetAssembly(typeof(IronPython.Compiler.PythonCompilerOptions)).GetName().Version.ToString();
                searchPaths.Add(user + $"\\.nuget\\packages\\ironpython.stdlib\\3.4.0-alpha1\\content\\Lib");
                engine.SetSearchPaths(searchPaths);

                ScriptSource source = engine.CreateScriptSourceFromString(code);

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
