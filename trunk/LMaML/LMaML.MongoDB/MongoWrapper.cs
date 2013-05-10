using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading;
using LMaML.Infrastructure.Domain;
using MongoDB.Driver;
using iLynx.Common.Configuration;
using iLynx.Common;

//using T0yK4T.Tools.Data.Mongo;
//using System.ServiceProcess;

namespace LMaML.MongoDB
{
    /// <summary>
    ///     A "Wrapper" for mongodb - This actually just takes care of starting the service (unless mongoservice is set to false, in which case mongod will be started "standalone")
    /// </summary>
    public class MongoWrapper : ComponentBase, IMongoWrapper, IDisposable
    {
        private readonly IConfigurableValue<string> mongoFile;

        private readonly IConfigurableValue<string> dbPath;

        private readonly IConfigurableValue<string> logFile;

        private readonly IConfigurableValue<int> mongoPort;

        private readonly IConfigurableValue<string> mongoHost;

        private readonly IConfigurableValue<string> mongoServiceName;

        private readonly IConfigurableValue<string> mongoArgs;

        private readonly IConfigurableValue<bool> runAsService;

        private readonly IConfigurableValue<string> mongoServiceMachineName;

        private Process mongoProcess;
        private bool processStarted;

        public MongoWrapper(IConfigurationManager configurationManager, ILogger logger)
            : base(logger)
        {
            logger.Guard("logger");
            configurationManager.Guard("configurationManager");
            DatabaseName = typeof(MongoWrapper).Assembly.GetName().Name.Replace(".", "");
            mongoFile = configurationManager.GetValue("mongofile", "mongod.exe");
            dbPath = configurationManager.GetValue("dbpath", string.Format("{0}\\data\\db", Environment.CurrentDirectory));
            logFile = configurationManager.GetValue("logfile", "mongodb.log");
            mongoPort = configurationManager.GetValue("mongoport", 27017);
            mongoHost = configurationManager.GetValue("mongohost", "localhost");
            mongoServiceName = configurationManager.GetValue("mongoname", "MongoDB");
            mongoServiceMachineName = configurationManager.GetValue("mongomachine", Environment.MachineName);
            mongoArgs = configurationManager.GetValue("mongoargs", string.Format("--port {0} --dbpath \"{1}\" --logpath \"{2}\\{3}\"", mongoPort, dbPath, dbPath.Value, logFile.Value));
            runAsService = configurationManager.GetValue("mongoservice", false);
            if (MongoAvailable) return;
            StartMongo();
        }

        public void DumpOutput()
        {
            while (!mongoProcess.StandardOutput.EndOfStream)
                Trace.WriteLine(mongoProcess.StandardOutput.ReadLine() ?? string.Empty);
        }

        private MongoServer server;

        public MongoDatabase Database
        {
            get
            {
                if (!MongoAvailable)
                    throw new InvalidOperationException("Mongo server is not available");

                if (null == server)
                {
                    server = new MongoServer(new MongoServerSettings
                    {
                        ConnectTimeout = TimeSpan.FromSeconds(5),
                        Server = new MongoServerAddress(mongoHost.Value, mongoPort.Value)
                    });
                    server.Connect();
                }
                return server.GetDatabase(DatabaseName);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (processStarted && mongoProcess != null)
            {
                if (!mongoProcess.HasExited)
                {
                    mongoProcess.Close();
                    if (!mongoProcess.WaitForExit(5000))
                        mongoProcess.Kill();
                }
            }
            mongoProcess = null;
        }

        /// <summary>
        ///     Gets a value indicating whether or not MongoDB is considered as being available
        /// </summary>
        public bool MongoAvailable
        {
            get
            {
                if (runAsService.Value)
                {
                    var controller = GetController();
                    return controller != null && controller.Status == ServiceControllerStatus.Running;
                }
                if (null == mongoProcess)
                {
                    var file = mongoFile.Value;
                    var processes = Process.GetProcessesByName(file.Remove(file.LastIndexOf('.')));
                    var p = processes.FirstOrDefault();
                    if (null != p)
                        mongoProcess = p;
                }
                return (mongoProcess != null && !mongoProcess.HasExited);
            }
        }

        private ServiceController GetController()
        {
            ServiceController controller;
            try
            {
                controller =
                    ServiceController.GetServices(mongoServiceMachineName.Value)
                                     .Single(sc => sc.ServiceName == mongoServiceName.Value);
            }
            catch
            {
                controller = null;
            }
            return controller;
        }

        private ProcessStartInfo GetInfo(string mongoExe)
        {
            var info = new ProcessStartInfo
                           {
                               FileName = mongoExe,
                               Arguments = mongoArgs.Value,
#if DEBUG
                               UseShellExecute = true,
#else
                               UseShellExecute = false,
                               RedirectStandardOutput = true,
                               CreateNoWindow = true,
#endif
                           };
            return info;
        }

        /// <summary>
        ///     Attempts to Start MongoDB on the local machine
        ///     <para />
        ///     This method will throw an <see cref="InvalidOperationException" /> if mongo is already available
        /// </summary>
        //[PrincipalPermission(SecurityAction.Demand, Role = @"BUILTIN\Administrators")]
        public void StartMongo()
        {
            if (MongoAvailable)
                throw new InvalidOperationException("MongoDB is already running");
            var path = dbPath.Value;
            var exefile = mongoFile.Value;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            var controller = GetController();
            if (controller == null)
            {
                string mongoExe;
                if (!File.Exists((mongoExe = Path.Combine(Environment.CurrentDirectory, exefile))))
                    throw new FileNotFoundException("Unable to start mongod - file not found");
                mongoProcess = new Process { StartInfo = GetInfo(mongoExe) };
                if (runAsService.Value)
                {
                    mongoProcess.Start();
                    mongoProcess.WaitForExit();
                }
                else
                {
                    mongoProcess.Exited += MongoProcessOnExited;
                    mongoProcess.Start();
                    while (!mongoProcess.Responding)
                        Thread.CurrentThread.Join(1);
                    // TODO: Could this be refined somehow?
                    Thread.CurrentThread.Join(TimeSpan.FromMilliseconds(1000));
                    processStarted = true;
                    return;
                }
                controller = GetController();
            }
            if (controller == null)
                throw new InvalidOperationException("Unable to get ServiceController for MongoDB Service");
            if (!runAsService.Value) return;
            if (controller.Status == ServiceControllerStatus.StartPending ||
                controller.Status == ServiceControllerStatus.Running) return;
            controller.Start();
            controller.WaitForStatus(ServiceControllerStatus.Running);
        }

        private void MongoProcessOnExited(object sender, EventArgs eventArgs)
        {
            LogWarning("Mongo Process has exited. {0}", eventArgs);
        }

        /// <summary>
        ///     Attempts to get an <see cref="IDataAdapter{T}" /> for the specified type <typeparamref name="T" />
        /// </summary>
        /// <typeparam name="T">The type for which to get an adapter</typeparam>
        /// <returns>
        ///     Returns a <see cref="MongoDBAdapter{T}" />
        /// </returns>
        public MongoDBAdapter<T> GetAdapter<T>() where T : ILibraryEntity
        {
            return new MongoDBAdapter<T>(this);
        }

        protected string DatabaseName { get; private set; }
    }
}