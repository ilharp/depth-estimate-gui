using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace DepthEstimateGui.Controls
{
    public class DrawingIcon : TemplatedControl
    {
        #region Constructor

        static DrawingIcon()
        {
            WidthProperty.OverrideDefaultValue<DrawingIcon>(16);
            HeightProperty.OverrideDefaultValue<DrawingIcon>(16);
        }

        #endregion

        #region Icon

        public static readonly StyledProperty<Drawing> IconProperty =
            AvaloniaProperty.Register<DrawingIcon, Drawing>(nameof(Icon));

        public Drawing Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        #endregion
    }
}
