using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Microsoft.ServiceFabric.Services;

namespace Fabrilicious.Servilicious
{
	public class OwinCommunicationListener : ICommunicationListener
	{
		private IDisposable _serverHandle;
		private string _publishAddress;
		private string _listeningAddress;

		public void Initialize(ServiceInitializationParameters serviceInitializationParameters)
		{
			Trace.WriteLine("Initialize");

			var serviceEndpoint = serviceInitializationParameters.CodePackageActivationContext.GetEndpoint("ServiceEndpoint");
			_listeningAddress = $"http://+:{serviceEndpoint.Port}/";
			_publishAddress = _listeningAddress.Replace("+", FabricRuntime.GetNodeContext().IPAddressOrFQDN);
		}

		public Task<string> OpenAsync(CancellationToken cancellationToken)
		{
			Trace.WriteLine("Opening on " + _listeningAddress);

			try
			{
				StartWebServer(_listeningAddress);
				return Task.FromResult(_publishAddress);
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex);
				StopWebServer();
				return null;
			}
		}

		public Task CloseAsync(CancellationToken cancellationToken)
		{
			Trace.WriteLine("Close");
			StopWebServer();
			return Task.FromResult(true);
		}

		public void Abort()
		{
			Trace.WriteLine("Abort");
			StopWebServer();
		}

		private void StartWebServer(string url)
		{
			Trace.WriteLine("Starting web server on " + url);
			_serverHandle = WebApp.Start<Startup>(url);
		}

		private void StopWebServer()
		{
			if (_serverHandle != null)
			{
				try
				{
					_serverHandle.Dispose();
				}
				catch (ObjectDisposedException)
				{
					// no-op
				}
			}
		}

		public bool ListenOnSecondary
		{
			get;
			set;
		}
	}
}