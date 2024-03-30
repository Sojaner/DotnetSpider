using System;
using System.Threading.Tasks;
using LucasSpider.AgentCenter.Store;
using LucasSpider.Portal.Data;
using LucasSpider.Statistics.Store;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LucasSpider.Portal.Controllers
{
	public class HomeController : Controller
	{
		private readonly PortalDbContext _dbContext;

		public HomeController(PortalDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<IActionResult> Index()
		{
			ViewData["Agent"] = await _dbContext.Set<AgentInfo>().CountAsync();
			var activeTime = DateTimeOffset.Now.AddSeconds(-60);
			ViewData["OnlineAgent"] =
				await (_dbContext.Set<AgentInfo>().CountAsync(x => x.LastModificationTime > activeTime));
			ViewData["Spider"] = await _dbContext.Spiders.CountAsync();
			ViewData["RunningSpider"] = await _dbContext.Set<SpiderStatistics>()
				.CountAsync(x => x.LastModificationTime > activeTime);
			return View();
		}
	}
}
