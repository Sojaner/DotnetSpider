using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LucasSpider.Portal.Data
{
	[Table("SPIDER")]
	public class Spider
	{
		/// <summary>
		/// Primary key
		/// </summary>
		[Column("ID")]
		public int Id { get; set; }

		/// <summary>
		/// Whether to enable
		/// </summary>
		[Column("ENABLED")]
		public bool Enabled { get; set; }

		/// <summary>
		/// Reptile name
		/// </summary>
		[Required]
		[StringLength(255)]
		[Column("NAME")]
		public string Name { get; set; }

		/// <summary>
		/// Reptile name
		/// </summary>
		[Required]
		[StringLength(255)]
		[Column("IMAGE")]
		public string Image { get; set; }

		/// <summary>
		/// Timed expression
		/// </summary>
		[StringLength(255)]
		[Required]
		[Column("CRON")]
		public string Cron { get; set; }

		/// <summary>
		/// Environment variables for docker running
		/// </summary>
		[StringLength(2000)]
		[Column("ENVIRONMENT")]
		public string Environment { get; set; }

		/// <summary>
		/// Docker runs the mounted disk
		/// </summary>
		[StringLength(2000)]
		[Column("VOLUME")]
		public string Volume { get; set; }

		/// <summary>
		/// Creation time of this entity.
		/// </summary>
		[Required]
		[Column("CREATION_TIME")]
		public DateTimeOffset CreationTime { get; set; }

		/// <summary>
		/// Last updated
		/// </summary>
		[Column("LAST_MODIFICATION_TIME")]
		public DateTimeOffset LastModificationTime { get; set; }
	}
}
