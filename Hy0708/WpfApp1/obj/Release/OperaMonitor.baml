   M S B A M L   `   `   `  ���� B  >WpfApp1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:WpfApp1WpfApp1  -clr-namespace:WpfApp1.utilsWpfApp1.utils  5clr-namespace:WpfApp1.componentWpfApp1.component  ^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35tOclr-namespace:System.Windows.Forms.Integration;assembly=WindowsFormsIntegration System.Windows.Forms.Integration Q MZedGraph, Version=5.1.7.430, Culture=neutral, PublicKeyToken=02a83cbd123fcd605(clr-namespace:ZedGraph;assembly=ZedGraphZedGraph     WpfApp1.OperaMonitor   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     'localclr-namespace:WpfApp1     ,utilclr-namespace:WpfApp1.utils     2customclr-namespace:WpfApp1.component     _wfiOclr-namespace:System.Windows.Forms.Integration;assembly=WindowsFormsIntegration     =zedgraph(clr-namespace:ZedGraph;assembly=ZedGraph       � Title$#  制氢电源调试软件V2.37�� � Icon$" Resources/img/background.ico��$	��768��$
��1366�� � WindowState$ 	Maximized=� J� 	AllowDrop .  � WindowStartupLocation$ CenterScreen=� � WindowStyle$ ThreeDBorderWindow=� � 	Resources  
  dgCell �� 
TargetType �� Property	 �� Value�� %?   &	        �� " ��.���  �@��TextAlignment$	 Center=�.��(� 
 (� Stretch$

 Fill=�.���  � ColumnDefinitions �� $��0* �.+��� $
��1366��$	��768��$��11��$��	1,0,2.6,0q�.+��� $
��Left=�$	��130��$	��Top=�$	��167��$ ��./Resources/img/leftUp.jpg��$
��Fill=�$��0,0,0,0q� J� OpacityMask �	#FF040D36
 �� Top$ -1���� $
��Left=�$��16��$	��Top=�$	��106��$%��./Resources/img/LOGO白色1.png��$
��Fill=� J� RenderTransformOrigin$ 1.075,0.562&� �� Left$ 30��$ 20��� $
��Left=�$	��648��$��	0,120,0,0q�$��3��$	��Top=�$	��167�����	#FF040D36.+�(   "WpfApp1.component.ImageRadioButton  -     Name$ 	EquipMoni��(   "WpfApp1.component.ImageRadioButton   Click$ equipMoni_Click��  hlayoutImageRadioButton#��[    ImageSource$$ Resources/img/设备信息.png��$��37,80,39,526q�$
��Auto��$��13(�$��设备监控��  -   $ WaveShow��$ waveShow_Click��#��[ $ Resources/img/示波.png��$��37,134,39,472q�$
��Auto��$��13(�$��故障录波��  -   $ Upgrade��$ upgradeShow_Click��#��[ $ Resources/img/升级.png��$��37,188,39,418q�$
��Auto��$��13(�$��在线升级��  -   $ History��$ historyRecord_Click��#��[ $$ Resources/img/历史记录.png��$��37,242,39,364q�$
��Auto��$��13(�$��历史记录���� $
��Left=�$��68��$	��Top=�$��73��$$��./Resources/img/管理员1.png��$ 10.296,1.275&�$
��Fill=�$ 47��$ 86���� -    ��Name$ SysTime��$��123456789101112131415��$��23��$
 1089��$ 37��$	��158���� -   $ ip��$��IP地址：��$��23��$	 172��$ 37���� -    ��Name$ 	ipAddress��$��Center=�$��Center=�$��23��$	 220�� �� TextWrapping$
 Wrap=�$��192.168.11.212��$ 37��$	��120��$��13(�$��Center=�$��Center=��� -   $
 port��$��端口号：��$��23��$	 345��$ 37���� -	   $ portContent��$��Center=�$��Center=�$��23��$	 393��$	��502��$��13(�$ 37��$��57���� $��SlaveId:��$	 458��$ 37��$��23���� -
   $ slaveId2��$��Center=�$��Center=�$��23��$	 511��$
 Wrap=�$��1��$ 37��$��50���� -    ��Name$ connect��$��连接��$	 585��$ 37��$��63��$��23��
��� ���
�������
��� ���$ 0.257,0.289&��� -   $ 	importXML��$��导入配置文件��$��83��$��23��
��� ���
�������
��� ���$
��Left=�$	��Top=�$	 782��$ 37��$ -0.443,0.77&�A� -    A�Name$ status��
���  ��$��25��$	 670��
������ $ 37��$��25���� -   $��connectLabel��$��通信连接状态��$	 641��$ 17��$��29��
��� ���$y�Hidden=�A� -   $ 
workStatus��
��� ���$��50��$��50��$	 931��$ 23��
������ �� -   $��	workLabel��$��整机工作状态��$	 917��$ 2��$��29��
��� ���$y�Hidden=�� -    �Name$ monitor��$y�Visible=�$	��703��$	 167��$ 65��$
��1189��.+��� -    ��Name$ listBox��$	 167��$ 96��
�������$��0,41,999,10q��� -    ��Name$ 
tabControl��$	 367��$ 96��$��190,41,44,10q�.@��� $��实时信息��.���� $��0��$	��630��$��Bottom=�$��3��$	 449��$	 136��$��
0,-2,-1,-3q�.+�&   System.Windows.Controls.DataGrid  -     Name$ realGrid��   HeadersVisibility$ Column=�   GridLinesVisibility$ 
Horizontal=�$	��636��$��2q�   AutoGenerateColumns .     HorizontalGridLinesBrush
 �����    CanUserAddRows  .  !   SelectionMode$! Single=�$��-1,0,0,0q�"   
IsReadOnly" . #   Columns# 4  .System.Windows.Controls.DataGridTemplateColumn  ,  &System.Windows.Controls.DataGridColumn$   Width5  /System.Windows.Controls.DataGridLengthConverter$
$ 0.2* %   Header$% 序号��&   ClipboardContentBinding& Z� '   CellTemplate' �� .R��� $��Center=�$��Center=����� ( �� RelativeSource( �� ) �� Mode$) FindAncestor=�* �� AncestorType)  #System.Windows.Controls.DataGridRow"*  + �� Path$+ Header�, �� UpdateSourceTrigger$, PropertyChanged=�0  *System.Windows.Controls.DataGridTextColumn  1	  +System.Windows.Controls.DataGridBoundColumn- 	  ElementStyle#- [  $% 参数名称��$	$ 300 .   
IsReadOnly. . /   CanUserSort/ .  0 	  Binding0 �� *Name+  #- [  $% 	参数值��$$  * . . / .  0 �� *Value+  #- [  $% 单位��$$ * . . / .  0 �� *Unit+1   ColumnHeaderStyle1 �� =
  7System.Windows.Controls.Primitives.DataGridColumnHeader" 
 .���  � ��$	 Center=��� $��参数设置��.��� ���	#FFE5E5E5$��
0,0,-7,-15q�.+��� -   $ setting��$��设置��
��� ���$��812,0,83,615q��� -   $ callBack��$��回读��
��� ���$��885,0,10,615q��� $��0��$	��600��$��Bottom=�$	 449��$	 136��$��0,0,0,10q�.+�  -   $ setGrid��$ 
Horizontal=�$	��599��$��2q� .  
 �����2   EnableRowVirtualization2 . !3   EnableColumnVirtualization3 .   .  $! Single=�#   ' �� .R�� 4 � RowDefinitions4 ��  �� $��40 �.+��� 5 ��Name$5 
Set_Select��-   $��0��$��0��$��Center=�6 d� 	IsChecked6 �� *
Selected+  #- [  $% 参数名称��$	$ 300 . . / .  0 �� *Name+  $	$ 300 $% 	参数值��& Z� ' �� .R��� $	��200��7 �� TextAlignment$7 Center=�$��Center=�$��Center=����� *Value+$, 	LostFocus=�  #- [  $% 单位��$$ * . . / .  0 �� *Unit+  #- [  $% 数据范围��$$ * . . / .  0 �� *Scope+1 �� " 
 .���  � ��$	 Center=�� -   $
 wave��$y�	Collapsed=�$	��703��$	 167��$ 65��$
��1189��.+��� -   $ query��$��查询录波��$
��Left=�$��	10,20,0,0q�$	��Top=�$��75��$ -1.274,0.579&�$��29��
��� ���
���   �
��� ���$��2���� -   $ waveON��$��开启录波��$
��Left=�$��	90,20,0,0q�$	��Top=�$��75��$��29��
��� ���
���   �
��� ����� -   $ getfile��$��文件获取��$y�Hidden=�$
��Left=�$��141,664,0,0q�$	��Top=�$��60��$��29��
��� ���
���   �
��� ���{�. �� -   $ download��$��文件下载��$
��Left=�$��215,664,0,0q�$	��Top=�$��60��$��29��
��� ���
���   �
��� ���{�. �� -   $ delete��$��删除文件��$
��Left=�$��288,664,0,0q�$	��Top=�$��60��$��29��
��� ���
���   �
��� ���{�. �� -   $
 back��$��返回上级目录��$
��Left=�$��
10,664,0,0q�$	��Top=�$��80��$��29��$ 0.124,0.676&�
��� ���
���   �
��� ���{�.  �� $
��Left=�$	��610��$��	10,54,0,0q�$	��Top=�$	��338��.+��� -    $ fileList��$	��611��   WpfApp1.utils.WaveLoading  -!   8  Name$8 waveHolding��$y�	Collapsed=�$��	0,0,841,0q�� $��348,54,0,10q�.+��� -"   9 ��Name$9 devList��$
��Left=�$	��Top=�$	��132��$��26��$��13(�$y�	Collapsed=�7  1System.Windows.Forms.Integration.WindowsFormsHost  $��
0,26,0,212q�:   Child.:   ZedGraph.ZedGraphControl  -#   Fault_waveform��;   IsShowPointValues; .   -$   $ dataGridCorrugated��$��Center=�$	��212��$	��Top=�$	��841��<   ColumnHeaderHeight$< 30Q�  .  $ Column=�=   CanUserReorderColumns= .  .  $��	0,427,0,0q�> � ContextMenu> �� .@��� ? �� Orientation$? Vertical=�.+�m� -%   $��LoadForm��#   ' �� .R�� .+��� $5 CorrugatedCheckBox_Select��-&   $��20��$��0��$��0��$��Center=�$��Center=�6 �� *
Selected+  #- [  $$ * $% 波形代码��. .  0 �� *Name+@ 	  ClipboardContentBinding@ Z�   #- [  $$ * $% 	总点数��. .  0 �� *	AllPoints+@ Z�   #- [  $$ * $% 故障点数��. .  0 �� *
FaultPoint+  #- [  $$ * $% 	后时长��. .  0 �� *	AfterTime+  #- [  $$ * $% 	后点数��. .  0 �� *BehindPoints+1 �� " 
 .���  � ��$	 Center=�� -'   $ upgrade��$y�	Collapsed=�$	��703��$	 167��$ 65��$
��1189�� �� $	��99* ��� $	��20* ��� $	��30* ��� $
��139* ��� $
��804* ��� $	��85* ��� $	��12* �.+��� -(   $ 
sourceName��$��升级源文件：��$
��Left=�$��	19,25,0,0q�$	��Top=�$ 0.185,-0.965&�$��2���� -)   $ 
sourceFile��$
��Left=�$��25��$��2,25,0,0q�$
 Wrap=�$	��Top=�$	��335��$��4��$��1���� -*   $ browse��$��	浏览...��$
��Left=�$��153.337,25,0,0q�$	��Top=�$��74��$��25��
��� ���
�������
��� ���$��4���� -+   $ upload��$��文件上传��$
��Left=�$��232.337,25,0,0q�$	��Top=�$��82��$��25�����	#FF0DABF5���	#FFF5EEEE���	#FF0DABF5$��4���� -,   $ turnUp��$��固件上传��$
��Left=�$��724.337,47,0,0q�$	��Top=�$��75��
��� ���
�������
��� ���$��4���� --   $ upgradeButton��$��升级��$
��Left=�$��0,47,0,0q�$	��Top=�$��75��
��� ���
�������
��� ���$��5��{�.    -.   $ upgradeGrid��$
��Left=�$	��603��$��0,73,0,0q�$	��Top=�$
��1189��$< 40Q�A   	RowHeight$A 40Q�  .  $ Column=�= .  .  $��7��#   % �  �� $��40 �.+��� -/   $5 upgrade_AllSelect��$��0��$��0��$��Center=�' �� .R��  �� $��40 �.+��� $5 upgrade_Select��$��0��$��0��$��Center=�6 �� *
Selected+  $% 名称��$$ * #- [  0 �� *Name+  $% 固件编码��$$ * #- [  0 �� *FirmwareCode+  $% 固件版本号��$$ * #- [  0 �� *FirmwareVersion+  -0   	Progresss��$% 升级进度��$
$ 1.5* ' �� .R��� .+�� B �Name$B pbBar��
������ 
������ $	��180��$��20��C � ValueC �� *
Progress+�� $��20������ *ProgressValue+  $% 升级结果��$
$ 0.5* #- [  0 �� *Result+1 �� " 
 .���  � ��$	 Center=�   WpfApp1.utils.MyLoading  -1   D  Name$D _loading��$y�	Collapsed=�$��7��� -2   $ history��$y�	Collapsed=�$	��703��$	 167��$ 65��$
��1189��.+�� $
��Left=�$��37��$	��Top=�$	��946��$��
243,64,0,0q�.+��� -3   $9 sql_comboBox��$
��Left=�$��0,16,0,0q�$	��Top=�$y�	Collapsed=�  -4   $ historyRecord��  .  $
��Left=�$	��602��$��243,101,0,0q�$	��Top=�$	��946�� . E   ColumnWidth$E * � $
��Left=�$	��622��$��0,81,0,0q�$	��Top=�$	��243��
�������.+��� -5   $ sqlGet��$��文件获取��$
��Left=�$	��Top=�$��75��$��29��
��� ���
���   �
��� ����� -6   $ getList��F �� SelectionMode$F Extended=�$��
0,29,0,322q��� -7   $ sqlDown��$��文件下载��{�.  $
��Left=�$	��Top=�$��75��$��29��$ 0.898,0.471&�
��� ���
���   �
��� ���$��	0,305,0,0q��� -8   $ 
browseFile��$��浏览��$
��Left=�$	��Top=�$��75��$��29��$ 0.898,0.471&�
��� ���
���   �
��� ���$��
75,305,0,0q��� -9   $ showList��$��	0,334,0,0q�