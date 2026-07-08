$entities = @(
    @{ Name = 'Rol'; Route = 'Roles'; Type = 'long'; PK = 'Id' },
    @{ Name = 'Usuario'; Route = 'Usuarios'; Type = 'long'; PK = 'Id' },
    @{ Name = 'Direccion'; Route = 'Direcciones'; Type = 'long'; PK = 'Id' },
    @{ Name = 'Restaurante'; Route = 'Restaurantes'; Type = 'long'; PK = 'Id' },
    @{ Name = 'CategoriaProducto'; Route = 'CategoriasProducto'; Type = 'long'; PK = 'Id' },
    @{ Name = 'Producto'; Route = 'Productos'; Type = 'long'; PK = 'Id' },
    @{ Name = 'Repartidor'; Route = 'Repartidores'; Type = 'long'; PK = 'UsuarioId' },
    @{ Name = 'Vehiculo'; Route = 'Vehiculos'; Type = 'long'; PK = 'Id' },
    @{ Name = 'UbicacionActualRepartidor'; Route = 'UbicacionesActuales'; Type = 'long'; PK = 'RepartidorId' },
    @{ Name = 'HistorialAsignacionesRepartidor'; Route = 'HistorialAsignaciones'; Type = 'long'; PK = 'Id' },
    @{ Name = 'Pedido'; Route = 'Pedidos'; Type = 'long'; PK = 'Id' },
    @{ Name = 'DetallePedido'; Route = 'DetallesPedido'; Type = 'long'; PK = 'Id' },
    @{ Name = 'Pago'; Route = 'Pagos'; Type = 'long'; PK = 'Id' },
    @{ Name = 'Resena'; Route = 'Resenas'; Type = 'long'; PK = 'Id' },
    @{ Name = 'Cupon'; Route = 'Cupones'; Type = 'long'; PK = 'Id' }
)

New-Item -ItemType Directory -Force -Path 'Delivery.MVC\Controllers'

foreach ($e in $entities) {
    $name = $e.Name
    $route = $e.Route
    $type = $e.Type
    $pk = $e.PK

    New-Item -ItemType Directory -Force -Path "Delivery.MVC\Views\$route"

    # Controller
    $controllerContent = @"
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Delivery.Modelos.Entidades;
using Delivery.Consumer.Interfaces;

namespace Delivery.MVC.Controllers
{
    public class ${route}Controller : Controller
    {
        private readonly I${name}Consumer _consumer;

        public ${route}Controller(I${name}Consumer consumer)
        {
            _consumer = consumer;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _consumer.GetAllAsync();
            return View(data);
        }

        public async Task<IActionResult> Details(${type} id)
        {
            var data = await _consumer.GetByIdAsync(id);
            if (data == null) return NotFound();
            return View(data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(${name} entity)
        {
            await _consumer.CreateAsync(entity);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(${type} id)
        {
            var data = await _consumer.GetByIdAsync(id);
            if (data == null) return NotFound();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(${type} id, ${name} entity)
        {
            await _consumer.UpdateAsync(id, entity);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(${type} id)
        {
            var data = await _consumer.GetByIdAsync(id);
            if (data == null) return NotFound();
            return View(data);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(${type} id)
        {
            await _consumer.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
"@
    Set-Content -Path "Delivery.MVC\Controllers\${route}Controller.cs" -Value $controllerContent

    # Index View
    $indexView = @"
@model IEnumerable<Delivery.Modelos.Entidades.${name}>
<h1>${name} - Index</h1>
<p><a asp-action="Create">Create New</a></p>
<table border="1">
    <thead>
        <tr>
            <th>Id / Info</th>
            <th>Acciones</th>
        </tr>
    </thead>
    <tbody>
@foreach (var item in Model) {
        <tr>
            <td>@Html.DisplayFor(modelItem => item.${pk})</td>
            <td>
                <a asp-action="Edit" asp-route-id="@item.${pk}">Edit</a> |
                <a asp-action="Details" asp-route-id="@item.${pk}">Details</a> |
                <a asp-action="Delete" asp-route-id="@item.${pk}">Delete</a>
            </td>
        </tr>
}
    </tbody>
</table>
"@
    Set-Content -Path "Delivery.MVC\Views\$route\Index.cshtml" -Value $indexView

    # Create View
    $createView = @"
@model Delivery.Modelos.Entidades.${name}
<h1>Create ${name}</h1>
<form asp-action="Create">
    @Html.EditorForModel()
    <br/>
    <input type="submit" value="Create" />
</form>
<a asp-action="Index">Back to List</a>
"@
    Set-Content -Path "Delivery.MVC\Views\$route\Create.cshtml" -Value $createView

    # Edit View
    $editView = @"
@model Delivery.Modelos.Entidades.${name}
<h1>Edit ${name}</h1>
<form asp-action="Edit">
    @Html.HiddenFor(m => m.${pk})
    @Html.EditorForModel()
    <br/>
    <input type="submit" value="Save" />
</form>
<a asp-action="Index">Back to List</a>
"@
    Set-Content -Path "Delivery.MVC\Views\$route\Edit.cshtml" -Value $editView

    # Details View
    $detailsView = @"
@model Delivery.Modelos.Entidades.${name}
<h1>Details ${name}</h1>
<div>
    @Html.DisplayForModel()
</div>
<a asp-action="Edit" asp-route-id="@Model.${pk}">Edit</a> |
<a asp-action="Index">Back to List</a>
"@
    Set-Content -Path "Delivery.MVC\Views\$route\Details.cshtml" -Value $detailsView

    # Delete View
    $deleteView = @"
@model Delivery.Modelos.Entidades.${name}
<h1>Delete ${name}</h1>
<h3>Are you sure you want to delete this?</h3>
<div>
    @Html.DisplayForModel()
</div>
<form asp-action="Delete">
    <input type="hidden" asp-for="${pk}" />
    <input type="submit" value="Delete" /> |
    <a asp-action="Index">Back to List</a>
</form>
"@
    Set-Content -Path "Delivery.MVC\Views\$route\Delete.cshtml" -Value $deleteView
}
