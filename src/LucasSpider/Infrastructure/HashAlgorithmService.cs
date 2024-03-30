using System.Security.Cryptography;
using System.Threading;

namespace LucasSpider.Infrastructure
{
	public abstract class HashAlgorithmService : IHashAlgorithmService
	{
		private SpinLock _spin;

		protected abstract HashAlgorithm GetHashAlgorithm();

		public byte[] ComputeHash(byte[] bytes)
		{
			var locker = false;
			try
			{
				//Apply for a lock
				_spin.Enter(ref locker);
				return GetHashAlgorithm().ComputeHash(bytes);
			}
			finally
			{
				//When the work is completed or an exception occurs, check whether the current thread holds the lock. If we have the lock, release it
				//To avoid deadlock situations
				if (locker)
				{
					_spin.Exit();
				}
			}
		}
	}
}
