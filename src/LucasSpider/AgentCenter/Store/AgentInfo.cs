using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LucasSpider.Infrastructure;

namespace LucasSpider.AgentCenter.Store
{
	[Table("agent")]
	public class AgentInfo
	{
		/// <summary>
		/// Logo
		/// </summary>
		[StringLength(36)]
		public virtual string Id { get; private set; }

		/// <summary>
		/// Name
		/// </summary>
		[StringLength(255)]
		public string Name { get; private set; }

		/// <summary>
		/// Number of CPU cores
		/// </summary>
		[Column("processor_count")]
		public int ProcessorCount { get; private set; }

		/// <summary>
		/// Total memory
		/// </summary>
		[Column("total_memory")]
		public long TotalMemory { get; private set; }

		/// <summary>
		/// Last updated
		/// </summary>
		[Column("last_modification_time")]
		public DateTimeOffset LastModificationTime { get; private set; }

		/// <summary>
		/// Has it been marked for deletion?
		/// </summary>
		[Column("deleted")]
		public bool Deleted { get; private set; }

		/// <summary>
		/// Creation time
		/// </summary>
		[Required]
		[Column("creation_time")]
		public DateTimeOffset CreationTime { get; private set; }

		public AgentInfo(string id, string name, int processorCount, long totalMemory)
		{
			id.NotNullOrWhiteSpace(nameof(id));
			name.NotNullOrWhiteSpace(nameof(name));

			Id = id;
			Name = name;
			Deleted = false;
			ProcessorCount = processorCount;
			TotalMemory = totalMemory;
			CreationTime = DateTimeOffset.Now;
			LastModificationTime = CreationTime;
		}

		public bool Online => (DateTimeOffset.Now - LastModificationTime).TotalSeconds <= 30;

		/// <summary>
		/// Refresh the last update time
		/// </summary>
		public void Refresh()
		{
			LastModificationTime = DateTimeOffset.Now;
		}

		public override string ToString()
		{
			return $"Id {Id}, CreationTime {CreationTime}, Deleted {Deleted}";
		}
	}
}
