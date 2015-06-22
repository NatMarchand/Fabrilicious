using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;

namespace Fabrilicious.Servilicious
{
	public class Program
	{
		public static void Main(string[] args)
		{
			try
			{
				using (var fabricRuntime = FabricRuntime.Create())
				{
					fabricRuntime.RegisterServiceType("FabriliciousServiliciousType", typeof(ServiceliciousService));
					ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(ServiceliciousService).Name);
					Thread.Sleep(Timeout.Infinite);
				}
			}
			catch (Exception e)
			{
				ServiceEventSource.Current.ServiceHostInitializationFailed(e);
				throw;
			}
		}
	}
}
