import os
import re

dtos_dir = 'Delivery.Modelos/DTOs'
files = ['RegistroDto.cs', 'RegistroRepartidorDto.cs', 'RegistroRestauranteDto.cs']

error_messages = set()
for file in files:
    path = os.path.join(dtos_dir, file)
    if os.path.exists(path):
        with open(path, 'r', encoding='utf-8') as f:
            content = f.read()
            matches = re.findall(r'ErrorMessage\s*=\s*"([^"]+)"', content)
            for m in matches:
                error_messages.add(m)

for e in sorted(error_messages):
    print(e)
