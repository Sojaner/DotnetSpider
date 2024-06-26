using System.IO;
using System.Linq;
using LucasSpider.Selector;
using Xunit;

namespace LucasSpider.Tests
{
	public class JsonSelectorTests
	{
		[Fact]
		public void SelectLinks()
		{
			var json = File.ReadAllText("test.json");
			var selectable = new JsonSelectable(json);
			var result = selectable.SelectList(Selectors.JsonPath("$.[*].link")).Select(x => x.Value).ToList();
			Assert.Equal(8, result.Count);
			Assert.Equal("http://viettelglobal.vn/", result[0]);
		}
	}
}
