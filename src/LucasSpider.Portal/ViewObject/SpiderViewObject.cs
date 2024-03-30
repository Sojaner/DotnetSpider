using System.ComponentModel.DataAnnotations;

namespace LucasSpider.Portal.ViewObject
{
	public class SpiderViewObject
	{
		/// <summary>
		/// Name
		/// </summary>
		[Required]
		[StringLength(255)]
		public string Name { get; set; }

		/// <summary>
		/// Docker image
		/// </summary>
		[Required]
		[StringLength(255)]
		public string Image { get; set; }

		/// <summary>
		/// Timed expression
		/// </summary>
		[Required]
		[StringLength(100)]
		public string Cron { get; set; }

		/// <summary>
		/// Environment variables
		/// </summary>
		[StringLength(2000)]
		public string Environment { get; set; }

		/// <summary>
		/// Mount directory
		/// </summary>
		[StringLength(2000)]
		public string Volume { get; set; }
	}
}
