using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LucasSpider.Statistics.Store
{
	[Table("agent_statistics")]
	public class AgentStatistics
	{
		/// <summary>
		/// Node ID
		/// </summary>
		[StringLength(36)]
		[Column("agent_id")]
		public virtual string Id { get; private set; }

		/// <summary>
		/// Node name
		/// </summary>
		[Column("name")]
		public string Name { get; private set; }

		/// <summary>
		/// Number of successful downloads
		/// </summary>
		[Column("success")]
		public virtual long Success { get; private set; }

		/// <summary>
		/// Number of failed downloads
		/// </summary>
		[Column("failure")]
		public virtual long Failure { get; private set; }

		/// <summary>
		/// Total download time
		/// </summary>
		[Column("elapsed_milliseconds")]
		public virtual long ElapsedMilliseconds { get; private set; }

		/// <summary>
		/// Reporting time
		/// </summary>
		[Column("creation_time")]
		public DateTimeOffset CreationTime { get; private set; }

		/// <summary>
		///
		/// </summary>
		[Column("last_modification_time")]
		public DateTimeOffset LastModificationTime { get; private set; }

		public AgentStatistics(string id)
		{
			Id = id;
			CreationTime = DateTimeOffset.Now;
		}

		public void IncreaseSuccess()
		{
			Success += 1;
			LastModificationTime = DateTimeOffset.Now;
		}

		public void IncreaseFailure()
		{
			Failure += 1;
			LastModificationTime = DateTimeOffset.Now;
		}

		public void IncreaseElapsedMilliseconds(int elapsedMilliseconds)
		{
			ElapsedMilliseconds += (uint)elapsedMilliseconds;
		}

		public void SetName(string name)
		{
			Name = name;
		}
	}
}
