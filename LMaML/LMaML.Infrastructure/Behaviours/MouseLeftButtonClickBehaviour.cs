using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LMaML.Infrastructure.Behaviours
{
    /// <summary>
    /// MouseButtonClickBehaviour
    /// </summary>
    public class MouseLeftButtonClickBehaviour : DependencyObject
    {
        /// <summary>
        /// The command property
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof (ICommand), typeof (MouseLeftButtonClickBehaviour), new PropertyMetadata(default(ICommand), OnCommandChangedCallback));

        /// <summary>
        /// Called when [command changed callback].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="dependencyPropertyChangedEventArgs">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnCommandChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = dependencyObject as Control;
            if (null == control) return;
            control.MouseLeftButtonDown -= ControlOnMouseUp;
            control.MouseLeftButtonDown += ControlOnMouseUp;
        }

        /// <summary>
        /// Controls the on mouse up.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="mouseButtonEventArgs">The <see cref="MouseButtonEventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private static void ControlOnMouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            var control = sender as Control;
            if (null == control) return;
            var command = GetCommand(control);
            if (null == command) return;
            command.Execute(GetCommandParameter(control));
        }

        /// <summary>
        /// The command parameter property
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached("CommandParameter", typeof (object), typeof (MouseLeftButtonClickBehaviour), new PropertyMetadata(default(object)));

        /// <summary>
        /// Sets the command parameter.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetCommandParameter(UIElement element, object value)
        {
            element.SetValue(CommandParameterProperty, value);
        }

        /// <summary>
        /// Gets the command parameter.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static object GetCommandParameter(UIElement element)
        {
            return element.GetValue(CommandParameterProperty);
        }

        /// <summary>
        /// Sets the command.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetCommand(UIElement element, ICommand value)
        {
            element.SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static ICommand GetCommand(UIElement element)
        {
            return (ICommand) element.GetValue(CommandProperty);
        }
    }
}
