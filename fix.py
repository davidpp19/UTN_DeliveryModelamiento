import os

def fix_file(path, is_restaurante=False):
    with open(path, 'r', encoding='latin-1') as f:
        content = f.read()
        
    content = content.replace(
        '<th class="text-uppercase text-muted small fw-bold text-center">@Localizer["AccionesTag"]</th>',
        '<th class="text-uppercase text-muted small fw-bold text-center text-nowrap" style="min-width: 280px;">@Localizer["AccionesTag"]</th>'
    )
    content = content.replace(
        '<td class="text-center">',
        '<td class="text-center text-nowrap d-flex gap-2 justify-content-center">'
    )
    content = content.replace(
        'btn-sm btn-danger btn-confirm" data-title="¿Eliminar registro?" data-text="Esta acción no se puede deshacer" data-danger="true" data-confirm="Sí, eliminar" data-bs-toggle="modal"',
        'btn-sm btn-danger" data-bs-toggle="modal"'
    )
    
    split_target = '''                        </td>
                    </tr>

                    <!-- Modal Mostrar Información -->'''
    split_target = split_target.encode('utf-8').decode('latin-1')
                    
    rep_middle = '''                        </td>
                    </tr>
                }
            </tbody>
        </table>
        </div>

        @section Modals {
            @foreach (var item in Model)
            {
                var avatarUrl = !string.IsNullOrEmpty(item.FotoPerfilUrl) ? item.FotoPerfilUrl : "https://ui-avatars.com/api/?name=" + Uri.EscapeDataString(item.Nombres) + "&background=random";
                var modalId = "modalRepartidor" + item.Id;
                var modalRechazoId = "modalRechazo" + item.Id;

                <!-- Modal Mostrar Información -->'''
    rep_middle = rep_middle.encode('utf-8').decode('latin-1')
                
    rest_middle = '''                        </td>
                    </tr>
                }
            </tbody>
        </table>
        </div>

        @section Modals {
            @foreach (var item in Model)
            {
                var logoUrl = !string.IsNullOrEmpty(item.LogoUrl) ? item.LogoUrl : "https://ui-avatars.com/api/?name=" + Uri.EscapeDataString(item.Nombre) + "&background=random";
                var avatarPropietario = !string.IsNullOrEmpty(item.FotoPerfilUrl) ? item.FotoPerfilUrl : "https://ui-avatars.com/api/?name=" + Uri.EscapeDataString(item.NombresPropietario) + "&background=random";
                var modalId = "modalRestaurante" + item.Id;
                var modalRechazoId = "modalRechazo" + item.Id;

                <!-- Modal Mostrar Información -->'''
    rest_middle = rest_middle.encode('utf-8').decode('latin-1')
                
    content = content.replace(split_target, rest_middle if is_restaurante else rep_middle)
    
    end_target = '''                    </div>
                }
            </tbody>
        </table>
        </div>
    }
</div>'''
    end_new = '''                    </div>
                }
        }
    }
</div>'''
    content = content.replace(end_target, end_new)
    
    with open(path, 'w', encoding='utf-8') as f:
        f.write(content.encode('latin-1').decode('utf-8', errors='replace'))

dir_path = r'C:\Users\david\OneDrive\Desktop\UTN_archivos\TERCER_SEMESTRE\MODELAMIENTO_DE_SOFTWARE\UTN_DeliveryModelamiento\Delivery.MVC\Views\AdminAprobaciones'
fix_file(os.path.join(dir_path, 'Repartidores.cshtml'), False)
fix_file(os.path.join(dir_path, 'Restaurantes.cshtml'), True)
print("Done!")
