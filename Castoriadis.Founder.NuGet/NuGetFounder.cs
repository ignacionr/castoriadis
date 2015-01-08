using System;
using System.Linq;
using Castoriadis.Client;
using NuGet;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace Castoriadis.Founder
{
	public class NuGetFounder
	{
		public void EnsureNamespace(IAgora agora, string nsName, string package) {
			if (!agora.GetRegistrations ().Any (reg => reg.Namespace == nsName)) {
				//Connect to the official package repository
				var repo = PackageRepositoryFactory.Default.CreateRepository(
					//"https://packages.nuget.org/api/v2"
					"https://www.myget.org/F/castoriadis/"
					);

				//Initialize the package manager
				var entryAssembly = Assembly.GetEntryAssembly ();
				var path = entryAssembly == null ? "." : Path.GetDirectoryName(entryAssembly.Location);
				var packageManager = new PackageManager(repo, path);

				//Download and unzip the package
				packageManager.InstallPackage(package, null, false, true);

				// find the EXE
				var di = new DirectoryInfo (path);
				var usedpath = di.GetDirectories ().FirstOrDefault (subd => subd.Name.ToUpper ().Contains (package.ToUpper ()));
				var exeFile = usedpath.GetFiles ("*.exe",SearchOption.AllDirectories).First ();
				using (var proc = Process.Start(new ProcessStartInfo(exeFile.FullName))) 
				{
					if (proc == null) {
						// try to determine why...
						throw new Exception ("The process coulnd't start.");
					}
					int waitTimes = 0;
					for (var registered = agora.GetRegistrations ().Any (reg => reg.Namespace == nsName); !registered; registered = agora.GetRegistrations ().Any (reg => reg.Namespace == nsName)) {
						Thread.Sleep (100);
						if (++waitTimes > 50) {
							proc.Kill ();
							throw new Exception ("Couldn't register on time.");
						}
					}
				}
			}
		}
	}
}

