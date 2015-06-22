using System;
using System.Diagnostics;
using System.Fabric;
using System.Fabric.Query;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Fabrilicious.Servilicious
{
	[RoutePrefix("api/fabric")]
	public class FabricController : ApiController
	{
		[HttpGet]
		[Route("nodes")]
		public async Task<IHttpActionResult> GetNodes()
		{
			try
			{
				var client = new FabricClient();
				var list = await client.QueryManager.GetNodeListAsync();
				return Ok(list);
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.ToString());
				throw;
			}
		}

		[HttpGet]
		[Route("applicationTypes")]
		public async Task<IHttpActionResult> GetApplicationTypes()
		{
			try
			{
				var client = new FabricClient();
				var list = await client.QueryManager.GetApplicationTypeListAsync();
				return Ok(list);
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.ToString());
				throw;
			}
		}

		[HttpGet]
		[Route("applications")]
		public async Task<IHttpActionResult> GetApplications()
		{
			try
			{
				var client = new FabricClient();
				var list = await client.QueryManager.GetApplicationListAsync();
				return Ok(list);
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.ToString());
				throw;
			}
		}

		[HttpGet]
		[Route("serviceTypes/{applicationTypeName}/{applicationTypeVersion}")]
		public async Task<IHttpActionResult> GetServiceTypes(string applicationTypeName, string applicationTypeVersion)
		{
			try
			{
				var client = new FabricClient();
				var list = await client.QueryManager.GetServiceTypeListAsync(applicationTypeName, applicationTypeVersion);
				return Ok(list);
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.ToString());
				throw;
			}
		}

		[HttpGet]
		[Route("services/{*applicationName}")]
		public async Task<IHttpActionResult> GetServices(string applicationName)
		{
			try
			{
				var client = new FabricClient();
				var list = await client.QueryManager.GetServiceListAsync(new Uri(applicationName));
				return Ok(list);
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.ToString());
				throw;
			}
		}

		[HttpGet]
		[Route("partitions/{*serviceName}")]
		public async Task<IHttpActionResult> GetPartitions(string serviceName)
		{
			try
			{
				var client = new FabricClient();
				var list = await client.QueryManager.GetPartitionListAsync(new Uri(serviceName));
				return Ok(list);
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.ToString());
				throw;
			}
		}

		[HttpGet]
		[Route("replicas/{partitionId}")]
		public async Task<IHttpActionResult> GetReplicas(Guid partitionId)
		{
			try
			{
				var client = new FabricClient();
				var list = await client.QueryManager.GetReplicaListAsync(partitionId);
				return Ok(list);
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.ToString());
				throw;
			}
		}


		[HttpGet]
		[Route("details")]
		public async Task<IHttpActionResult> GetDetails()
		{
			try
			{
				var client = new FabricClient();
				var nodes = (await client.QueryManager.GetNodeListAsync())
					.Select(async n =>
								{
									try
									{
										var applications = (await client.QueryManager.GetDeployedApplicationListAsync(n.NodeName))
											.Select(async a =>
														{
															var replicas = await client.QueryManager.GetDeployedReplicaListAsync(n.NodeName, a.ApplicationName);
															var groupedReplicas = replicas.GroupBy(r => r.ServiceName,
																									(k, items) =>
																									new
																									{
																										ServiceName = k,
																										Replicas = items.Select(i =>
																																{
																																	var replica = i as DeployedStatefulServiceReplica;
																																	if (replica != null)
																																	{
																																		return new
																																				{
																																					PartitionId = replica.Partitionid,
																																					replica.ServiceKind,
																																					Role = replica.ReplicaRole.ToString(),
																																					InstanceId = replica.ReplicaId,
																																					replica.ReplicaStatus
																																				};
																																	}
																																	var instance = i as DeployedStatelessServiceInstance;
																																	if (instance != null)
																																	{
																																		return new
																																				{
																																					PartitionId = instance.Partitionid,
																																					instance.ServiceKind,
																																					Role = string.Empty,
																																					InstanceId = instance.InstanceId,
																																					instance.ReplicaStatus
																																				};
																																	}

																																	return null;
																																})
																									})
																						.ToList();

															return new
																	{
																		a.ApplicationName,
																		a.ApplicationTypeName,
																		a.DeployedApplicationStatus,
																		//Services = await Task.WhenAll(services),
																		Services = groupedReplicas,
																	};
														});
										return new
												{
													n.NodeName,
													n.NodeStatus,
													n.HealthState,
													Applications = await Task.WhenAll(applications)
												};
									}
									catch (Exception e)
									{
										return null;
									}
								})
					.ToList();
				return Ok((await Task.WhenAll(nodes)).Where(n => n != null).OrderBy(n => n.NodeName));
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.ToString());
				throw;
			}
		}
	}
}