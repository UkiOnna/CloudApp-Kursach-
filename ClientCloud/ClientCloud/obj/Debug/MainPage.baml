   M S B A M L   `   `   `  ����F  BClientCloud, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:ClientCloudClientCloud  5          ClientCloud.MainPage   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      X TXceed.Wpf.Toolkit, Version=3.4.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5      %localclr-namespace:ClientCloud  5      -   5          N� Title$  MainPage��5
      .��5      � .+�5   
   �� -   5        $��welcome��5      $
��Left=�6#   $��
470,10,0,0q�6>   $
��Wrap=�6R   $&�� Добро пожаловать ��6f   $	��Top=�6   $��72��6�   $	��320��6�   $��25(�6�    �� TextAlignment$ Center=�6�   �� 5   
   -   5        $��	listFiles��5      $	��364��6#   $��10,10,335,0q�60   $	��Top=�6E   6]   �� 5   
   -   5        $��downloadButton��5      $��Скачать��6'   $
��Left=�69   $��470,160,0,0q�6T   $	��Top=�6i   $	��307��6�   $��34��6�   $��20(�6�   �� 5   
   -   5        $��uploadButton��5      $��Загрузить��6%   $
��Left=�69   $��470,100,0,0q�6T   $	��Top=�6i   $	��307��6�   $��34��6�   $��20(�6�   �� 5   
   -   5        $��refreshButton��5      $��Обновить��6&   $
��Left=�69   $��470,340,0,0q�6T   $	��Top=�6i   $	��307��6�   $��34��6�   $��20(�6�   �� 5   
   -   5        $��deleteButton��5      $��Удалить��6%   $
��Left=�67   $��470,220,0,0q�6R   $	��Top=�6g   $	��307��6   $��34��6�   $��20(�6�   �� 5   
   -   5         ��Name$ createFolderButton��5      $��Создать папку��6-   $
��Left=�6E   $��470,280,0,0q�6`   $	��Top=�6u   $	��307��6�   $��34��6�   $��20(�6�   6�   �� 5   
   -	   5        $��	logButton��5      $��Лог файлов��6"   $
��Left=�67   $��
10,374,0,0q�6R   $	��Top=�6f   $	��136��6~   6�   %  Xceed.Wpf.Toolkit.BusyIndicator  5   
   -
   5        $��loading��5         IsBusy .     BusyContent$/ )Пожалуйста подождите..��6;   5      5      