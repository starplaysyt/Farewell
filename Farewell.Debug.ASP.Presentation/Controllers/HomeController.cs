using System.Diagnostics;
using Farewell.Abstractions.CQRS;
using Farewell.Debug.ASP.Application.Commands;
using Microsoft.AspNetCore.Mvc;
using Farewell.Debug.ASP.Presentation.Models;

namespace Farewell.Debug.ASP.Presentation.Controllers;

public class HomeController(Mediator mediator) : Controller
{
    public async Task<IActionResult> Index()
    {
        await mediator.HandleAsync(new AddFirstEntityCommand() { Field1 = "testField1", Field2 = "testField2"});
        
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
            { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}