import os

keys_en = {
    'Confirme su contraseña': 'Confirm your password',
    'El RUC es obligatorio': 'Tax ID (RUC) is required',
    'El apellido es requerido': 'Last name is required',
    'El color es requerido': 'Color is required',
    'El correo es requerido': 'Email is required',
    'El email es obligatorio': 'Email is required',
    'El email no tiene un formato válido': 'Invalid email format',
    'El modelo es requerido': 'Model is required',
    'El nombre del propietario es obligatorio': 'Owner name is required',
    'El nombre del restaurante es obligatorio': 'Restaurant name is required',
    'El nombre es requerido': 'First name is required',
    'El número de licencia es requerido': 'License number is required',
    'El teléfono es obligatorio': 'Phone number is required',
    'El teléfono es requerido': 'Phone number is required',
    'El tipo de vehículo es requerido': 'Vehicle type is required',
    'Formato de correo inválido': 'Invalid email format',
    'La ciudad es obligatoria': 'City is required',
    'La confirmación de la contraseña es obligatoria': 'Password confirmation is required',
    'La contraseña debe tener al menos 6 caracteres': 'Password must be at least 6 characters',
    'La contraseña es obligatoria': 'Password is required',
    'La contraseña es requerida': 'Password is required',
    'La cédula es obligatoria': 'ID number is required',
    'La cédula es requerida': 'ID number is required',
    'La dirección (calle) es obligatoria': 'Street address is required',
    'La marca es requerida': 'Brand is required',
    'La placa es requerida': 'License plate is required',
    'Las contraseñas no coinciden': 'Passwords do not match',
    'Los apellidos del propietario son obligatorios': 'Owner last name is required',
    'Los apellidos son requeridos': 'Last names are required',
    'Máximo 20 caracteres': 'Maximum 20 characters',
    'Mínimo 6 caracteres': 'Minimum 6 characters',
    'Seleccione su ubicación en el mapa': 'Select your location on the map'
}

def append_to_resx(filepath, keys):
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()
    
    xml_data = ""
    for k, v in keys.items():
        # Clean xml attributes
        safe_k = k.replace("&", "&amp;").replace("<", "&lt;").replace(">", "&gt;").replace('"', "&quot;").replace("'", "&apos;")
        safe_v = v.replace("&", "&amp;").replace("<", "&lt;").replace(">", "&gt;").replace('"', "&quot;").replace("'", "&apos;")
        xml_data += f'  <data name="{safe_k}" xml:space="preserve"><value>{safe_v}</value></data>\n'
    
    content = content.replace('</root>', xml_data + '</root>')
    
    with open(filepath, 'w', encoding='utf-8') as f:
        f.write(content)

append_to_resx('Delivery.MVC/Resources/SharedResource.en.resx', keys_en)
# we don't need to append them to .es.resx because they are already in Spanish in the DTO,
# so if the localizer doesn't find them, it just outputs the Spanish string, which is correct!
