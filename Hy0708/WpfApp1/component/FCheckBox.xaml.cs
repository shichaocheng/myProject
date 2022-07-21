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
{

    /// <summary>
    /// FCheckBox.xaml 的交互逻辑
    /// </summary>
    public partial class FCheckBox : CheckBox
    {
        public FCheckBox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 字体图标颜色
        /// </summary>
        public Brush FIconBrush
        {
            get { return (Brush)GetValue(FIconBrushProperty); }
            set { SetValue(FIconBrushProperty, value); }
        }
        // Using a DependencyProperty as the backing store for FIconBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FIconBrushProperty =
            DependencyProperty.Register("FIconBrush", typeof(Brush), typeof(FCheckBox), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(100, 100, 100))));

        public Brush PressFIconBrush
        {
            get { return (Brush)GetValue(PressFIconBrushProperty); }
            set { SetValue(PressFIconBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PressFIconBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PressFIconBrushProperty =
            DependencyProperty.Register("PressFIconBrush", typeof(Brush), typeof(FCheckBox), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0, 150, 136))));




        public string FIcon
        {
            get { return (string)GetValue(FIconProperty); }
            set { SetValue(FIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FIconProperty =
            DependencyProperty.Register("FIcon", typeof(string), typeof(FCheckBox), new PropertyMetadata("&#xe8c7;"));



        public string FIconNull
        {
            get { return (string)GetValue(FIconNullProperty); }
            set { SetValue(FIconNullProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FIconNull.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FIconNullProperty =
            DependencyProperty.Register("FIconNull", typeof(string), typeof(FCheckBox), new PropertyMetadata("&#xe61b;"));




        public string FIconSelected
        {
            get { return (string)GetValue(FIconSelectedProperty); }
            set { SetValue(FIconSelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FIconSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FIconSelectedProperty =
            DependencyProperty.Register("FIconSelected", typeof(string), typeof(FCheckBox), new PropertyMetadata("&#xe66a;"));



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
            DependencyProperty.Register("FIconSize", typeof(int), typeof(FCheckBox), new PropertyMetadata(16));

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
            DependencyProperty.Register("FIconMargin", typeof(Thickness), typeof(FCheckBox), new PropertyMetadata(new Thickness(0, 0, 2, 0)));



    }
}
