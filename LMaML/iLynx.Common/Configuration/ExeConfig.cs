using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace iLynx.Common.Configuration
{
    /// <summary>
    ///     Simple helper class that contains a <see cref="System.Configuration.Configuration" /> object for the entry assembly
    /// </summary>
    public static class ExeConfig
    {
        private static System.Configuration.Configuration configuration;

        /// <summary>
        ///     Gets a <see cref="System.Configuration.Configuration" /> object for the entry assembly
        /// </summary>
        public static System.Configuration.Configuration Configuration
        {
            get
            {
                return configuration ??
                       (configuration = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location));
            }
        }

        private static BinaryConfigSection configurableValuesSection;
        private const string BinaryConfigFile = "Configuration.bin";
        private readonly static string TargetPath = Path.Combine(Environment.CurrentDirectory, BinaryConfigFile);

        /// <summary>
        /// Gets the configurable values section.
        /// </summary>
        /// <value>
        /// The configurable values section.
        /// </value>
        public static BinaryConfigSection ConfigurableValuesSection
        {
            get { return configurableValuesSection ?? (configurableValuesSection = GetConfigurableValueSection()); }
        }

        private static BinaryConfigSection GetConfigurableValueSection()
        {
            if (null != configurableValuesSection) return configurableValuesSection;
            configurableValuesSection = Load();
            Save();
            return configurableValuesSection;
        }

        private static BinaryConfigSection Load()
        {
            var result = new BinaryConfigSection();
            if (!File.Exists(TargetPath)) return result;
            using (var source = File.OpenRead(TargetPath))
                result.ReadFrom(source);
            return result;
        }

        public static void Save()
        {
            if (File.Exists(TargetPath))
                File.Delete(TargetPath);
            using (var target = File.Open(TargetPath, FileMode.Create))
                ConfigurableValuesSection.WriteTo(target);
        }
    }
}