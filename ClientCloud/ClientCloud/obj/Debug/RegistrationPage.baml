   M S B A M L   `   `   `  ����F  BClientCloud, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:ClientCloudClientCloud  5      "    ClientCloud.RegistrationPage   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      +localclr-namespace:ClientCloud     5      X TXceed.Wpf.Toolkit, Version=3.4.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  5        N� Title$  RegistrationPage��5
      .��5      � .+�5   
   �� $
��Left=�6   $��379,104,0,0q�6/   $
��Wrap=�6D   $��
Логин��6X   $	��Top=�6e   $��14(�6}   �� 5   
   $
��Left=�6   $��379,170,0,0q�6/   $
��Wrap=�6D   $��Пароль��6X   $	��Top=�6f   $��14(�6~   �� 5   
   -   5        $��	loginText��5      $
��Left=�6#   $��23��6>   $��342,128,0,0q�6J    �� TextWrapping$
 Wrap=�6_   $�� ��6s   $	��Top=�6{   $	��120��6�   �� 5   
   -   5        $��passwordText��5      $
��Left=�6&   $��23��6A   $��342,194,0,0q�6M   $
 Wrap=�6b   $�� ��6v   $	��Top=�6~   $	��120��6�   �� 5   
   -   5        $��registrationButton��5      $��Регистрация��6+   $
��Left=�6A   $��341,296,0,0q�6\   $	��Top=�6q   $	��120��6�   6�   �� 5   
   $
��Left=�6   $��386,232,0,0q�6/   $
��Wrap=�6D   $��Ключ��6X   $	��Top=�6d   $��14(�6|   �� 5   
   -   5        $��keyText��5      $
��Left=�6!   $��23��6<   $��160,255,0,0q�6H   $
 Wrap=�6]   $�� ��6q   $	��Top=�6y   $	��505��6�   �� 5   
   -   5        $��
backButton��5      $��
Назад��6#   $
��Left=�63   $��512,296,0,0q�6N   $	��Top=�6c   $	��120��6{   6�   %  Xceed.Wpf.Toolkit.BusyIndicator  5   
   -   5        $��loading��5         IsBusy .     BusyContent$! Идет проверка..��6;   5      5      