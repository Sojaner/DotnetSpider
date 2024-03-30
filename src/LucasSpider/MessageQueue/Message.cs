using LucasSpider.Infrastructure;

namespace LucasSpider.MessageQueue
{
	public abstract class Message
	{
		public long Timestamp { get; set; }
		public string MessageId { get; set; }

		protected Message()
		{
			MessageId = ObjectId.CreateId().ToString();
		}
	}
}
