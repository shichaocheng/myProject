   M S B A M L   `   `   `  ���� B  >WpfApp1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:WpfApp1WpfApp1  5clr-namespace:WpfApp1.componentWpfApp1.component      WpfApp1.LoginWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     'localclr-namespace:WpfApp1     2customclr-namespace:WpfApp1.component       � Title$  LoginWindow��$	��768��$
��1366�� � WindowState$ 	Maximized=� � WindowStyle$
 None=�.��(�  (� Stretch$
 Fill=�.��� .+��� $
��Left=�$	��768��$��0,0,0,0q�$	��Top=�$
��1366��.+��� -    ��Name$ image��$	��560��$	��900��$��./Resources/img/bg.jpg��$
��Fill=�
 �� Top$ 58���� -   $ image1��$��23�� �� Left$	 910��$ 58��$	��200��$$��./Resources/img/LOGO橙色.png��$
��Fill=��� -    ��Name$ gridSplitter��$��30��$
 1120��$ 58��$��1�����	#FF8A9393�� -    ��Name$ 	HeadLabel��$��Hydrogen Energy��$
 1130��$ 50��$��48��$	��220��$��25(�� -   	 �Name$	 	loginName��$	��384��$	 859��$	 288��$	��497��.+��� -   $ 
loginLabel��$��登录��$
 1202��$	 294��$��42(�$��124,0,277,313q��� -   $ prompt��$'��!请输入账号密码进行登录��$	 976��$	 362��$��18(����	#FF333333$��124,71,160,279q�    WpfApp1.component.FTextBox  -   
  Name$
 username��$	��200��   CornerRadius$	 120��   	WaterMark$ 请输入用户名��$��137,146,160,210q�  -	   $
 password��$	��200��   IsPasswordBox . $ 请输入密码��$��137,198,160,158q�   PasswordStr �� *
Password+�� -
    ��Name$ login��$��登录��$
 1007��$	 574��$	��177��$��36��
�������$��17(�$��148,287,171,61q���Z� ����  �� EndPoint$ 0.5,1&� �� 
StartPoint$ 0.5,0&�.���  � Color$ 	#FFFF9D38�� � Offset$ 0Q�� $ 	#FFFF7605��$ 1Q�