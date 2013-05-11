using System;
using LMaML.Infrastructure.Services.Interfaces;
using Microsoft.Practices.Prism.Regions;
using iLynx.Common;

namespace LMaML.Infrastructure.Services.Implementations
{
    /// <summary>
    /// RegionManagerService
    /// </summary>
    public class RegionManagerService : ComponentBase, IRegionManagerService
    {
        private readonly IRegionManager regionManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentBase" /> class.
        /// </summary>
        /// <param name="regionManager">The region manager.</param>
        /// <param name="logger">The logger.</param>
        public RegionManagerService(IRegionManager regionManager, ILogger logger) : base(logger)
        {
            this.regionManager = regionManager;
        }

        #region Implementation of IRegionManagerService

        /// <summary>
        /// Registers the view with region.
        /// </summary>
        /// <param name="regionName">Name of the region.</param>
        /// <param name="view">The view.</param>
        public IRegionManagerService AddToRegion(string regionName, object view)
        {
            regionManager.AddToRegion(regionName, view);
            return this;
        }

        /// <summary>
        /// Registers the view with region.
        /// </summary>
        /// <param name="regionName">Name of the region.</param>
        /// <param name="viewType">Type of the view.</param>
        /// <returns></returns>
        public IRegionManagerService RegisterViewWithRegion(string regionName, Type viewType)
        {
            regionManager.RegisterViewWithRegion(regionName, viewType);
            return this;
        }

        /// <summary>
        /// Registers the view with region.
        /// </summary>
        /// <param name="regionName">Name of the region.</param>
        /// <param name="contentDelegate">The content delegate.</param>
        public IRegionManagerService RegisterViewWithRegion(string regionName, Func<object> contentDelegate)
        {
            regionManager.RegisterViewWithRegion(regionName, contentDelegate);
            return this;
        }

        #endregion
    }
}
