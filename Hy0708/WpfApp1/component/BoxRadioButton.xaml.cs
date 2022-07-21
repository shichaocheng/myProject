using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1.component
{ /// <summary>
  /// BoxRadioButton.xaml 的交互逻辑
  /// </summary>
    public partial class BoxRadioButton : RadioButton
    {
        public BoxRadioButton()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 选中边框的颜色
        /// </summary>
        public Brush SelectedBrush
        {
            get { return (Brush)GetValue(SelectedBrushProperty); }
            set { SetValue(SelectedBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedBrushProperty =
            DependencyProperty.Register("SelectedBrush", typeof(Brush), typeof(BoxRadioButton), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0, 150, 136))));



    }
}
