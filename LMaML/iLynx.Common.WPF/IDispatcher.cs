using System;

namespace iLynx.Common.WPF
{
    /// <summary>
    /// IDispatcher
    /// </summary>
    public interface IDispatcher
    {
        /// <summary>
        /// Invokes the specified method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        object Invoke(Delegate method, TimeSpan timeout, params object[] args);

        /// <summary>
        /// Begins the invoke.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="args">The args.</param>
        void BeginInvoke(Delegate method, params object[] args);

        /// <summary>
        /// Invokes the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        void Invoke(Action action);

        /// <summary>
        /// Invokes the specified action.
        /// </summary>
        /// <typeparam name="TParam">The type of the param.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="param">The param.</param>
        void Invoke<TParam>(Action<TParam> action, TParam param);

        /// <summary>
        /// Invokes the specified func.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="func">The func.</param>
        /// <returns></returns>
        TResult Invoke<TResult>(Func<TResult> func);
    }
}