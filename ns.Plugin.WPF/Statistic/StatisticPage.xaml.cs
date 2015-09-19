using ns.Base;
using ns.Base.Plugins.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ns.Plugin.WPF.Statistic {
    /// <summary>
    /// Interaction logic for StatisticPage.xaml
    /// </summary>
    public partial class StatisticPage : TabItem {

        public class StatisticModel {
            public class Information : INotifyPropertyChanged {
                private double _value = 0.0;

                /// <summary>
                /// Gets or sets the name.
                /// </summary>
                /// <value>
                /// The name.
                /// </value>
                public string Name { get; set; }

                /// <summary>
                /// Gets or sets the value.
                /// </summary>
                /// <value>
                /// The value.
                /// </value>
                public double Value {
                    get { return _value; }
                    set {
                        _value = value;
                        if (PropertyChanged != null) {
                            PropertyChanged(this, new PropertyChangedEventArgs("Value"));
                        }
                    }
                }

                public event PropertyChangedEventHandler PropertyChanged;
            }

            /// <summary>
            /// Gets the statistics.
            /// </summary>
            /// <value>
            /// The statistics.
            /// </value>
            public ObservableCollection<Information> Statistics { get; private set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="StatisticModel"/> class.
            /// </summary>
            public StatisticModel() {
                Statistics = new ObservableCollection<Information>();
                Statistics.Add(new Information() { Name = "Min", Value = 0.0 });
                Statistics.Add(new Information() { Name = "Max", Value = 0.0 });
                Statistics.Add(new Information() { Name = "Mean", Value = 0.0 });
                Statistics.Add(new Information() { Name = "Median", Value = 0.0 });
            }

            private object selectedItem = null;
            /// <summary>
            /// Gets or sets the selected item.
            /// </summary>
            /// <value>
            /// The selected item.
            /// </value>
            public object SelectedItem {
                get { return selectedItem; }
                set { selectedItem = value; }
            }

            /// <summary>
            /// Replaces the specified category.
            /// </summary>
            /// <param name="category">The category.</param>
            /// <param name="value">The value.</param>
            public void Replace(string category, double value) {
                bool contains = false;
                foreach (Information info in Statistics) {
                    if (info.Name == category) {
                        info.Value = value;
                        contains = true;
                        break;
                    }
                }
                if (!contains)
                    Statistics.Add(new Information() { Name = category, Value = value });
            }
        }

        private StatisticModel _statisticModel;

        private DataStorageContainer _container;
        private Property _property;

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <value>
        /// The container.
        /// </value>
        public DataStorageContainer Container {
            get { return _container; }
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <value>
        /// The property.
        /// </value>
        public Property Property {
            get { return _property; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticPage"/> class.
        /// </summary>
        public StatisticPage() {
            InitializeComponent();
            _statisticModel = new StatisticModel();
            this.Style = new Style(GetType(), this.FindResource(typeof(TabItem)) as Style);
            this.DataContext = _statisticModel;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticPage"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="container">The container.</param>
        public StatisticPage(Property property, DataStorageContainer container) {
            InitializeComponent();
            _statisticModel = new StatisticModel();
            this.Style = new Style(GetType(), this.FindResource(typeof(TabItem)) as Style);
            this.DataContext = _statisticModel;
            _property = property;
            this.Header = _property.TreeName;
            _property.PropertyChanged += PropertyChanged;
            _container = container;
        }

        private double CalculateMedian(double[] xs) {
            var ys = xs.OrderBy(x => x).ToList();
            double mid = (ys.Count - 1) / 2.0;
            return (ys[(int)(mid)] + ys[(int)(mid + 0.5)]) / 2;
        }

        /// <summary>
        /// Updates the container.
        /// </summary>
        /// <param name="container">The container.</param>
        public void UpdateContainer(DataStorageContainer container) {
            double min = Convert.ToDouble(container.Values.Min());
            double max = Convert.ToDouble(container.Values.Max());
            double mean = 0;
            List<double> valueList = new List<double>();

            double count = 0;
            for (int index = 0; index < container.Values.Count; index++) {
                if (container.Values[index] != null) {
                    double value = Convert.ToDouble(container.Values[index]);
                    valueList.Add(value);
                    mean += value;
                    count++;
                }
            }

            mean = mean / count;

            double[] values = valueList.ToArray();
            double median = CalculateMedian(values);

            _statisticModel.Replace("Min", min);
            _statisticModel.Replace("Max", max);
            _statisticModel.Replace("Mean", mean);
            _statisticModel.Replace("Median", median);
        }

        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == "Name" || e.PropertyName == "TreeName") {
                this.Header = _property.TreeName;
            }
        }
    }
}
