   M S B A M L   `   `   `  ���� O  Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  B >WpfApp1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:WpfApp1WpfApp1 5clr-namespace:WpfApp1.componentWpfApp1.component \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   
testBorder  �� 
TargetType �� Property �� Value  DefFontSize  DisableOpacity  ColorGrey333 �� Color  ColorGrey555  ColorGreyDDD  ColorGreyBBB  ColorGreyE6E6E6  ColorGreyF5F5F5 	 ColorGreyF9F9F9 
 ColorGreyFBFBFB  
ColorWhite  ColorBlueOver  	ColorBlue  ColorGreenOver  
ColorGreen  ColorGreenDarkOver  ColorGreenDark  ColorOrangeOver  ColorOrange  ColorRedOver  ColorRed  ColorYellowOver  ColorYellow  ColorBlueBlack  ColorBlueDark  
Dateformat  DateHeaderSize  DateHeaderBackground  DateHeaderForeground  DateHeaderOverForeground  DateHeaderBorderBottomColor   DateDaySize ! DateWeekForeground " 
baseHeader # largeHeader �� BasedOn $ normalHeader % smallHeader & iconfont ' FButton_Template �� 
TargetType   WpfApp1.component.FButton ��Name �� RelativeSource �� Mode	 �� Path
 �� CornerRadius �� Orientation ��Name J� RenderTransformOrigin �� Angle �� Padding �� Triggers P� Value P� Property �� 
TargetName ]� 
Conditions �� Property �� Value N� EnterActions �� 
TargetName �� TargetProperty	 W� To g� Duration N� ExitActions ( 
baseButton ) 	navButton * FButton_Transparency + 
infoButton , successButton - 
warnButton . dangerButton / FTextBox_Template   WpfApp1.component.FTextBox �Name J� SnapsToDevicePixels � ColumnDefinitions  �� Padding! ��Name" � Cursor# �� Value$ �� Binding 0 baseFTextBox 1 smallFTextBox 2 largeFTextBox 3 FCheckBox_Template!  WpfApp1.component.FCheckBox% ��Name 4 baseCheckBox 5 blueCheckBox 6 greenCheckBox 7 navCheckBox 8 warnCheckBox 9 dangerCheckBox : BulletCheckBox_Templete&   WpfApp1.component.BulletCheckBox& (� Stretch' T� X(   Text) �� TextAlignment ; baseBulletCheckBox < blueBulletCheckBox = greenBulletCheckBox > warnBulletCheckBox ? dangerBulletCheckBox @ FRadioButton_Template$  WpfApp1.component.FRadioButton A baseFRadioButton B blueRadioButton C greenRadioButton D navRadioButton E warnRadioButton F dangerRadioButton G BoxRadioButton_Templete&   WpfApp1.component.BoxRadioButton H baseBoxRadioButton I dangerBoxRadioButton J blueBoxRadioButton K warnBoxRadioButton L stlToggleButton* � ActualWidth M FCombox_Templete+ �� 	ClickMode, d� 	IsChecked- �� Mode. J� IsHitTestVisible/ �� SelectionBoxItem0 �� SelectionBoxItemTemplate1 � HorizontalOffset2 � VerticalOffset3 �� IsDropDownOpen4 K� IsItemsHost-  'System.Windows.Input.KeyboardNavigation5   DirectionalNavigation N CustomCombox O ImageRadioButton_Template(  "WpfApp1.component.ImageRadioButton6 ��Name7 �� 
StartPoint8 �� EndPoint9 � Color: � Offset P baseImageRadioButton Q hlayoutImageRadioButton R navItemImageRadioButton�� F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     :x,http://schemas.microsoft.com/winfx/2006/xaml     .sys&clr-namespace:System;assembly=mscorlib  localclr-namespace:WpfApp1  *customclr-namespace:WpfApp1.component  %_F  &	        &	 +     &	 7     &	 D     &	 T     &	 d     &	 t     &	 �     &	 �     &		 �     &	
 �     &	 �     &	 �     &	 �     &	     &	     &	 ,    &	 ?    &	 R    &	 e    &	 x    &	 �    &	 �    &	 �    &	 �    &	 �    &	 �    &	 �    &	 
    &	     &	 -    &	 @    &	  P    &	! \    &	" l    &	# �    7 " &	$ &    7 " &	% N    7 " &	& v    &	' �    7 & &	( _    7 ' '   E    7 ( 7  7  7  7  7  7  7  7  7  &	) �    7 ( 7  7  7  7  7  7  &	* J    7 ( 7  7  7  &	+ E    7 ( 7  7  7  7  7  7  &	,     7 ( 7  7  7  7  7  7  &	- �    7 ( 7  7  7  7  7  7  &	. �    7 ( 7  7  7  7  7  7  &	/ m    7 & &	0 Q    7 / 7  7  7  7  7  7 	 '  �    7 0 &	1     7 0 &	2 K    7 0 &	3 �    7 & &	4 �!    7 3 7  7  '  #    7 4 7  &	5 @#    7 4 7  &	6 t#    7 4 7  &	7 �#    7 4 7  &	8 �#    7 4 7  &	9 $    7 4 7  &	: D$    &	; c(    7 : 7  7  7  '  C)    7 ; 7  &	< y)    7 ; 7  &	= �)    7 ; 7  &	> �)    7 ; 7  &	? *    7 ; 7  &	@ Q*    7 & &	A �-    7  7 @ 7  '  ).    7 A 7  &	B ].    7 A 7  &	C �.    7 A 7  &	D �.    7 A 7  &	E �.    7 A 7  &	F -/    7 A 7  &	G a/    7 & &	H 2    7 G 7  7  '  �2    7 H 7  &	I �2    7 H 7  &	J '3    7 H 7  &	K Y3    7 H 7  &	L �3    '�� G5    &	M 6    7 L &	N �7    7 M 7  7  &	O �8    7 & &	P �>    7 O '  �?    7 P 7  &	Q @    7 P 7  &	R v@    7 P 7  �� "  ��.���  �@��Padding$ 20q�X�30Q�X�0.6Q��� $
 #333���� $
 #555���� $
 #DDD���� $
 #BBB���� $ #E6E6E6���� $ #F5F5F5���� $ #F9F9F9���� $ #FBFBFB���� $
 #FFF���� $ #48B2FF���� $ #1E9FFF���� $ #74C474���� $ #5EB95E���� $ #15AA9D���� $ #009688���� $ #F58B3B���� $ #FF5722���� $ #E56764���� $ #DD514C���� $ #F5D171���� $ #F0C044���� $ #0D648D���� $ #3D94BD����
yyyy-MM-dd��X�18Q��� $ #FBFBFB���� $
 #555���� $ #009688���� $
 #ccc��X�15Q��� $
 #000���� "  ��.���  � ��$ 黑体)���  � ��$
 Bold"���  � �� �#fff��  � �� �#333��  �@��Padding$ 20 10q��� "  ��8   .���  � ��$ 24(��� "  ��8   .���  � ��$ 20(��� "  ��8   .���  � ��$ 16(��� "  ��.���  � ��$ Center=���  � ��$ Center=���  � ��$9 3pack://application:,,,/resources/iconfont/#iconfont)��� "   .R��� $ border��$	��0.1q�#��z��#��z������  �� $ TemplatedParent=�$	 
Background�����  �� $ TemplatedParent=�$	 Height�
 ��  �� $ TemplatedParent=�$	 CornerRadius�����  �� $ TemplatedParent=�$	 Width�._��� $ 
Horizontal=�$��Center=�#��z��#��z��.+��� $
 icon��$ 0.5,0.5&�8��  ����  �� $ TemplatedParent=�$	 FIconMargin�����  �� $ TemplatedParent=�$	 FIcon�����  �� $ TemplatedParent=�$	 	FIconSize�����  �� $ TemplatedParent=�$	 
Foreground�z��� 	transIcon��$ 0Q��� $	 txt��$��Center=�$ 0 0 0 0q�����  �� $ TemplatedParent=�$	 ContentDecorations�����  �� $ TemplatedParent=�$	 Content�����  �� $ TemplatedParent=�$	 FontSize�����  �� $ TemplatedParent=�$	 
Foreground� P�  Z�  �@  FIcon. ��� $
 icon�� � y�$ 	Collapsed=�P�  Z�  � ��. ��� $
 icon�� � ��$ 0q�P�  �@J�IsMouseOver . . ��� $ border�� ��  �� $ TemplatedParent=�$	 MouseOverBackground� � ���� $ border�� ��  �� $ TemplatedParent=�$	 MouseOverBorderBrush� � ���� $
 icon�� ��  �� $ TemplatedParent=�$	 MouseOverForeground� � ���� $	 txt�� ��  �� $ TemplatedParent=�$	 MouseOverForeground� � ��P�  �@J�	IsFocused . . ��� $ border�� ��  �� $ TemplatedParent=�$	 MouseOverBackground� � ���� $ border�� ��  �� $ TemplatedParent=�$	 PressBorderBrush� � ���� $
 icon�� ��  �� $ TemplatedParent=�$	 MouseOverForeground� � ���� $	 txt�� ��  �� $ TemplatedParent=�$	 MouseOverForeground� � ��]�  ��  �@J�IsMouseOver . ��  �@  AllowsAnimation .  �� .���� .��W� $ 	transIcon��$ Angle�$	 360X�$ 0:0:0.5D� �� .���� .��W� $ 	transIcon��$ Angle�$ 0X�$ 0:0:0.5D�P�  � �� . . ��� $ border�� ��  �� $ TemplatedParent=�$	 PressedBackground� � ���� $
 icon�� ��  �� $ TemplatedParent=�$	 PressedForeground� � ���� $	 txt�� ��  �� $ TemplatedParent=�$	 PressedForeground� � ��P�  � {� .  . ��� $ border�� �@J�Opacity$	 0.5Q��� "    .���  � ��$ Center=���  � ��$ 13(���  �@  CornerRadius$ 0����  �@  	FIconSize$ 16����  � ��8   ��  � ��$
 15 5q���  � �� Z� ��  �@  FIconMargin$ 0,0,8,0q��� "    8   .���  � ��8  ��  � ��$ 1q���  � ��8  ��  � ��8  ��  �@  MouseOverBackground8  ��  �@  MouseOverForeground8  ��  �@  MouseOverBorderBrush8  ��  �@  PressedBackground8  ��  �@  PressedForeground8  ��  �@  PressBorderBrush8 	 �� "    8   .���  � ��8  ��  � ��8  ��  �@  MouseOverBackground8  ��  �@  MouseOverForeground8  ��  �@  PressedBackground8  ��  �@  PressedForeground8  �� "    8   .���  � ��
 ���� ��  � ��8  ��  �@  MouseOverBackground
 ���� ��  �@  MouseOverForeground8  ��  �@  PressedBackground
 ���� ��  �@  PressedForeground8  ��  �@�Cursor$
 Hand���� "    8   .���  � ��8  ��  � ��8  ��  �@  MouseOverBackground8  ��  �@  MouseOverForeground8  ��  �@  PressedBackground8  ��  �@  PressedForeground8  �� "    8   .���  � ��8  ��  � ��8  ��  �@  MouseOverBackground8  ��  �@  MouseOverForeground8  ��  �@  PressedBackground8  ��  �@  PressedForeground8  �� "    8   .���  � ��8  ��  � ��8  ��  �@  MouseOverBackground8  ��  �@  MouseOverForeground8  ��  �@  PressedBackground8  ��  �@  PressedForeground8  �� "    8   .���  � ��8  ��  � ��8  ��  �@  MouseOverBackground8  ��  �@  MouseOverForeground8  ��  �@  PressedBackground8  ��  �@  PressedForeground8  �� "  .R�� $ 	PART_ROOT��.+��� $ Bg��$��0q�# z #��z��#��z��#��z��#��z��$
 0��._�� $ PART_InnerGrid�� �� $
��Auto ��� $��* ��� $
��Auto �.+��� $ Label��$��0��$��0q�$
  10 0q�$y�Visible=�$��0 0 1 0q�#��z��$
��auto��#��z��$��Center=�$��Center=�$
 0������  �� $ TemplatedParent=�$	 LabelBackground�._��� #��z��$��Center=�����  �� $ TemplatedParent=�$	 LabelForeground�����  �� $ TemplatedParent=�$	 	LabelText��� $! PART_ContentHost��$��1��$��0q���.  $��0q�#" z" $��Center=���Z� �� $ Message��$��1��$y�	Collapsed=�#��z��$ 0q�
������ $
��Left=�$��Center=�#��z������  �� $ TemplatedParent=�$	 WaterMarkBrush�����  �� $ TemplatedParent=�$	 	WaterMark��� $ TextIcon��$��2��$y�Visible=�$
��auto��#��z��$��Center=�$��Center=�#��z��8��  ����  �� $ TemplatedParent=�$	 	TextFIcon�����  �� $ TemplatedParent=�$	 TextFIconSize�����  �� $ TemplatedParent=�$	 TextFIconMargin� �� $#  ��$ ��  �� *Self+$
	 Text�.a��� $ Message�� � y�$ Visible=�P�  Z�  �@ 	TextFIcon. ��� $ TextIcon�� � y�$ 	Collapsed=�P�  Z�  �@ 	LabelText. ��� $ Label�� � y�$ 	Collapsed=�P�  �@J�IsMouseOver . . ��� $ Bg�� ��  �� $ TemplatedParent=�$	 MouseOverBrush� � ���� $ Label�� ��  �� $ TemplatedParent=�$	 MouseOverBrush� � ��P�  �@J�	IsFocused . . ��� $ Bg�� ��  �� $ TemplatedParent=�$	 MousePressBrush� � ���� $ Label�� ��  �� $ TemplatedParent=�$	 MousePressBrush� � ���� "   .���  � ��8   ��  � ��$	 5 5q���  � ��$ 60����  � ��$ 24����  � ��$	 120����  � ��$ 28����  � ��$ 13(���  � ��8  ��  � �� �#FFF��  � ��$ 1q���  �@ MouseOverBrush8  ��  �@ MousePressBrush8  ��  � ��8  ��  �@ LabelForeground8  ��  �@ LabelBackground8  �� "   8   .���  � ��$ 14(���  � ��$ 28���� "   8   .���  � ��$ 12(���  � ��$ 26���� "   8   .���  � ��$ 16(���  � ��$ 32���� "  .R��� $% 
stackpanel��#��z��$ 
Horizontal=�$��Center=�.+��� $
 icon��8��  ����  �� $ TemplatedParent=�$	 FIcon�����  �� $ TemplatedParent=�$	 	FIconSize�����  �� $ TemplatedParent=�$	 FIconMargin�����  �� $ TemplatedParent=�$	 
FIconBrush��� $��Center=�#��z�� P�  �@d�	IsChecked$
 trueY�. ��� $
 icon�� ��  �� $ TemplatedParent=�$	 FIconSelected� � ���� $
 icon�� ��  �� $ TemplatedParent=�$	 PressFIconBrush� � ��P�  Z�  �@d�	IsChecked. ��� $
 icon�� ��  �� $ TemplatedParent=�$	 	FIconNull� � ���� $
 icon�� ��  �� $ TemplatedParent=�$	 PressFIconBrush� � ��P�  �@J�IsMouseOver . . ��� $
 icon�� ��  �� $ TemplatedParent=�$	 PressFIconBrush� � ��P�  � �� . . ��� $
 icon�� ��  �� $ TemplatedParent=�$	 FIconSelected� � ���� $
 icon�� ��  �� $ TemplatedParent=�$	 PressFIconBrush� � ��P�  � {� .  . ��� $ 
stackpanel�� �@J�Opacity$	 0.5Q��� "   .���  � ��8   ��  �@ FIcon$	 ����  �@ 	FIconNull$	 ����  �@ FIconSelected$	 ����  � ��$ 13(���  � ��8  ��  �@ 	FIconSize$ 20����  �@ 
FIconBrush8  ��  �@ FIconMargin$ 0 0 2 0q��� "   8   .���  �@ PressFIconBrush8  �� "   8   .���  �@ PressFIconBrush8  �� "   8   .���  �@ PressFIconBrush8  �� "   8   .���  �@ PressFIconBrush8  �� "   8   .���  �@ PressFIconBrush8  �� "   8   .���  �@ PressFIconBrush8  �� "  .R�(� $& Uniform=�$��Center=�$��Center=�.���� $ border��$��75��$��28��#��z�� . #��z��$
 14��._��� $ 
Horizontal=�.+��� $ state��$��24��$��24��$��3,2,1,2q�$
 12�� . #��z��z�T� 
transState��$' 0Q��� $	 txt��$��40��#��z��#��z( $��Center=�$) Center=�z�T� transTxt��$' 0Q� P�  �@d�	IsChecked$
 TrueY�. ��� $	 txt�� ��  �� $ TemplatedParent=�$	 CheckedText� � ���� $ state�� ��  �� $ TemplatedParent=�$	 CheckedForeground� � ���� $	 txt�� ��  �� $ TemplatedParent=�$	 CheckedForeground� � ���� $ border�� ��  �� $ TemplatedParent=�$	 CheckedBackground� � �� �� .���� .��W� $ 
transState��$ X�$ 45X�$ 0:0:0.2D�W� $ transTxt��$ X�$	 -24X�$ 0:0:0.2D� �� .���� .��W� $ 
transState��$ X�$ 0X�$ 0:0:0.2D�W� $ transTxt��$ X�$ 0X�$ 0:0:0.2D�P�  � {� .  . ��� $ border�� �@J�Opacity$	 0.5Q��� "   .���  � ��8   ��  � ��$
 Bold"���  � ��$ 20(���  �@�Cursor$
 Hand����  � ��$	 100����  � ��$ 0q���  � ��8  ��  � ��8  ��  �@ CheckedForeground8  �� 8   "   .���  �@ CheckedBackground8  �� 8   "   .���  �@ CheckedBackground8  �� 8   "   .���  �@ CheckedBackground8  �� 8   "   .���  �@ CheckedBackground8  �� 8   "   .���  �@ CheckedBackground8  �� "  .R��� $% 
stackpanel��#��z��$ 
Horizontal=�$��Center=�.+��� $
 icon�� .  8��  $	����#��z������  �� $ TemplatedParent=�$	 	FIconSize�����  �� $ TemplatedParent=�$	 FIconMargin��� $��Center=�#��z�� P�  �@d�	IsChecked$
 trueY�. ��� $
 icon�� � ��$	 ���� $
 icon�� ��  �� $ TemplatedParent=�$	 PressFIconBrush� � ��P�  Z�  �@d�	IsChecked. ��� $
 icon�� � ��$	 ��P�  �@J�IsMouseOver . . ��� $
 icon�� ��  �� $ TemplatedParent=�$	 PressFIconBrush� � ��P�  � �� . . ��� $
 icon�� � ��$	 ���� $
 icon�� ��  �� $ TemplatedParent=�$	 PressFIconBrush� � ��P�  � {� .  . ��� $ 
stackpanel�� �@J�Opacity$	 0.5Q��� "   .���  � ��8   ��  � ��8  ��  � ��$ 13(���  � ��8  ��  �@ 	FIconSize$ 16����  �@ FIconMargin$ 0 0 2 0q��� "   8   .���  �@ PressFIconBrush8  �� "   8   .���  �@ PressFIconBrush8  �� "   8   .���  �@ PressFIconBrush8  �� "   8   .���  �@ PressFIconBrush8  �� "   8   .���  �@ PressFIconBrush8  �� "   8   .���  �@ PressFIconBrush8  �� "  .R�� $
 grid��$��Center=�.+��� $ Border�� . #��z��#��z��#��z��#��z��#��z��$��Center=�._��� $��Center=�$��Center=�#��z��# z �� $ 
checkState��$	����8��  $��0 0 0 0q�$��14(�$��Right=�$��Bottom=�$y�	Collapsed=�����  �� $ TemplatedParent=�$	 SelectedBrush� P�  �@d�	IsChecked$
 trueY�. ��� $ 
checkState�� � y�$ Visible=��� $ Border�� ��  �� $ TemplatedParent=�$	 SelectedBrush� � ��P�  �@J�IsMouseOver . . ��� $ Border�� ��  �� $ TemplatedParent=�$	 SelectedBrush� � ��P�  � {� .  . ��� $
 grid�� �@J�Opacity$	 0.5Q��� "   .���  � ��$
 10 2q���  � ��$
 10 0q���  � ��8   ��  � ��
 ���� ��  � ��$ 14(���  � ��8  ��  � ��8  ��  � ��$ 1q��� "   8   .���  �@ SelectedBrush8  �� "   8   .���  �@ SelectedBrush8  �� "   8   .���  �@ SelectedBrush8  �� "   8   .���  �@ SelectedBrush8  �� "  d�.���  � ��
 �������  � �� �� " d�.R��  �� $��* ��� $
��Auto �.+��� $ border��$��0��$��25��#��z* 
������ ���#AAA$��1 1 0 1q��� $��1��$��22��$��Right=�#��z��#��z��#��z��._�F� "����        ��g5  	= ���j     	
�������$��Center=�$��Center=� P�  �@J�IsMouseOver . . ��� $ border�� ��  �� $ TemplatedParent=�$	 
Background� � ���� "  ��.���  � �� �� " ��.R��� $ 
ItemBorder��$
  10 5q�$��1q�$��Stretch=�$��0q�._��� $��Stretch=� P�  �@J�IsMouseOver . . ��� $ 
ItemBorder�� � �� �#EEEEEE�� " ��.R�� .+�d� #��z��#��z��#��z��8��  |�.  $+ Press=�, �� $	 IsDropDownOpen�$- TwoWay=� �� *TemplatedParent+�� $
��10 0q�. .  #��z/ #��z0 #��z��$��Center=�$��Stretch=�� $1 0��$2 1��#��z* #��z3 |�.  $��Slide=�.���  . $��Stretch=�.+��� $��1q����#888$��Stretch=����#FFF��  . $��Stretch=�$��0q�$	��0 5q�.���� 4 . $5 	Contained=�$��Stretch=��� "  ��.���  � ��8   ��  �@J�SnapsToDevicePixels . ��  � ��$
 Auto=���  � ��$
 Auto=���  � �� . ��  � ��$ Stretch=���  � ��8  ��  � ��8  ��  � ��$ 0q���  � ��$ 15(���  � ��$ 0q��� "  .R��� $ border��$��0.1 0q�$  0q�#��z��#��z������  �� $ TemplatedParent=�$	 
Background�����  �� $ TemplatedParent=�$	 Height�����  �� $ TemplatedParent=�$	 Width�._��� $��Center=�#��z��#��z�� ��  �� $ TemplatedParent=�$	 ImageOrientation�.+��� $6 Image��$��Center=�$��Center=�$��35��$y�Visible=�$��0q�����  �� *TemplatedParent+$	 ImageSource��� $
 icon��$y�Visible=�$ 0.5,0.5&�8��  ����  �� $ TemplatedParent=�$	 FIconMargin�����  �� $ TemplatedParent=�$	 FIcon�����  �� $ TemplatedParent=�$	 	FIconSize�����  �� $ TemplatedParent=�$	 
Foreground�z��� 	transIcon��$ 0Q��� $	 txt��$��Center=�$��Center=�����  �� $ TemplatedParent=�$	 Content�����  �� $ TemplatedParent=�$	 FontSize�����  �� $ TemplatedParent=�$	 
FontFamily�����  �� $ TemplatedParent=�$	 
Foreground�����  �� $ TemplatedParent=�$	 ImageMargin� P�  �@d�	IsChecked$
 trueY�. ��� $ border�� � �� �� $7 0.5 0&�$8 0.5 1&�.��� $9 	#22FFFFFF��$: 1Q�P�  �@J�IsMouseOver . . ��� $ border�� � �� �� $7 0.5 0&�$8 0.5 1&�.��� $9 	#22FFFFFF��$: 1Q�P�  Z�  �@ FIcon. ��� $
 icon�� � y�$ 	Collapsed=�P�  Z�  �@ ImageSource. ��� $ Image�� � y�$ 	Collapsed=��� "   .���  � ��8   ��  � ��$	 0 8q���  �@ ImageMargin$ 5q���  � ��$ 0q���  � ��$ 15(���  � ��$ Center=���  �@ FIconMargin$ 5q���  �@ 	FIconSize$ 25����  � ��
 ���� �� "   8   .���  � ��$	 100����  �@ ImageOrientation$ Vertical=���  � ��8  �� "   8   .���  � ��$	 120����  �@ ImageOrientation$ 
Horizontal=���  � ��8  �� "   8   .���  � ��8  ��  � ��$ Stretch=�