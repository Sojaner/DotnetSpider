using Microsoft.AspNetCore.Mvc;

namespace LucasSpider.Portal.Controllers
{
	[Route("agents")]
	public class AgentController : Controller
	{
		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		[HttpGet("{id}/heartbeats")]
		public IActionResult Heartbeat()
		{
			return View();
		}
	}
}
