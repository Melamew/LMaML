using System;
using System.Windows;
using System.Windows.Input;

namespace LMaML.Infrastructure.Behaviours
{
    /// <summary>
    /// KeyUpDownBehaviour
    /// </summary>
    public class KeyUpDownBehaviour
    {
        /// <summary>
        /// The key down command property
        /// </summary>
        public static readonly DependencyProperty KeyDownCommandProperty =
            DependencyProperty.RegisterAttached("KeyDownCommand", typeof (ICommand), typeof (KeyUpDownBehaviour), new PropertyMetadata(default(ICommand), KeyDownCommandChanged));

        public static readonly DependencyProperty KeyUpCommandProperty =
            DependencyProperty.RegisterAttached("KeyUpCommand", typeof (ICommand), typeof (KeyUpDownBehaviour), new PropertyMetadata(default(ICommand), KeyUpCommandChanged));

        private static void KeyUpCommandChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var element = dependencyObject as UIElement;
            if (null == element) return;
            element.KeyUp -= ElementOnKeyUp;
            if (null == dependencyPropertyChangedEventArgs.NewValue) return;
            element.KeyUp += ElementOnKeyUp;
        }

        /// <summary>
        /// Elements the on key up.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="KeyEventArgs" /> instance containing the event data.</param>
        private static void ElementOnKeyUp(object sender, KeyEventArgs e)
        {
            var element = sender as UIElement;
            if (null == element) return;
            var cmd = GetKeyUpCommand(element);
            if (null == cmd) return;
            cmd.Execute(e);
        }

        public static void SetKeyUpCommand(UIElement element, ICommand value)
        {
            element.SetValue(KeyUpCommandProperty, value);
        }

        public static ICommand GetKeyUpCommand(UIElement element)
        {
            return (ICommand) element.GetValue(KeyUpCommandProperty);
        }

        /// <summary>
        /// Keys down command changed.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="dependencyPropertyChangedEventArgs">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void KeyDownCommandChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var element = dependencyObject as UIElement;
            if (null == element) return;
            element.KeyDown -= ElementOnKeyDown;
            if (null == dependencyPropertyChangedEventArgs.NewValue) return;
            element.KeyDown += ElementOnKeyDown;
        }

        /// <summary>
        /// Elements the on key down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="keyEventArgs">The <see cref="KeyEventArgs" /> instance containing the event data.</param>
        private static void ElementOnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            var element = sender as UIElement;
            if (null == element) return;
            var cmd = GetKeyDownCommand(element);
            if (null == cmd) return;
            cmd.Execute(keyEventArgs);
        }

        /// <summary>
        /// Sets the key down command.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetKeyDownCommand(UIElement element, ICommand value)
        {
            element.SetValue(KeyDownCommandProperty, value);
        }

        /// <summary>
        /// Gets the key down command.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static ICommand GetKeyDownCommand(UIElement element)
        {
            return (ICommand) element.GetValue(KeyDownCommandProperty);
        }
    }
}
