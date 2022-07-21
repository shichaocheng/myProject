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
  /// FRadioButton.xaml 的交互逻辑
  /// </summary>
    public partial class FRadioButton : RadioButton
    {
        public FRadioButton()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 选中颜色
        /// </summary>
        public Brush PressFIconBrush
        {
            get { return (Brush)GetValue(PressFIconBrushProperty); }
            set { SetValue(PressFIconBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PressFIconBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PressFIconBrushProperty =
            DependencyProperty.Register("PressFIconBrush", typeof(Brush), typeof(FRadioButton), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0, 150, 136))));


        /// <summary>
        /// 字体图标大小
        /// </summary>
        public int FIconSize
        {
            get { return (int)GetValue(FIconSizeProperty); }
            set { SetValue(FIconSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FIconSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FIconSizeProperty =
            DependencyProperty.Register("FIconSize", typeof(int), typeof(FRadioButton), new PropertyMetadata(16));

        /// <summary>
        /// 字体图标的Margin
        /// </summary>
        public Thickness FIconMargin
        {
            get { return (Thickness)GetValue(FIconMarginProperty); }
            set { SetValue(FIconMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FIconMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FIconMarginProperty =
            DependencyProperty.Register("FIconMargin", typeof(Thickness), typeof(FRadioButton), new PropertyMetadata(new Thickness(0, 0, 2, 0)));
    }
}
