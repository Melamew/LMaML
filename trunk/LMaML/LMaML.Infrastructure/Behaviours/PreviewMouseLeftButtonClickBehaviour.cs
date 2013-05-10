using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LMaML.Infrastructure.Behaviours
{
    public class PreviewMouseLeftButtonClickBehaviour : DependencyObject
    {
        /// <summary>
        /// The command property
        /// </summary>
        public static readonly DependencyProperty PreviewLeftClickCommandProperty =
            DependencyProperty.RegisterAttached("PreviewLeftClickCommand", typeof(ICommand), typeof(PreviewMouseLeftButtonClickBehaviour), new PropertyMetadata(default(ICommand), OnCommandChangedCallback));

        /// <summary>
        /// Called when [command changed callback].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="dependencyPropertyChangedEventArgs">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnCommandChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var control = dependencyObject as Control;
            if (null == control) return;
            control.PreviewMouseLeftButtonDown -= ControlOnMouseUp;
            control.PreviewMouseLeftButtonDown += ControlOnMouseUp;
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
            var command = GetPreviewLeftClickCommand(control);
            if (null == command) return;
            command.Execute(GetPreviewLeftClickCommandParameter(control));
        }

        /// <summary>
        /// The command parameter property
        /// </summary>
        public static readonly DependencyProperty PreviewLeftClickCommandParameterProperty =
            DependencyProperty.RegisterAttached("PreviewLeftClickCommandParameter", typeof(object), typeof(PreviewMouseLeftButtonClickBehaviour), new PropertyMetadata(default(object)));

        /// <summary>
        /// Sets the command parameter.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetPreviewLeftClickCommandParameter(UIElement element, object value)
        {
            element.SetValue(PreviewLeftClickCommandParameterProperty, value);
        }

        /// <summary>
        /// Gets the command parameter.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static object GetPreviewLeftClickCommandParameter(UIElement element)
        {
            return element.GetValue(PreviewLeftClickCommandParameterProperty);
        }

        /// <summary>
        /// Sets the command.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetPreviewLeftClickCommand(UIElement element, ICommand value)
        {
            element.SetValue(PreviewLeftClickCommandProperty, value);
        }

        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static ICommand GetPreviewLeftClickCommand(UIElement element)
        {
            return (ICommand)element.GetValue(PreviewLeftClickCommandProperty);
        }
    }
}
