import sys

def fix_lines(filepath):
    with open(filepath, 'r', encoding='utf-8') as f:
        lines = f.readlines()

    lines[268] = '                                        <TextBlock Text="👁️" FontSize="18" Foreground="Gray"/>\n'
    lines[300] = '                                            <TextBlock Text="👁️" FontSize="18" Foreground="Gray"/>\n'
    lines[403] = '                                                <TextBlock Text="🔍" VerticalAlignment="Center" Margin="10,0" Foreground="Gray" FontSize="18"/>\n'
        
    with open(filepath, 'w', encoding='utf-8') as f:
        f.writelines(lines)

fix_lines('MainWindow.xaml')
