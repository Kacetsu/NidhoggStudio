using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace ns.GUI.WPF {
    public class GuiHelper {
        public static DoubleAnimation DoubleAnimateControl(double size, UIElement control, DependencyProperty property) {
            DoubleAnimation animation = new DoubleAnimation(size, TimeSpan.FromSeconds(0.2));
            control.BeginAnimation(property, animation);
            return animation;
        }

        public static DoubleAnimation DoubleAnimateControl(double size, UIElement control, DependencyProperty property, TimeSpan timeSpan) {
            DoubleAnimation animation = new DoubleAnimation(size, timeSpan);
            control.BeginAnimation(property, animation);
            return animation;
        }

        public static DoubleAnimation DoubleAnimateControl(double from, double size, UIElement control, DependencyProperty property, TimeSpan timeSpan) {
            DoubleAnimation animation = new DoubleAnimation(from, size, timeSpan);
            control.BeginAnimation(property, animation);
            return animation;
        }
    }
}
