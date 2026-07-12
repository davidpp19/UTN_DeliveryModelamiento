import os
import re

views_dir = r"C:\Users\david\OneDrive\Desktop\UTN_archivos\TERCER_SEMESTRE\MODELAMIENTO_DE_SOFTWARE\UTN_DeliveryModelamiento\Delivery.MVC\Views"

fields_to_remove = [
    "FechaRegistro", "FechaCreacion", "FechaModificacion", "CreadoEn", "ActualizadoEn",
    "AprobadoPor", "FechaAprobacion", "UsuarioAprobador", "UsuarioCreador", "Categorias",
    "Productos", "Detalles", "Restaurante", "Repartidor", "Usuario", "Categoria",
    "DireccionEntrega", "CuponesUsados", "Vehiculos", "EstadoAprobacion",
    "FechaPedido", "FechaEntrega", "FechaUso", "FechaCanje", "FechaHora", "FechaAccion"
]

# ID fields that should be changed to dropdowns (if they aren't hidden or removed entirely depending on context)
id_fields = [
    "UsuarioId", "RestauranteId", "CategoriaId", "RepartidorId", "DireccionId", "RolId", "CuponId", "VehiculoId", "ClienteId"
]

def process_file(filepath):
    with open(filepath, 'r', encoding='utf-8') as f:
        content = f.read()

    original_content = content

    # 1. Remove blocks of fields_to_remove
    # We look for <div class="mb-3">...asp-for="Field"...</div>
    # A simple regex to match the <div class="mb-3"> block containing the field
    for field in fields_to_remove:
        # Regex: match <div class="mb-3"> followed by any chars (non-greedy, not containing <div class="mb-3">) 
        # that contains asp-for="Field" and ends with </div>
        # Since HTML can be tricky, let's use a simpler approach:
        # We split by <div class="mb-3">, and if a segment contains asp-for="Field", we remove the segment.
        pass

    # A better approach: 
    # Find all <div class="mb-3"> tags and their closing </div>
    # Wait, div blocks can be nested. But in scaffolding, they are usually flat inside the form.
    # Let's use regex to match the exact pattern: <div class="mb-3">\s*<label asp-for="FIELD".*?</div>
    for field in fields_to_remove:
        pattern = r'<div class="mb-3">\s*<label asp-for="' + field + r'".*?</div>'
        content = re.sub(pattern, '', content, flags=re.DOTALL)
        # Sometimes there's no label, just the input:
        pattern2 = r'<div class="mb-3">\s*<input asp-for="' + field + r'".*?</div>'
        content = re.sub(pattern2, '', content, flags=re.DOTALL)
        # Checkbox pattern
        pattern3 = r'<div class="mb-3 form-check">\s*<input class="form-check-input" asp-for="' + field + r'".*?</div>'
        content = re.sub(pattern3, '', content, flags=re.DOTALL)
        
        # Another common scaffolding format:
        pattern4 = r'<div class="form-group">\s*<label asp-for="' + field + r'".*?</div>'
        content = re.sub(pattern4, '', content, flags=re.DOTALL)

    # For ID fields, replace the input with a select
    for id_field in id_fields:
        # We want to replace `<input asp-for="UsuarioId" class="form-control" />`
        # with `<select asp-for="UsuarioId" class="form-control" asp-items="ViewBag.UsuarioId"></select>`
        
        # Match the input tag
        input_pattern = r'<input asp-for="' + id_field + r'" class="form-control" (type="number" )?/>'
        
        # Replacement
        select_tag = f'<select asp-for="{id_field}" class="form-control" asp-items="ViewBag.{id_field}"></select>'
        
        content = re.sub(input_pattern, select_tag, content)

    if content != original_content:
        with open(filepath, 'w', encoding='utf-8') as f:
            f.write(content)
        print(f"Updated: {filepath}")

for root, dirs, files in os.walk(views_dir):
    for file in files:
        if file.endswith('.cshtml') and (file == 'Create.cshtml' or file == 'Edit.cshtml'):
            process_file(os.path.join(root, file))

print("Done.")
