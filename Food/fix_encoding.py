import sys

def fix_file(filepath):
    with open(filepath, 'r', encoding='utf-8') as f:
        c = f.read()

    # The corrupted characters are what happens when UTF-8 is decoded as CP1252 and then encoded back to UTF-8.
    # So we can fix it generally by decoding the string back to bytes using CP1252, and decoding those bytes as UTF-8.
    # However, since PowerShell might have messed up some unmapped characters, we can do targeted replacements.
    
    # Actually, let's try the targeted replacements using unicode escapes or by just putting them in the script.
    replacements = {
        'ðŸ” ': '🔍',
        'â†’': '→',
        'â† ': '←',
        'â˜…â˜…â˜…â˜…â˜…': '★★★★★',
        'â™¥': '♥',
        'âœ•': '✖',
        'ðŸ›¡ï¸ ': '🛡️',
        'ðŸ“¦': '📦',
        'ðŸŽ ': '🎁',
        'â »': '⏻',
        'âœŽ': '✏️',
        'ðŸ—‘': '🗑️',
        'ðŸ“ ': '📁',
        'dY>,?': '🛡️',
        'oZ': '✏️',
        'dY-`': '🗑️'
    }
    
    for k, v in replacements.items():
        c = c.replace(k, v)
        
    with open(filepath, 'w', encoding='utf-8') as f:
        f.write(c)

fix_file('MainWindow.xaml')
