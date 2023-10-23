namespace TLRN_DataAggregator_RegressionTest
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Security.Policy;
	using System.Text;
	using System.Threading.Tasks;
	using ApiStatus;
	using Library.Tests.TestCases;
	using Newtonsoft.Json;
	using QAPortalAPI.Models.ReportingModels;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Core.DataMinerSystem.Common;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel.Status;
	using Skyline.DataMiner.Net.Apps.MigrationManager.Objects;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	public class TestJobsSuccessful : ITestCase
	{
		public TestJobsSuccessful(string name)
		{
			if (String.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentNullException("name");
			}

			Name = name;
		}

		public string Name { get; set; }

		public TestCaseReport TestCaseReport { get; private set; }

		public PerformanceTestCaseReport PerformanceTestCaseReport { get; private set; }

		public static JobsStatus GetJobsStatus(Uri statusUri)
		{

			using (var webClient = new WebClient())
			{
				var json = webClient.DownloadString(statusUri);
				JobsStatus jobsStatus = JsonConvert.DeserializeObject<JobsStatus>(json);
				return jobsStatus;
			}
		}

		public static bool IsJobSuccessful(JobsStatus jobsStatus)
		{
			foreach (Job job in jobsStatus.Jobs)
			{
				if (job.State == "Failed")
				{
					return false;
				}
			}

			return true;
		}

		public void Execute(IEngine engine)
		{
			// TODO: Implement your test case
			// The below is an example.
			Uri statusUri = new Uri("http://localhost:5000/api/status");
			JobsStatus jobStatus = GetJobsStatus(statusUri);

			var isSuccess = IsJobSuccessful(jobStatus);
			if (isSuccess)
			{
				TestCaseReport = TestCaseReport.GetSuccessTestCase(Name);
			}
			else
			{
				TestCaseReport = TestCaseReport.GetFailTestCase(Name, "Failed example");
			}
		}
	}
}

namespace ApiStatus
{
	using System;
	using System.Collections.Generic;

	using System.Globalization;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Converters;

	public class JobsStatus
	{
		[JsonProperty("jobs")]
		public List<Job> Jobs { get; set; }

		[JsonProperty("fetching")]
		public Fetching Fetching { get; set; }
	}

	public class Fetching
	{
		[JsonProperty("active")]
		public long Active { get; set; }

		[JsonProperty("queued")]
		public long Queued { get; set; }
	}

	public class Job
	{
		[JsonProperty("jobID")]
		public long JobId { get; set; }

		[JsonProperty("state")]
		public string State { get; set; }

		[JsonProperty("error")]
		public string Error { get; set; }

		[JsonProperty("lastRunUTC")]
		public long LastRunUtc { get; set; }

		[JsonProperty("nextRunUTC")]
		public long NextRunUtc { get; set; }

		[JsonProperty("lastRunDurationSeconds")]
		public double LastRunDurationSeconds { get; set; }

		[JsonProperty("lastRunRowCount")]
		public long LastRunRowCount { get; set; }
	}
}