using System;
using System.IO;
using System.Text;

class Program
{
    static void Main()
    {
        string dir = @"C:\Users\david\OneDrive\Desktop\UTN_archivos\TERCER_SEMESTRE\MODELAMIENTO_DE_SOFTWARE\UTN_DeliveryModelamiento\Delivery.MVC\Views\AdminAprobaciones";
        
        // Repartidores
        string pathRep = Path.Combine(dir, "Repartidores.cshtml");
        string contentRep = File.ReadAllText(pathRep, Encoding.UTF8);
        
        contentRep = contentRep.Replace(
            "<th class=\"text-uppercase text-muted small fw-bold text-center\">@Localizer[\"AccionesTag\"]</th>",
            "<th class=\"text-uppercase text-muted small fw-bold text-center text-nowrap\" style=\"min-width: 280px;\">@Localizer[\"AccionesTag\"]</th>");
            
        contentRep = contentRep.Replace(
            "<td class=\"text-center\">",
            "<td class=\"text-center text-nowrap d-flex gap-2 justify-content-center\">");
            
        contentRep = contentRep.Replace(
            "btn-sm btn-danger btn-confirm\" data-title=\"¿Eliminar registro?\" data-text=\"Esta acción no se puede deshacer\" data-danger=\"true\" data-confirm=\"Sí, eliminar\" data-bs-toggle=\"modal\"",
            "btn-sm btn-danger\" data-bs-toggle=\"modal\"");
            
        // Move modals out
        string splitRep = @"                        </td>
                    </tr>

                    <!-- Modal Mostrar Información -->";
        string replaceRep = @"                        </td>
                    </tr>
                }
            </tbody>
        </table>
        </div>

        @section Modals {
            @foreach (var item in Model)
            {
                var avatarUrl = !string.IsNullOrEmpty(item.FotoPerfilUrl) ? item.FotoPerfilUrl : ""https://ui-avatars.com/api/?name="" + Uri.EscapeDataString(item.Nombres) + ""&background=random"";
                var modalId = ""modalRepartidor"" + item.Id;
                var modalRechazoId = ""modalRechazo"" + item.Id;

                <!-- Modal Mostrar Información -->";
        contentRep = contentRep.Replace(splitRep, replaceRep);
        
        string endRep = @"                    </div>
                }
            </tbody>
        </table>
        </div>
    }
</div>";
        string endRepNew = @"                    </div>
                }
        }
    }
</div>";
        contentRep = contentRep.Replace(endRep, endRepNew);
        File.WriteAllText(pathRep, contentRep, new UTF8Encoding(false));


        // Restaurantes
        string pathRest = Path.Combine(dir, "Restaurantes.cshtml");
        string contentRest = File.ReadAllText(pathRest, Encoding.UTF8);
        
        contentRest = contentRest.Replace(
            "<th class=\"text-uppercase text-muted small fw-bold text-center\">@Localizer[\"AccionesTag\"]</th>",
            "<th class=\"text-uppercase text-muted small fw-bold text-center text-nowrap\" style=\"min-width: 280px;\">@Localizer[\"AccionesTag\"]</th>");
            
        contentRest = contentRest.Replace(
            "<td class=\"text-center\">",
            "<td class=\"text-center text-nowrap d-flex gap-2 justify-content-center\">");
            
        contentRest = contentRest.Replace(
            "btn-sm btn-danger btn-confirm\" data-title=\"¿Eliminar registro?\" data-text=\"Esta acción no se puede deshacer\" data-danger=\"true\" data-confirm=\"Sí, eliminar\" data-bs-toggle=\"modal\"",
            "btn-sm btn-danger\" data-bs-toggle=\"modal\"");
            
        string splitRest = @"                        </td>
                    </tr>

                    <!-- Modal Mostrar Información -->";
        string replaceRest = @"                        </td>
                    </tr>
                }
            </tbody>
        </table>
        </div>

        @section Modals {
            @foreach (var item in Model)
            {
                var logoUrl = !string.IsNullOrEmpty(item.LogoUrl) ? item.LogoUrl : ""https://ui-avatars.com/api/?name="" + Uri.EscapeDataString(item.Nombre) + ""&background=random"";
                var avatarPropietario = !string.IsNullOrEmpty(item.FotoPerfilUrl) ? item.FotoPerfilUrl : ""https://ui-avatars.com/api/?name="" + Uri.EscapeDataString(item.NombresPropietario) + ""&background=random"";
                var modalId = ""modalRestaurante"" + item.Id;
                var modalRechazoId = ""modalRechazo"" + item.Id;

                <!-- Modal Mostrar Información -->";
        contentRest = contentRest.Replace(splitRest, replaceRest);
        
        contentRest = contentRest.Replace(endRep, endRepNew);
        File.WriteAllText(pathRest, contentRest, new UTF8Encoding(false));
        
        Console.WriteLine(""Done replacing!"");
    }
}
