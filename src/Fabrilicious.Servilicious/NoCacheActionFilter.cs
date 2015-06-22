using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Fabrilicious.Servilicious
{
	public class NoCacheActionFilter : IActionFilter
	{
		public bool AllowMultiple => true;

		public async Task<HttpResponseMessage> ExecuteActionFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
		{
			var response = await continuation();

			if (response?.Headers != null)
			{
				response.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
				response.Headers.Pragma.Add(new NameValueHeaderValue("no-cache"));
			}

			return response;
		}
	}
}