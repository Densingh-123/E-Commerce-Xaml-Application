import sys

def fix_lines(filepath):
    with open(filepath, 'r', encoding='utf-8') as f:
        lines = f.readlines()

    lines[268] = lines[268].replace('ðŸ‘ ', '👁️')
    lines[300] = lines[300].replace('ðŸ‘ ', '👁️')
    lines[403] = lines[403].replace('ðŸ” ', '🔍')
        
    with open(filepath, 'w', encoding='utf-8') as f:
        f.writelines(lines)

fix_lines('MainWindow.xaml')
