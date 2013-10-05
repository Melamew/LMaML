using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.AvalonDock.Layout;

namespace LMaML.Infrastructure.Docking
{
    public class DockingServiceLayoutUpdateStrategy : ILayoutUpdateStrategy
    {
        #region Implementation of ILayoutUpdateStrategy

        public bool BeforeInsertAnchorable(LayoutRoot layout,
                                           LayoutAnchorable anchorableToShow,
                                           ILayoutContainer destinationContainer)
        {
            return false;
        }

        public void AfterInsertAnchorable(LayoutRoot layout,
                                          LayoutAnchorable anchorableShown)
        {
        }

        public bool BeforeInsertDocument(LayoutRoot layout,
                                         LayoutDocument anchorableToShow,
                                         ILayoutContainer destinationContainer)
        {
            return false;
        }

        public void AfterInsertDocument(LayoutRoot layout,
                                        LayoutDocument anchorableShown)
        {
        }

        #endregion
    }
}
