   M S B A M L   `   `   `  ����B  >WpfApp1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:WpfApp1WpfApp1  5   	   5clr-namespace:WpfApp1.componentWpfApp1.component  5   	       WpfApp1.LoginWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   'localclr-namespace:WpfApp1     5   	   2customclr-namespace:WpfApp1.component     5   	     � Title$  LoginWindow��5	   	   $	��768��6   $
��1366��6*    � WindowState$ 	Maximized=�67    � WindowStyle$
 None=�6O   .��5
      (�  (� Stretch$
 Fill=�6   .��5   
   � .+�5      �� $
��Left=�6   $	��768��60   $��0,0,0,0q�6=   $	��Top=�6N   $
��1366��6f   .+�5      �� -   5         ��Name$ image��5      $	��560��6'   $	��900��64   $��./Resources/img/bg.jpg��6@   $
��Fill=�6`   
 �� Top$ 58��6o   �� 5      -   5        $ image1��5      $��23��6(    �� Left$	 910��64   $ 58��6F   $	��200��6V   $$��./Resources/img/LOGO橙色.png��6b   $
��Fill=�6�   �� 5      -   5         ��Name$ gridSplitter��5      $��30��65   $
 1120��6A   $ 58��6T   $��1��6d   ���	#FF8A93936n   �� 5      -   5         ��Name$ 	HeadLabel��5      $��Hydrogen Energy��6+   $
 1130��6E   $ 50��6X   $��48��6h   $	��220��6t   $��25(�6�   � 5      -   5        	 �Name$	 	loginName��5      $	��384��6*   $	 859��67   $	 288��6I   $	��497��6Z   .+�5      �� -   5        $ 
loginLabel��5      $��登录��60   $
 1202��6=   $	 294��6P   $��42(�6a   $��124,0,277,313q�6o   �� 5      -   5        $ prompt��5      $'��!请输入账号密码进行登录��6,   $	 976��6B   $	 362��6T   $��18(�6e   ���	#FF333333$��124,71,160,279q�6�       WpfApp1.component.FTextBox  5      -   5        
  Name$
 username��5   &   $	��200��69      CornerRadius$	 120��6E      	WaterMark$ 请输入用户名��6Y   $��137,146,160,210q�6l   6�     5      -	   5        $
 password��5   &   $	��200��69      IsPasswordBox . $ 请输入密码��6{   $��137,198,160,158q�6�      PasswordStr 6Z   �� *
Password+6�   �� 5      -
   5         ��Name$ login��5      $��登录��6,   $
 1007��69   $	 574��6L   $	��177��6]   $��36��6i   
�������$��17(�6�   $��148,287,171,61q�6�   ��6�   Z� ��5      �� 5       �� EndPoint$ 0.5,1&�62    �� 
StartPoint$ 0.5,0&�6C   .��5   "   �  � Color$ 	#FFFF9D38��6/    � Offset$ 0Q�6A   � 5   "   $ 	#FFFF7605��6/   $ 1Q�6A   5      5       5!      5"      5#      5$      5%      