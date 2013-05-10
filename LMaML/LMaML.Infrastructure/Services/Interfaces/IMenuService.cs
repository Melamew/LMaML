using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace LMaML.Infrastructure.Services.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMenuItem
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; }
        /// <summary>
        /// Gets the sub items.
        /// </summary>
        /// <value>
        /// The sub items.
        /// </value>
        IEnumerable<IMenuItem> SubItems { get; }
        /// <summary>
        /// Clicks this instance.
        /// </summary>
        ICommand Command { get; }

        /// <summary>
        /// Occurs when [changed].
        /// </summary>
        event Action Changed;
    }
    
    /// <summary>
    /// IMenuService
    /// </summary>
    public interface IMenuService
    {
        /// <summary>
        /// Gets the root menus.
        /// </summary>
        /// <value>
        /// The root menus.
        /// </value>
        IEnumerable<IMenuItem> RootMenus { get; }

        /// <summary>
        /// Registers the root.
        /// </summary>
        /// <param name="root">The root.</param>
        void Register(IMenuItem root);
    }

    /// <summary>
    /// IContextMenuService
    /// </summary>
    public interface IContextMenuService
    {
        /// <summary>
        /// Tries the get for.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        bool TryGetFor<T>(out IEnumerable<IMenuItem> result);
    }
}
