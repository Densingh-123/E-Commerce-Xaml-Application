import os
import json

assets_dir = r"c:\Users\Dens\source\repos\Food\Food\Assets"
products = []

categories = {
    "burger": "Burger",
    "pizza": "Pizza",
    "coffee": "Coffee",
    "pasta": "Pasta",
    "c": "Coffee",
    "drink": "Drinks",
    "dessert": "Dessert",
    "salad": "Salad",
    "fried-chicken": "Chicken",
    "roll": "Rolls",
    "sandwich": "Sandwich"
}

def get_category(name):
    name = name.lower()
    for key, cat in categories.items():
        if key in name:
            return cat
    return "Other"

id_counter = 1
for root, dirs, files in os.walk(assets_dir):
    for file in files:
        if file.lower().endswith(('.png', '.jpg', '.jpeg')):
            path = os.path.relpath(os.path.join(root, file), r"c:\Users\Dens\source\repos\Food\Food")
            path = path.replace("\\", "/")
            
            # Skip website assets or hero images for products
            if "website" in path or "hero" in path or "carousel" in path or "logo" in path:
                continue
                
            name = file.split('.')[0].replace('-', ' ').title()
            category = get_category(name)
            
            products.append({
                "Name": name,
                "Description": f"Delicious {name} prepared with fresh ingredients.",
                "Price": 5.0 + (id_counter % 15),
                "ImagePath": path,
                "Category": category
            })
            id_counter += 1

with open(r"c:\Users\Dens\source\repos\Food\Food\products.json", "w") as f:
    json.dump(products, f, indent=2)
