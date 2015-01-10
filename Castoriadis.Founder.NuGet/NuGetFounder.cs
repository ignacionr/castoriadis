using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Castoriadis.Client;
using NuGet;

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
				path = Path.Combine (path, nsName);
				Directory.CreateDirectory (path);

				var packageManager = new PackageManager(repo, path);

				//Download and unzip the package
				packageManager.InstallPackage(package, null, false, true);

				// copy all EXEs and DLLs to the root install
				var di = new DirectoryInfo (path);
				new[]{"*.dll","*.exe"}.SelectMany(ext => di.GetFiles(ext, SearchOption.AllDirectories))
					.ToList()
						.ForEach(fi => {
							var dest = Path.Combine(path, fi.Name);
							if (Path.GetFullPath(dest) != fi.FullName) {
								if (File.Exists(dest))
									File.Delete(dest);
								fi.CopyTo(dest);
							}
						});

				// find the EXE
				var exeFile = di.GetFiles ("*.exe").First ();
#if __MonoCS__
				var module = Assembly.Load("Mono.Posix, Version=4.0.0.0, Culture=neutral, PublicKeyToken=0738eb9f132ed756");
				var type = module.GetType("Mono.Unix.Native.Syscall");
				var chmodMI = type.GetMethod("chmod");
				chmodMI.Invoke(null, new object[]{exeFile.FullName, 4095u});
#endif
				using (var proc = Process.Start(exeFile.FullName)) 
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

