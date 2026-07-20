import os

def fix_modals(path):
    with open(path, 'r', encoding='utf-8') as f:
        content = f.read()
        
    content = content.replace(
        '<div class="modal-content">',
        '<div class="modal-content bg-dark text-light border-secondary shadow-lg">'
    )
    content = content.replace(
        '<div class="modal-header">',
        '<div class="modal-header border-secondary">'
    )
    content = content.replace(
        '<div class="modal-footer">',
        '<div class="modal-footer border-secondary">'
    )
    content = content.replace(
        'class="btn-close"',
        'class="btn-close btn-close-white"'
    )
    
    with open(path, 'w', encoding='utf-8') as f:
        f.write(content)

dir_path = r'C:\Users\david\OneDrive\Desktop\UTN_archivos\TERCER_SEMESTRE\MODELAMIENTO_DE_SOFTWARE\UTN_DeliveryModelamiento\Delivery.MVC\Views\AdminAprobaciones'
fix_modals(os.path.join(dir_path, 'Repartidores.cshtml'))
fix_modals(os.path.join(dir_path, 'Restaurantes.cshtml'))
print("Done!")
