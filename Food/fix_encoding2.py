import sys

def fix_file(filepath):
    with open(filepath, 'r', encoding='utf-8') as f:
        c = f.read()

    replacements = {
        'âˆ’': '−',
        'â–¢': '□',
        'ðŸ›’': '🛒',
        'â–¾': '▾',
        'ðŸ‘ ': '👤',
        'ðŸ‘¤': '👤',
        'ðŸ” ': '🔍',
        'â†’': '→',
        'â† ': '←',
        'â˜…': '★',
        'â™¥': '♥',
        'âœ•': '✖',
        'ðŸ›¡ï¸ ': '🛡️',
        'ðŸ“¦': '📦',
        'ðŸŽ ': '🎁',
        'â »': '⏻',
        'âœŽ': '✏️',
        'ðŸ—‘': '🗑️',
        'ðŸ“ ': '📁',
        'ðŸŽ': '🎁',
        'ðŸ›¡': '🛡️',
        'ï¸': ''
    }
    
    for k, v in replacements.items():
        c = c.replace(k, v)
        
    with open(filepath, 'w', encoding='utf-8') as f:
        f.write(c)

fix_file('MainWindow.xaml')
