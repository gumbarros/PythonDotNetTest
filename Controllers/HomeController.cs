using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    }

}
