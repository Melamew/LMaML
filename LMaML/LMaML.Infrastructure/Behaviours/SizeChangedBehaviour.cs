using System.Windows;
using System.Windows.Input;

namespace LMaML.Infrastructure.Behaviours
{
    /// <summary>
    /// 
    /// </summary>
    public class SizeContainer
    {
        private readonly Size size;
        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public double Height { get { return size.Height; } }

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public double Width { get { return size.Width; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="SizeContainer" /> class.
        /// </summary>
        /// <param name="size">The size.</param>
        public SizeContainer(Size size)
        {
            this.size = size;
        }
    }
    /// <summary>
    /// SizeChangedBehaviour
    /// </summary>
    public class SizeChangedBehaviour
    {
        /// <summary>
        /// The size changed command property
        /// <para/>
        /// Note that the command parameter for this command will be the new "Size" (<see cref="Size"/>)
        /// </summary>
        public static readonly DependencyProperty SizeChangedCommandProperty =
            DependencyProperty.RegisterAttached("SizeChangedCommand", typeof (ICommand), typeof (SizeChangedBehaviour), new PropertyMetadata(default(ICommand), PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var element = dependencyObject as FrameworkElement;
            if (null == element) return;
            element.SizeChanged -= ElementOnSizeChanged;
            if (null == dependencyPropertyChangedEventArgs.NewValue) return;
            element.SizeChanged += ElementOnSizeChanged;
        }

        /// <summary>
        /// Elements the on size changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="sizeChangedEventArgs">The <see cref="SizeChangedEventArgs" /> instance containing the event data.</param>
        private static void ElementOnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            var element = sender as FrameworkElement;
            if (null == element) return; // Wat!?
            var command = GetSizeChangedCommand(element);
            if (null == command)
            {
                element.SizeChanged -= ElementOnSizeChanged;
                return;
            }
            command.Execute(new SizeContainer(sizeChangedEventArgs.NewSize));
        }

        /// <summary>
        /// Sets the size changed command.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">The value.</param>
        public static void SetSizeChangedCommand(FrameworkElement element, ICommand value)
        {
            element.SetValue(SizeChangedCommandProperty, value);
        }

        /// <summary>
        /// Gets the size changed command.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static ICommand GetSizeChangedCommand(FrameworkElement element)
        {
            return (ICommand) element.GetValue(SizeChangedCommandProperty);
        }
    }
}
