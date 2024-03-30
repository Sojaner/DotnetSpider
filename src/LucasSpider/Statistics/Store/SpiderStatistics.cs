using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LucasSpider.Infrastructure;

namespace LucasSpider.Statistics.Store
{
	[Table("statistics")]
	public class SpiderStatistics
	{
		/// <summary>
		/// Spider ID
		/// </summary>
		[StringLength(36)]
		[Column("id")]
		public virtual string Id { get; private set; }

		/// <summary>
		/// Spider name
		/// </summary>
		[StringLength(255)]
		[Column("name")]
		public virtual string Name { get; private set; }

		/// <summary>
		/// Crawler start time
		/// </summary>
		[Column("start")]
		public virtual DateTimeOffset? Start { get; private set; }

		/// <summary>
		/// Crawler exit time
		/// </summary>
		[Column("exit")]
		public virtual DateTimeOffset? Exit { get; private set; }

		/// <summary>
		/// Total number of links
		/// </summary>
		[Column("total")]
		public virtual long Total { get; private set; }

		/// <summary>
		/// Has been completed
		/// </summary>
		[Column("success")]
		public virtual long Success { get; private set; }

		/// <summary>
		/// Number of failed links
		/// </summary>
		[Column("failure")]
		public virtual long Failure { get; private set; }

		/// <summary>
		///
		/// </summary>
		[Column("last_modification_time")]
		public DateTimeOffset LastModificationTime { get; private set; }

		/// <summary>
		///
		/// </summary>
		[Column("creation_time")]
		public DateTimeOffset CreationTime { get; private set; }

		public SpiderStatistics(string id)
		{
			id.NotNullOrWhiteSpace(nameof(id));

			Id = id;
			CreationTime = DateTimeOffset.Now;
		}

		public void SetName(string name)
		{
			name.NotNullOrWhiteSpace(nameof(name));
			Name = name;
		}

		public void OnStarted()
		{
			Start = DateTimeOffset.Now;
		}

		public void OnExited()
		{
			Exit = DateTimeOffset.Now;
		}

		public void IncrementSuccess()
		{
			Success += 1;
			LastModificationTime = DateTimeOffset.Now;
		}

		public void IncrementFailure()
		{
			Failure += 1;
			LastModificationTime = DateTimeOffset.Now;
		}

		public void IncrementTotal(long count)
		{
			Total += count;
			LastModificationTime = DateTimeOffset.Now;
		}
	}
}
