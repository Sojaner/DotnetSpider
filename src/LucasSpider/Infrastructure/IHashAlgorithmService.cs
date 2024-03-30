namespace LucasSpider.Infrastructure
{
	public interface IHashAlgorithmService
	{
		byte[] ComputeHash(byte[] bytes);
	}
}
