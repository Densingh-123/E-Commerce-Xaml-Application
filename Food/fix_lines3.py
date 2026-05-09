import sys

def fix_lines(filepath):
    with open(filepath, 'r', encoding='utf-8') as f:
        lines = f.readlines()

    lines[519] = '                                        <TextBlock Text="← BACK TO MARKETPLACE" Foreground="Gray" FontSize="14" FontWeight="Bold" />\n'
    lines[643] = '                                         <TextBlock Text="← RETURN TO BASKET" Foreground="Gray" FontWeight="Bold"/>\n'
    lines[875] = '                                     <TextBlock Text="← BACK TO ORDER HISTORY" Foreground="Gray" FontWeight="Bold"/>\n'
    lines[949] = '                                                <TextBlock Text="⏻" Margin="0,0,10,0" VerticalAlignment="Center" FontSize="18"/>\n'
    lines[1039] = '                                                            <Button Grid.Column="1" Content="📁" Command="{Binding UploadProductImageCommand}" Background="Transparent" BorderThickness="0" FontSize="18" Margin="10,0,0,0" ToolTip="Upload Local Image"/>\n'
    lines[1254] = '                                                                <Button Grid.Column="1" Content="📁" Command="{Binding UploadCarouselImageCommand}" Background="Transparent" BorderThickness="0" Margin="10,0,0,0"/>\n'
    lines[1294] = '                                                                <Button Grid.Column="1" Content="📁" Command="{Binding UploadOfferImageCommand}" Background="Transparent" BorderThickness="0" Margin="10,0,0,0"/>\n'
    lines[1335] = '                                                                <Button Grid.Column="1" Content="📁" Command="{Binding UploadAdImageCommand}" Background="Transparent" BorderThickness="0" Margin="10,0,0,0"/>\n'
        
    with open(filepath, 'w', encoding='utf-8') as f:
        f.writelines(lines)

fix_lines('MainWindow.xaml')
