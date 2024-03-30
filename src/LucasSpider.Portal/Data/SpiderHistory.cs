using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LucasSpider.Portal.Data
{
	[Table("SPIDER_HISTORIES")]
	public class SpiderHistory
	{
		/// <summary>
		/// Primary key
		/// </summary>
		[Column("ID")]
		public int Id { get; set; }

		/// <summary>
		///
		/// </summary>
		[Column("SPIDER_ID")]
		[Required]
		public int SpiderId { get; set; }

		/// <summary>
		///
		/// </summary>
		[Column("SPIDER_NAME")]
		[StringLength(255)]
		[Required]
		public string SpiderName { get; set; }

		/// <summary>
		/// Container ID
		/// </summary>
		[Column("CONTAINER_ID")]
		[StringLength(100)]
		public string ContainerId { get; set; }

		/// <summary>
		/// Container ID
		/// </summary>
		[Column("BATCH")]
		[StringLength(36)]
		public string Batch { get; set; }

		/// <summary>
		/// Creation time
		/// </summary>
		[Column("CREATION_TIME")]
		[Required]
		public DateTimeOffset CreationTime { get; set; }

		[Column("STATUS")]
		[StringLength(20)]
		[Required]
		public string Status { get; set; }
	}
}
