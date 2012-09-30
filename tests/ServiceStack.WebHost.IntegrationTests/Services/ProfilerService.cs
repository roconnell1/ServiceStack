﻿using System.Collections.Generic;
using ServiceStack.MiniProfiler;
using ServiceStack.OrmLite;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;

namespace ServiceStack.WebHost.IntegrationTests.Services
{
	[Route("/profiler", "GET")]
	[Route("/profiler/{Type}", "GET")]
	public class MiniProfiler
	{
		public string Type { get; set; }
	}

	public class MiniProfilerService : ServiceBase<MiniProfiler>
	{
		public IDbConnectionFactory DbFactory { get; set; }

		protected override object Run(MiniProfiler request)
		{
			var profiler = Profiler.Current;

			using (var db = DbFactory.OpenDbConnection())
			using (profiler.Step("MiniProfiler Service"))
			{
				if (request.Type == "n1")
				{
					using (profiler.Step("N + 1 query"))
					{
						var results = new List<Movie>();
						foreach (var movie in db.Select<Movie>())
						{
							results.Add(db.QueryById<Movie>(movie.Id));
						}
						return results;
					}
				}

				using (profiler.Step("Simple Select all"))
				{
					return db.Select<Movie>();
				}
			}
		}
	}
}