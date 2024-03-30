using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LucasSpider.Http;
using Microsoft.Extensions.Options;

namespace LucasSpider.Downloader
{
	public class PPPoEService
	{
		private readonly PPPoEOptions _options;

		public PPPoEService(IOptions<PPPoEOptions> options)
		{
			_options = options.Value;
		}

		public bool IsActive => !string.IsNullOrWhiteSpace(_options.Account) &&
		                        !string.IsNullOrWhiteSpace(_options.Password) &&
		                        !string.IsNullOrWhiteSpace(_options.Interface);

		/// <summary>
		/// Asynchronous dialing, the result is returned directly first, and the crawler will retry and send to other agents.
		/// There is no need to wait for other downloads to complete when dialing, unless the node is offline first, and then wait for all downloads to complete.
		/// Dial again and resubscribe after successful dialing. The logic is too complicated.
		/// ADSL itself cannot be very fast, so just dial up and trigger a retry. As long as there are enough nodes, it is completely acceptable.
		/// </summary>
		/// <param name="request"></param>
		/// <param name="response"></param>
		/// <returns></returns>
		public Task<string> DetectAsync(Request request, string response)
		{
			var pattern = request.PPPoERegex;
			if (IsActive && !string.IsNullOrWhiteSpace(pattern))
			{
				var match = Regex.Match(response, pattern);
				if (match.Success)
				{
					Redial();
					return Task.FromResult(match.Value);
				}
			}

			return null;
		}

		private void Redial()
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				KillPPPoEProcesses();
				var process = Process.Start("/sbin/ifdown", "ppp0");
				if (process == null)
				{
					return;
				}

				process.WaitForExit();
				process = Process.Start("/sbin/ifup", "ppp0");
				if (process == null)
				{
					return;
				}

				process.WaitForExit();
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				RedialOnWindows();
				return;
			}

			throw new PlatformNotSupportedException($"{Environment.OSVersion.Platform}");
		}

		private void RedialOnWindows()
		{
			var process = new Process
			{
				StartInfo =
				{
					FileName = "rasdial.exe",
					UseShellExecute = false,
					CreateNoWindow = false,
					WorkingDirectory = @"C:\Windows\System32",
					Arguments = _options.Interface + @" /DISCONNECT"
				}
			};
			process.Start();
			process.WaitForExit(10000);

			process = new Process
			{
				StartInfo =
				{
					FileName = "rasdial.exe",
					UseShellExecute = false,
					CreateNoWindow = false,
					WorkingDirectory = @"C:\Windows\System32",
					Arguments = _options.Interface + " " + _options.Account + " " +
					            _options.Password
				}
			};
			process.Start();
			process.WaitForExit(10000);
		}

		private void KillPPPoEProcesses()
		{
			if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				var processes = Process.GetProcessesByName("pppd").ToList();
				processes.AddRange(Process.GetProcessesByName("pppoe"));
				foreach (var process in processes)
				{
					try
					{
						process.Kill();
					}
					catch
					{
						// ignore
					}
				}
			}
		}
	}
}
