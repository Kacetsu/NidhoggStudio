using ns.Base.Attribute;
using ns.Base.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ns.Base.Extensions;
using ns.Base.Plugins.Properties;

namespace ns.Plugin.Base {
    [Visible, Serializable]
    public class CheckIntensity : Tool {
        private RectangleProperty _aoiProperty;
        private ImageProperty _inputImage;
        private DoubleProperty _intensityProperty;

        public override string Category {
            get {
                return ToolCategory.Common.GetDescription();
            }
        }

        public override string Description {
            get {
                return string.Empty;
            }
        }

        public CheckIntensity() {
            DisplayName = "Check Intensity";
            AddChild(new RectangleProperty("AOI", 0.0, 0.0, 0.0, 0.0));
            AddChild(new ImageProperty("InputImage", false));
            AddChild(new DoubleProperty("Intensity", true));
        }

        public override bool Initialize() {
            base.Initialize();

            _aoiProperty = GetProperty("AOI") as RectangleProperty;
            _inputImage = GetProperty("InputImage") as ImageProperty;
            _intensityProperty = GetProperty("Intensity") as DoubleProperty;
            return true;
        }
    }
}
