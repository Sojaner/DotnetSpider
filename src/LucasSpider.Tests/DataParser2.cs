using LucasSpider.DataFlow.Parser;
using LucasSpider.DataFlow.Storage;

namespace LucasSpider.Tests
{
	public class DataParser2<T>: DataParser<T> where T : EntityBase<T>, new()
	{
	}
}
