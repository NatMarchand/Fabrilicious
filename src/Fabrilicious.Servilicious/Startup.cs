using System;
using System.Web.Http;
using Microsoft.Owin.Diagnostics;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Owin;

namespace Fabrilicious.Servilicious
{
	public class Startup
	{
		private readonly HttpConfiguration _httpConfiguration;

		public Startup()
		{
			_httpConfiguration = new HttpConfiguration();
			_httpConfiguration.MapHttpAttributeRoutes();
			_httpConfiguration.EnsureInitialized();
			_httpConfiguration.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter());
			_httpConfiguration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
			_httpConfiguration.Filters.Add(new NoCacheActionFilter());
		}

		public void Configuration(IAppBuilder appBuilder)
		{
			appBuilder.UseErrorPage(ErrorPageOptions.ShowAll);
			appBuilder.UseWebApi(_httpConfiguration);
			appBuilder.UseStageMarker(PipelineStage.MapHandler);
			appBuilder.UseFileServer(new FileServerOptions
									{
										FileSystem = new PhysicalFileSystem("site/wwwroot"),
										EnableDirectoryBrowsing = true
									});
		}
	}
}