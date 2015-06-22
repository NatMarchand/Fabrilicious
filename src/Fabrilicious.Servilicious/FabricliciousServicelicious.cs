using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services;

namespace Fabrilicious.Servilicious
{
	public class ServiceliciousService : StatelessService
	{
		protected override ICommunicationListener CreateCommunicationListener()
		{
			return new OwinCommunicationListener();
		}

		protected override Task RunAsync(CancellationToken cancellationToken)
		{
			return Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
		}
	}
}
