﻿using System.Diagnostics;
using AutoNews.DB;
using AutoNews.Models;
using Microsoft.AspNetCore.Mvc;

namespace AutoNews.Controllers;

public class HomeController : Controller
{
    private readonly AutoNewsContext _dataContext;

    public HomeController(AutoNewsContext dataContext) =>
        _dataContext = dataContext;

    //логика вам мерещится
    public IActionResult Index()
    {
        var monthAgo = DateTime.Now.AddMonths(-1);
        var twoMonthsAgo = DateTime.Now.AddMonths(-2);

        var news = _dataContext.News
            .Where(n => n.Date > monthAgo)
            .OrderBy(n => n.Likes)
            .Select(n => new HomeIndexModel.BetterNews
            {
                Id = n.Id,
                Title = n.Title,
                Text = n.Text,
                LogoUrl = n.LogoUrl,
                Likes = n.Likes,
                Date = n.Date,
                Creator = _dataContext.Users.FirstOrDefault(u => u.Id == n.CreatorId)
            })
            .ToList();

        var writer1 = _dataContext.Users
            .OrderBy(u => _dataContext.News
                .Where(n => n.CreatorId == u.Id && n.Date >= twoMonthsAgo && n.Date < monthAgo)
                .Sum(n => n.Likes))
            .FirstOrDefault();
        var writer2 = _dataContext.Users
            .OrderBy(u => _dataContext.News
                .Where(n => n.CreatorId == u.Id && n.Date >= monthAgo)
                .Sum(n => n.Likes))
            .FirstOrDefault();

        var model = new HomeIndexModel();
        model.HeaderNews = news.Count != default ? news.FirstOrDefault() : null;
        model.News1 = news.Count > 1
            ? news.Skip(1).Take(Math.Min(4, news.Count - 1)).ToList()
            : new List<HomeIndexModel.BetterNews>();
        model.News2 = news.Count > 5
            ? news.Skip(5).Take(Math.Min(4, news.Count - 1)).ToList()
            : new List<HomeIndexModel.BetterNews>();
        model.Writer1 = writer1;
        model.Writer2 = writer2;
        model.Likes1 = writer1 is not null
            ? _dataContext.News
                .Where(n => n.CreatorId == writer1.Id &&
                            n.Date >= twoMonthsAgo && n.Date < monthAgo)
                .Sum(n => n.Likes)
            : null;
        model.Likes2 = writer2 is not null
            ? _dataContext.News
                .Where(n => n.CreatorId == writer2.Id &&
                            n.Date >= monthAgo)
                .Sum(n => n.Likes)
            : null;
        return View(model);
    }

    public struct HomeIndexModel
    {
        public BetterNews? HeaderNews { get; set; }
        public List<BetterNews> News1 { get; set; }
        public List<BetterNews> News2 { get; set; }
        public User? Writer1 { get; set; }
        public int? Likes1 { get; set; }
        public User? Writer2 { get; set; }
        public int? Likes2 { get; set; }

        public readonly struct BetterNews
        {
            public int Id { get; init; }
            public string Title { get; init; }
            public string Text { get; init; }
            public string LogoUrl { get; init; }
            public int Likes { get; init; }
            public DateTime Date { get; init; }
            public User? Creator { get; init; }
        }
    }

    public IActionResult Privacy() =>
        View();

    public IActionResult Contacts() =>
        View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
}
