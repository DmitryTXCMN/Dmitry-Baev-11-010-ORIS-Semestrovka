﻿using System.Diagnostics;
using AutoNews.Models;
using Microsoft.AspNetCore.Mvc;

namespace AutoNews.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() =>
        View();

    public IActionResult Privacy() =>
        View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
}
