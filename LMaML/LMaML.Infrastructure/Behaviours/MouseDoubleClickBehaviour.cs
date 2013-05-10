using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LMaML.Infrastructure.Behaviours
{
    /// <summary>
    /// MouseDoubleClickBehaviour
    /// </summary>
    public class MouseDoubleClickBehaviour : DependencyObject
    {
        /// <summary>
        /// The command property
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(MouseDoubleClickBehaviour), new PropertyMetadata(default(ICommand), OnCommandChanged));

        /// <summary>
        /// The command parameter property
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter", typeof (object), typeof (MouseDoubleClickBehaviour), new PropertyMetadata(default(object)));

        /// <summary>
        /// Sets the command parameter.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="value">The value.</param>
        public static void SetCommandParameter(Control control, object value)
        {
            control.SetValue(CommandParameterProperty, value);
        }

        /// <summary>
        /// Gets the command parameter.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <returns></returns>
        public static object GetCommandParameter(Control control)
        {
            return control.GetValue(CommandParameterProperty);
        }

        /// <summary>
        /// Called when [command changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="dependencyPropertyChangedEventArgs">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnCommandChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = dependencyObject as Control;
            if (null == control) return;
            control.MouseDoubleClick -= ControlOnMouseDoubleClick;
            control.MouseDoubleClick += ControlOnMouseDoubleClick;
        }

        /// <summary>
        /// Controls the on mouse double click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="mouseButtonEventArgs">The <see cref="MouseButtonEventArgs" /> instance containing the event data.</param>
        private static void ControlOnMouseDoubleClick(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var control = sender as Control;
            if (null == control) return;
            var command = GetCommand(control);
            command.Execute(GetCommandParameter(control));
        }

        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static ICommand GetCommand(Control source)
        {
            return (ICommand)source.GetValue(CommandProperty);
        }

        /// <summary>
        /// Sets the command.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        public static void SetCommand(Control target, ICommand value)
        {
            target.SetValue(CommandProperty, value);
        }
    }
}
