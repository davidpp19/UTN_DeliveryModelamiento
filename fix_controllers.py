import os
import re

controllers_dir = r"C:\Users\david\OneDrive\Desktop\UTN_archivos\TERCER_SEMESTRE\MODELAMIENTO_DE_SOFTWARE\UTN_DeliveryModelamiento\Delivery.MVC\Controllers"

def replace_in_file(filepath, pattern, replacement):
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()
    content = re.sub(pattern, replacement, content, flags=re.DOTALL)
    with open(filepath, 'w', encoding='utf-8') as f:
        f.write(content)

# 1. UsuariosController: needs IRolConsumer
# But actually, adding DI is complicated.
