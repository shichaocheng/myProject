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
    /// FTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class FTextBox : TextBox
    {
        public FTextBox()
        {
            InitializeComponent();
        }


        /// <summary>
        /// 圆角
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        // Using a DependencyProperty as the backing store for CornerRadius.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(FTextBox), new PropertyMetadata(null));

        /// <summary>
        /// 是否为密码框
        /// </summary>
        public bool IsPasswordBox
        {
            get { return (bool)GetValue(IsPasswordBoxProperty); }
            set { SetValue(IsPasswordBoxProperty, value); }
        }

        public static DependencyProperty IsPasswordBoxProperty =
      DependencyProperty.Register("IsPasswordBox", typeof(bool), typeof(FTextBox), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsPasswordBoxChnage)));

        //4.当设置为密码框时，监听TextChange事件，处理Text的变化，这是密码框的核心功能
        private static void OnIsPasswordBoxChnage(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as FTextBox).SetEvent();
        }

        /// <summary>
        /// 定义TextChange事件
        /// </summary>
        private void SetEvent()
        {
            if (IsPasswordBox)
                this.TextChanged += TextBox_TextChanged;
            else
                this.TextChanged -= TextBox_TextChanged;
        }
        //5.在TextChange事件中，处理Text为密码文，并将原字符记录给PasswordStr予以存储
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsResponseChange) //响应事件标识，替换字符时，不处理后续逻辑
                return;
            //Console.WriteLine(string.Format("------{0}------", e.Changes.Count));
            foreach (TextChange c in e.Changes)
            {
                //Console.WriteLine(string.Format("addLength:{0} removeLenth:{1} offSet:{2}", c.AddedLength, c.RemovedLength, c.Offset));
                PasswordStr = PasswordStr.Remove(c.Offset, c.RemovedLength); //从密码文中根据本次Change对象的索引和长度删除对应个数的字符
                PasswordStr = PasswordStr.Insert(c.Offset, Text.Substring(c.Offset, c.AddedLength));   //将Text新增的部分记录给密码文
                lastOffset = c.Offset;
            }
            //Console.WriteLine(PasswordStr);
            /*将文本转换为密码字符*/
            IsResponseChange = false;  //设置响应标识为不响应
            this.Text = ConvertToPasswordChar(Text.Length);  //将输入的字符替换为密码字符
            IsResponseChange = true;   //回复响应标识
            this.SelectionStart = lastOffset + 1; //设置光标索引
            //Console.WriteLine(string.Format("SelectionStar:{0}", this.SelectionStart));
        }



        /// <summary>
        /// 按照指定的长度生成密码字符
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private string ConvertToPasswordChar(int length)
        {
            if (PasswordBuilder != null)
                PasswordBuilder.Clear();
            else
                PasswordBuilder = new StringBuilder();
            for (var i = 0; i < length; i++)
                PasswordBuilder.Append(PasswordChar);
            return PasswordBuilder.ToString();
        }

        //6.如果用户设置了记住密码，密码文(PasswordStr)一开始就有值的话，别忘了在Load事件里事先替换一次明文
        private void FTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsPasswordBox)
            {
                IsResponseChange = false;
                this.Text = ConvertToPasswordChar(PasswordStr.Length);
                IsResponseChange = true;
            }
        }

        public static DependencyProperty PasswordStrProperty =
    DependencyProperty.Register("PasswordStr", typeof(string), typeof(FTextBox), new FrameworkPropertyMetadata(string.Empty));
        /// <summary>
        /// 密码字符串
        /// </summary>
        public string PasswordStr
        {
            get { return GetValue(PasswordStrProperty).ToString(); }
            set { SetValue(PasswordStrProperty, value); }
        }


        public static DependencyProperty PasswordCharProperty =
    DependencyProperty.Register("PasswordChar", typeof(char), typeof(FTextBox), new FrameworkPropertyMetadata('●'));

        /// <summary>
        /// 替换明文的单个密码字符
        /// </summary>
        public char PasswordChar
        {
            get { return (char)GetValue(PasswordCharProperty); }
            set { SetValue(PasswordCharProperty, value); }
        }

        private bool IsResponseChange = true;
        private StringBuilder PasswordBuilder;
        private int lastOffset = 0;

        /// <summary>
        /// 水印
        /// </summary>
        public string WaterMark
        {
            get { return (string)GetValue(WaterMarkProperty); }
            set { SetValue(WaterMarkProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WaterMark.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WaterMarkProperty =
            DependencyProperty.Register("WaterMark", typeof(string), typeof(FTextBox), new PropertyMetadata(null));

        /// <summary>
        /// 水印颜色
        /// </summary>
        public Brush WaterMarkBrush
        {
            get { return (Brush)GetValue(WaterMarkBrushProperty); }
            set { SetValue(WaterMarkBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WaterMarkBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WaterMarkBrushProperty =
            DependencyProperty.Register("WaterMarkBrush", typeof(Brush), typeof(FTextBox), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(170, 170, 170))));

        /// <summary>
        /// 标签背景色
        /// </summary>

        public Brush LabelBackground
        {
            get { return (Brush)GetValue(LabelBackgroundProperty); }
            set { SetValue(LabelBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelBackgroundProperty =
            DependencyProperty.Register("LabelBackground", typeof(Brush), typeof(FTextBox), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(240, 240, 240))));

        /// <summary>
        /// 标签文本
        /// </summary>

        public string LabelText
        {
            get { return (string)GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register("LabelText", typeof(string), typeof(FTextBox), new PropertyMetadata(null));

        /// <summary>
        /// label背景
        /// </summary>
        public Brush LabelForeground
        {
            get { return (Brush)GetValue(LabelForegroundProperty); }
            set { SetValue(LabelForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelForeground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelForegroundProperty =
            DependencyProperty.Register("LabelForeground", typeof(Brush), typeof(FTextBox), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(100, 100, 100))));

        /// <summary>
        /// 字体图标
        /// </summary>
        public string TextFIcon
        {
            get { return (string)GetValue(TextFIconProperty); }
            set { SetValue(TextFIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextFIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextFIconProperty =
            DependencyProperty.Register("TextFIcon", typeof(string), typeof(FTextBox), new PropertyMetadata(null));



        /// <summary>
        /// 字体图标大小
        /// </summary>
        public int TextFIconSize
        {
            get { return (int)GetValue(TextFIconSizeProperty); }
            set { SetValue(TextFIconSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextFIconSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextFIconSizeProperty =
            DependencyProperty.Register("TextFIconSize", typeof(int), typeof(FTextBox), new PropertyMetadata(16));


        /// <summary>
        /// 字体图标外边距
        /// </summary>
        public Thickness TextFIconMargin
        {
            get { return (Thickness)GetValue(TextFIconMarginProperty); }
            set { SetValue(TextFIconMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextFIconMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextFIconMarginProperty =
            DependencyProperty.Register("TextFIconMargin", typeof(Thickness), typeof(FTextBox), new PropertyMetadata(new Thickness(0, 0, 0, 0)));

        /// <summary>
        /// 鼠标悬停边框颜色
        /// </summary>
        public Brush MouseOverBrush
        {
            get { return (Brush)GetValue(MouseOverBrushProperty); }
            set { SetValue(MouseOverBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MouseOverBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MouseOverBrushProperty =
            DependencyProperty.Register("MouseOverBrush", typeof(Brush), typeof(FTextBox), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(150, 150, 150))));


        /// <summary>
        /// 鼠标按下边框颜色
        /// </summary>
        public Brush MousePressBrush
        {
            get { return (Brush)GetValue(MousePressBrushProperty); }
            set { SetValue(MousePressBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MousePressBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MousePressBrushProperty =
            DependencyProperty.Register("MousePressBrush", typeof(Brush), typeof(FTextBox), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(50, 160, 130))));



    }
}
