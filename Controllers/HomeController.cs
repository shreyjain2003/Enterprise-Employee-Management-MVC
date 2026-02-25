using EmployeeDepartmentMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EmployeeDepartmentMVC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var chromeProcesses = Process.GetProcessesByName("chrome");

            var processList = new List<string>();
            foreach (var process in chromeProcesses)
            {
                processList.Add($"PID: {process.Id}, Memory: {process.WorkingSet64 / 1024 / 1024} MB");
            }
            ViewBag.ProcessList = processList;

            string sessionId = HttpContext.Session.Id;
            ViewBag.SessionID = sessionId;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
