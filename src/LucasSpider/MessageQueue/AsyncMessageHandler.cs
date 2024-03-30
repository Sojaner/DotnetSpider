using System.Threading.Tasks;

namespace LucasSpider.MessageQueue
{
	public delegate Task AsyncMessageHandler<in TMessage>(TMessage message);
}
