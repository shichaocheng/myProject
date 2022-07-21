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
    /// ImageRadioButton.xaml 的交互逻辑
    /// </summary>
    public partial class ImageRadioButton : RadioButton
    {
        public ImageRadioButton()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 图片源路径
        /// </summary>
        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ImageRadioButton), new PropertyMetadata(null));

        /// <summary>
        /// 图片的margin
        /// </summary>
        public Thickness ImageMargin
        {
            get { return (Thickness)GetValue(ImageMarginProperty); }
            set { SetValue(ImageMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageMarginProperty =
            DependencyProperty.Register("ImageMargin", typeof(Thickness), typeof(ImageRadioButton), new PropertyMetadata(new Thickness(10, 10, 10, 10)));

        /// <summary>
        /// 水平 垂直
        /// </summary>
        public Orientation ImageOrientation
        {
            get { return (Orientation)GetValue(ImageOrientationProperty); }
            set { SetValue(ImageOrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageOrientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageOrientationProperty =
            DependencyProperty.Register("ImageOrientation", typeof(Orientation), typeof(ImageRadioButton));

        /// <summary>
        /// 鼠标悬停的边框色
        /// </summary>
        public Brush MouseOverBorderBrush
        {
            get { return (Brush)GetValue(MouseOverBorderBrushProperty); }
            set { SetValue(MouseOverBorderBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MouseOverBorderBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseOverBorderBrushProperty =
            DependencyProperty.Register("MouseOverBorderBrush", typeof(Brush), typeof(ImageRadioButton), new PropertyMetadata(Brushes.Blue));


        /// <summary>
        /// 鼠标按下的边框色
        /// </summary>
        public Brush MousePressBorderBrush
        {
            get { return (Brush)GetValue(MousePressBorderBrushProperty); }
            set { SetValue(MousePressBorderBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MousePressBorderBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MousePressBorderBrushProperty =
            DependencyProperty.Register("MousePressBorderBrush", typeof(Brush), typeof(ImageRadioButton), new PropertyMetadata(Brushes.Blue));


        /// <summary>
        /// 字体图标
        /// </summary>
        public string FIcon
        {
            get { return (string)GetValue(FIconProperty); }
            set { SetValue(FIconProperty, value); }
        }
        // Using a DependencyProperty as the backing store for FIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FIconProperty =
            DependencyProperty.Register("FIcon", typeof(string), typeof(ImageRadioButton), new PropertyMetadata(null));

        /// <summary>
        /// 字体图标的字体大小
        /// </summary>
        public int FIconSize
        {
            get { return (int)GetValue(FIconSizeProperty); }
            set { SetValue(FIconSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FIconSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FIconSizeProperty =
            DependencyProperty.Register("FIconSize", typeof(int), typeof(ImageRadioButton), new PropertyMetadata(20));

        /// <summary>
        /// 字体图标margin
        /// </summary>
        public Thickness FIconMargin
        {
            get { return (Thickness)GetValue(FIconMarginProperty); }
            set { SetValue(FIconMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FIconMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FIconMarginProperty =
            DependencyProperty.Register("FIconMargin", typeof(Thickness), typeof(ImageRadioButton), new PropertyMetadata(new Thickness(10, 10, 10, 10)));
    }
}
