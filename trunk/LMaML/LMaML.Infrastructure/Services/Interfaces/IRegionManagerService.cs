using System;

namespace LMaML.Infrastructure.Services.Interfaces
{
    public interface IRegionManagerService
    {
        /// <summary>
        /// Registers the view with region.
        /// </summary>
        /// <param name="regionName">Name of the region.</param>
        /// <param name="view">The view.</param>
        IRegionManagerService AddToRegion(string regionName, object view);

        /// <summary>
        /// Registers the view with region.
        /// </summary>
        /// <param name="regionName">Name of the region.</param>
        /// <param name="contentDelegate">The content delegate.</param>
        IRegionManagerService RegisterViewWithRegion(string regionName, Func<object> contentDelegate);

        /// <summary>
        /// Registers the view with region.
        /// </summary>
        /// <param name="regionName">Name of the region.</param>
        /// <param name="viewType">Type of the view.</param>
        /// <returns></returns>
        IRegionManagerService RegisterViewWithRegion(string regionName, Type viewType);
    }
}