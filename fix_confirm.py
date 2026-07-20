import re, os

def fix(path):
    with open(path, encoding="utf-8") as f:
        c = f.read()
    
    # Remove btn-confirm class and data attrs from the Aprobar submit button
    # and move confirmation to the form element
    c = re.sub(
        r'(<form method="post" asp-action="AprobarRepartidor" class="d-inline")>',
        r'\1 form-confirm" data-title="Aprobar solicitud" data-text="Confirma la aprobacion de este repartidor" data-confirm="Si, aprobar">',
        c
    )
    c = re.sub(
        r'class="btn btn-sm btn-success me-1 btn-confirm"[^>]*/?>',
        r'class="btn btn-sm btn-success me-1">',
        c
    )

    # Same for Restaurante
    c = re.sub(
        r'(<form method="post" asp-action="AprobarRestaurante" class="d-inline")>',
        r'\1 form-confirm" data-title="Aprobar solicitud" data-text="Confirma la aprobacion de este restaurante" data-confirm="Si, aprobar">',
        c
    )
    
    with open(path, "w", encoding="utf-8") as f:
        f.write(c)
    print("Fixed:", path)

d = r"C:\Users\david\OneDrive\Desktop\UTN_archivos\TERCER_SEMESTRE\MODELAMIENTO_DE_SOFTWARE\UTN_DeliveryModelamiento\Delivery.MVC\Views\AdminAprobaciones"
fix(os.path.join(d, "Repartidores.cshtml"))
fix(os.path.join(d, "Restaurantes.cshtml"))
