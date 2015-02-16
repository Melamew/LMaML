using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;
using iLynx.Common.Pixels;
using iLynx.Configuration;
using iLynx.Serialization.Xml;
using LMaML.Infrastructure.Events;
using LMaML.Infrastructure.Services.Interfaces;
using Microsoft.Practices.Unity;
using iLynx.Common;

namespace LMaML
{

    public class PaletteSerializer : XmlSerializerBase<LinearGradientPalette>
    {
        private readonly IXmlSerializer<double> doubleSerializer;
        private readonly IXmlSerializer<Color> colourSerializer;

        public PaletteSerializer()
        {
            doubleSerializer = XmlSerializerService.GetSerializer<double>();
            colourSerializer = XmlSerializerService.GetSerializer<Color>();
        }

        #region Overrides of XmlSerializerBase<IPalette<double>>

        public override void Serialize(LinearGradientPalette item, XmlWriter writer)
        {
            var map = item.GetMap();
            writer.WriteStartElement("Palette");
            try
            {
                writer.WriteAttributeString("Count", map.Length.ToString(CultureInfo.InvariantCulture));
                foreach (var element in map)
                {
                    doubleSerializer.Serialize(element.Item1, writer);
                    colourSerializer.Serialize(element.Item2, writer);
                }
            }
            finally { writer.WriteEndElement(); }
        }

        public override LinearGradientPalette Deserialize(XmlReader reader)
        {
            reader.SkipToElement("Palette");
            var result = new LinearGradientPalette();
            try
            {
                var countAttrib = reader.GetAttribute("Count");
                int count;
                if (!int.TryParse(countAttrib, out count)) return null;
                var target = new Tuple<double, Color>[count];
                if (reader.IsEmptyElement) return result;
                reader.ReadStartElement("Palette");
                for (var i = 0; i < count; ++i)
                {
                    var item1 = doubleSerializer.Deserialize(reader);
                    var item2 = colourSerializer.Deserialize(reader);
                    target[i] = new Tuple<double, Color>(item1, item2);
                }
                result.FromMap(target);
                return result;
            }
            finally
            {
                if (reader.IsEmptyElement)
                    reader.Skip();
                else
                    reader.ReadEndElement();
            }
        }
        #endregion
    }

    public class PointSerializer : XmlSerializerBase<Point>
    {
        public override void Serialize(Point item, XmlWriter writer)
        {
            writer.WriteElementString(typeof(Point).Name,
                string.Format(CultureInfo.InvariantCulture, "{0},{1}", item.X, item.Y));
        }

        public override Point Deserialize(XmlReader reader)
        {
            var values = reader.ReadElementString(typeof (Point).Name).Split(',');
            return new Point(double.Parse(values[0], CultureInfo.InvariantCulture), double.Parse(values[1], CultureInfo.InvariantCulture));
        }
    }

    public class RectSerializer : XmlSerializerBase<Rect>
    {
        public override void Serialize(Rect item, XmlWriter writer)
        {
            writer.WriteElementString(typeof(Rect).Name,
                string.Format(CultureInfo.InvariantCulture, "{0},{1},{2},{3}", item.Left, item.Top, item.Width, item.Height));
        }

        public override Rect Deserialize(XmlReader reader)
        {
            var values = reader.ReadElementString(typeof (Rect).Name).Split(',');
            var x = double.Parse(values[0], CultureInfo.InvariantCulture);
            var y = double.Parse(values[1], CultureInfo.InvariantCulture);
            var width = double.Parse(values[2], CultureInfo.InvariantCulture);
            var height = double.Parse(values[3], CultureInfo.InvariantCulture);
            return new Rect(x, y, width, height);
        }
    }


    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private IPublicTransport publicTransport;
        private ILogger logger;


        public App()
        {
            XmlSerializerService.AddOverride(new PaletteSerializer());
            XmlSerializerService.AddOverride(new PointSerializer());
            XmlSerializerService.AddOverride(new RectSerializer());
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            DispatcherUnhandledException += OnDispatcherUnhandledException;
            base.OnStartup(e);
#if DEBUG
            RunDebug();
#else
            RunRelease();
#endif
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Exit" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.Windows.ExitEventArgs" /> that contains the event data.</param>
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            if (shutdownOnEvent) return;
            if (null == publicTransport)
                return;
            if (null == publicTransport.ApplicationEventBus)
            {
                logger.Log(LogLevel.Error, this, "Cannot find Event bus to notify application shutdown");
                return;
            }
            publicTransport.ApplicationEventBus.Send(new ShutdownEvent());
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs dispatcherUnhandledExceptionEventArgs)
        {
            RuntimeCommon.DefaultLogger.Log(LogLevel.Error, this, string.Format("Unhandled Exception:{0}{1}", Environment.NewLine, dispatcherUnhandledExceptionEventArgs.Exception));
        }

        private bool shutdownOnEvent = false;

        /// <summary>
        /// Runs the debug.
        /// </summary>
        private void RunDebug()
        {
            var bootstrapper = new Bootstrapper();
            bootstrapper.Run(true);
            publicTransport = bootstrapper.Container.Resolve<IPublicTransport>();
            logger = bootstrapper.Container.Resolve<ILogger>();
            publicTransport.ApplicationEventBus.Subscribe<ShutdownEvent>(OnShutdown);
        }

        private void OnShutdown(ShutdownEvent shutdownEvent)
        {
            shutdownOnEvent = true;
            Shutdown();
            ExeConfig.Save();
        }

        private void RunRelease()
        {
            var bootstrapper = new Bootstrapper();
            try
            {
                bootstrapper.Run();
                publicTransport = bootstrapper.Container.Resolve<IPublicTransport>();
                logger = bootstrapper.Container.Resolve<ILogger>();
                publicTransport.ApplicationEventBus.Subscribe<ShutdownEvent>(OnShutdown);
            }
            catch (Exception e) { RuntimeCommon.DefaultLogger.Log(LogLevel.Critical, this, string.Format("APPFAILURE:{0}{1}", Environment.NewLine, e)); }
        }
    }
}
