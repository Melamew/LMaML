//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Reflection;
//using System.Windows;
//using LMaML.Infrastructure.Services.Interfaces;
//using iLynx.Common;

//namespace LMaML.Infrastructure.Services.Implementations
//{
//    /// <summary>
//    /// RegionManagerService
//    /// </summary>
//    public class RegionManagerService : ComponentBase, IRegionManagerService
//    {
//        /// <summary>
//        /// The regions
//        /// </summary>
//        private static readonly Dictionary<string, object> Regions = new Dictionary<string, object>();

//        /// <summary>
//        /// The region name property
//        /// </summary>
//        public static readonly DependencyProperty RegionNameProperty = DependencyProperty.RegisterAttached(
//            "RegionName", typeof(string), typeof(RegionManagerService), new PropertyMetadata(default(string), OnRegionNameChanged));

//        //public RegionManagerService() : base(new ConsoleLogger()) { }

//        public RegionManagerService(ILogger logger) : base(logger) { }

//        /// <summary>
//        /// Sets the name of the region.
//        /// </summary>
//        /// <param name="element">The element.</param>
//        /// <param name="value">The value.</param>
//        public static void SetRegionName(DependencyObject element, string value)
//        {
//            element.SetValue(RegionNameProperty, value);
//        }

//        /// <summary>
//        /// Gets the name of the region.
//        /// </summary>
//        /// <param name="element">The element.</param>
//        /// <returns></returns>
//        public static string GetRegionName(DependencyObject element)
//        {
//            return (string)element.GetValue(RegionNameProperty);
//        }

//        /// <summary>
//        /// Called when [region name changed].
//        /// </summary>
//        /// <param name="dependencyObject">The dependency object.</param>
//        /// <param name="dependencyPropertyChangedEventArgs">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
//        private static void OnRegionNameChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
//        {
//            var newValue = dependencyPropertyChangedEventArgs.NewValue as string;
//            var oldValue = dependencyPropertyChangedEventArgs.OldValue as string;
//            if (string.IsNullOrEmpty(newValue) && !string.IsNullOrEmpty(oldValue))
//            {
//                Regions.Remove(oldValue);
//                return;
//            }
//            if (string.IsNullOrEmpty(newValue) || Regions.ContainsKey(newValue)) return;
//            Regions.Add(newValue, dependencyObject);
//            Trace.WriteLine(string.Format("Added Region: {0}", newValue));
//        }

//        /// <summary>
//        /// Registers the view with region.
//        /// </summary>
//        /// <param name="regionName">Name of the region.</param>
//        /// <param name="view">The view.</param>
//        public void RegisterViewWithRegion(string regionName, object view)
//        {
//            object region;
//            if (!Regions.TryGetValue(regionName, out region)) return;
//            var dyn = (dynamic)region;
//            Exception ex = null;
//            Exception ex2 = null;
//            try
//            {
//                dyn.Content = view;
//            }
//            catch (MissingMemberException e)
//            {
//                ex = e;
//            }
//            if (null == ex) return;
//            try
//            {
//                dyn.Children.Add(view);
//                ex = null;
//            }
//            catch (MissingMethodException e2)
//            {
//                ex2 = e2;
//            }
//            catch (MissingMemberException e3)
//            {
//                ex2 = e3;
//            }
//            LogException(ex, MethodBase.GetCurrentMethod());
//            LogException(ex2, MethodBase.GetCurrentMethod());
//        }
//    }
//}
