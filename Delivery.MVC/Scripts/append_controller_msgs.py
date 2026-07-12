import os

keys_en = {
    'Credenciales incorrectas.': 'Invalid credentials.',
    'Ocurrió un error al registrar el usuario. Es posible que el correo o la cédula ya estén registrados.': 'An error occurred while registering the user. The email or ID may already be registered.',
    'El correo es requerido.': 'Email is required.',
    'La fotografía de la licencia es obligatoria para motos y carros.': 'A license photo is mandatory for motorcycles and cars.',
    'Registro exitoso. Tu cuenta está pendiente de aprobación por el administrador.': 'Registration successful. Your account is pending administrator approval.',
    'No se pudo completar el registro. El correo puede ya estar registrado.': 'Registration could not be completed. The email may already be registered.',
    'Hubo un error en el registro. Verifique que el correo o RUC no estén en uso.': 'There was a registration error. Check that the email or Tax ID are not in use.'
}

def append_to_resx(filepath, keys):
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()
    
    xml_data = ""
    for k, v in keys.items():
        safe_k = k.replace("&", "&amp;").replace("<", "&lt;").replace(">", "&gt;").replace('"', "&quot;").replace("'", "&apos;")
        safe_v = v.replace("&", "&amp;").replace("<", "&lt;").replace(">", "&gt;").replace('"', "&quot;").replace("'", "&apos;")
        xml_data += f'  <data name="{safe_k}" xml:space="preserve"><value>{safe_v}</value></data>\n'
    
    content = content.replace('</root>', xml_data + '</root>')
    
    with open(filepath, 'w', encoding='utf-8') as f:
        f.write(content)

append_to_resx('Delivery.MVC/Resources/SharedResource.en.resx', keys_en)
