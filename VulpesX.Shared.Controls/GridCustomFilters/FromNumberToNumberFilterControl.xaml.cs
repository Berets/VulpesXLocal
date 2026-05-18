using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Data;

namespace VulpesX.Shared.Controls.GridCustomFilters
{
    /// <summary>
    /// Interaction logic for FromNumberToNumberFilterControl.xaml
    /// </summary>
    public partial class FromNumberToNumberFilterControl : UserControl, IFilteringControl
    {
        private GridViewBoundColumnBase? column;
        private CompositeFilterDescriptor? compositeFilter;
        private Telerik.Windows.Data.FilterDescriptor? fromFilter;
        private Telerik.Windows.Data.FilterDescriptor? toFilter;

        public event EventHandler? FilterApplied;
        public event EventHandler? FilterCleared;
        public required string Member { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the filtering is active.
        /// </summary>
        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="IsActive"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register(
                "IsActive",
                typeof(bool),
                typeof(FromNumberToNumberFilterControl),
                new System.Windows.PropertyMetadata(false));

        [TypeConverter(typeof(DecimalTypeConverter))]
        public decimal? From
        {
            get { return (decimal?)this.numFrom.Value; }
            set { this.numFrom.Value = (double?)value; }
        }

        [TypeConverter(typeof(DecimalTypeConverter))]
        public decimal? To
        {
            get { return (decimal?)this.numTo.Value; }
            set { this.numTo.Value = (double?)value; }
        }

        public RadNumericUpDown FromPicker
        {
            get { return this.numFrom; }
        }

        public RadNumericUpDown ToPicker
        {
            get { return this.numTo; }
        }

        public FromNumberToNumberFilterControl()
        {
            InitializeComponent();
        }

        public void Prepare(Telerik.Windows.Controls.GridViewColumn column)
        {
            if (column is GridViewBoundColumnBase)
            {
                this.column = (column as GridViewBoundColumnBase)!;

                if (this.column == null || fromFilter == null || toFilter == null)
                {
                    return;
                }

                if (this.compositeFilter == null)
                {
                    this.CreateFilters();
                }

                this.fromFilter.Value = this.From;
                this.toFilter.Value = this.To;
            }
        }

        public void CreateFilters()
        {
            if (this.column == null)
            {
                return;
            }
            string dataMember = this.column.DataMemberBinding.Path.Path;
            this.compositeFilter = new CompositeFilterDescriptor();

            this.fromFilter = new FilterDescriptor(dataMember, FilterOperator.IsGreaterThanOrEqualTo, null);
            this.toFilter = new FilterDescriptor(dataMember, FilterOperator.IsLessThanOrEqualTo, null);

        }

        public void OnFilter(object sender, RoutedEventArgs e)
        {
            if (compositeFilter == null || column == null || fromFilter == null || toFilter == null)
            {
                return;
            }

            this.fromFilter.Value = this.From;
            this.toFilter.Value = this.To;

            this.compositeFilter.FilterDescriptors.Clear();

            if (this.From.HasValue)
                this.compositeFilter.FilterDescriptors.Add(this.fromFilter);

            if (this.To.HasValue)
                this.compositeFilter.FilterDescriptors.Add(this.toFilter);

            // Nothing to filter on — clear and bail out
            if (this.compositeFilter.FilterDescriptors.Count == 0)
            {
                OnClear(sender, e);
                return;
            }

            if (!this.column.DataControl.FilterDescriptors.Contains(this.compositeFilter))
                this.column.DataControl.FilterDescriptors.Add(this.compositeFilter);

            this.IsActive = true;


            var popup = this.ParentOfType<Popup>();
            if (popup != null)
            {
                popup.IsOpen = false;
            }

            FilterApplied?.Invoke(this, EventArgs.Empty);
        }


        private void OnClear(object sender, RoutedEventArgs e)
        {
            if (this.column == null)
            {
                return;
            }

            if (this.column.DataControl.FilterDescriptors.Contains(this.compositeFilter))
            {
                this.column.DataControl.FilterDescriptors.Remove(this.compositeFilter);
            }

            this.From = null;
            this.To = null;

            this.IsActive = false;

            var popup = this.ParentOfType<Popup>();
            if (popup != null)
            {
                popup.IsOpen = false;
            }

            FilterCleared?.Invoke(this, EventArgs.Empty);
        }
    }
}
