using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ViewGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../.."));
            var mvcPath = Path.Combine(basePath, "Delivery.MVC");
            
            var controllers = new[]
            {
                "ClienteDirecciones", "ClienteFavoritos"
            };

            var entityTypes = typeof(Delivery.Modelos.Entidades.Usuario).Assembly.GetTypes()
                .Where(t => t.Namespace == "Delivery.Modelos.Entidades" && t.IsClass)
                .ToList();

            // 1. Añadir Authorize a controladores
            foreach (var ctrl in controllers)
            {
                var ctrlPath = Path.Combine(mvcPath, "Controllers", $"{ctrl}Controller.cs");
                if (File.Exists(ctrlPath))
                {
                    var content = File.ReadAllText(ctrlPath);
                    if (!content.Contains("[Authorize(Roles = \"Admin\")]"))
                    {
                        if (!content.Contains("using Microsoft.AspNetCore.Authorization;"))
                        {
                            content = content.Replace("using Microsoft.AspNetCore.Mvc;", "using Microsoft.AspNetCore.Mvc;\nusing Microsoft.AspNetCore.Authorization;");
                        }
                        content = Regex.Replace(content, $@"public class {ctrl}Controller : Controller", $"[Authorize(Roles = \"Admin\")]\n    public class {ctrl}Controller : Controller");
                        File.WriteAllText(ctrlPath, content);
                        Console.WriteLine($"Secured {ctrl}Controller.");
                    }
                }
            }

            // 2. Generar vistas funcionales
            foreach (var ctrl in controllers)
            {
                string entityName = ctrl;
                if (ctrl == "ClienteDirecciones") entityName = "Direccion";
                if (ctrl == "ClienteFavoritos") entityName = "Favorito";

                var type = entityTypes.FirstOrDefault(t => t.Name == entityName);
                if (type == null)
                {
                    Console.WriteLine($"WARNING: Entity type {entityName} not found.");
                    continue;
                }

                var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                
                var viewFolder = Path.Combine(mvcPath, "Views", ctrl);
                if (!Directory.Exists(viewFolder)) Directory.CreateDirectory(viewFolder);

                // INDEX
                var indexSb = new StringBuilder();
                indexSb.AppendLine($"@model IEnumerable<Delivery.Modelos.Entidades.{entityName}>");
                indexSb.AppendLine($"@{{ ViewData[\"Title\"] = \"{ctrl}\"; }}");
                indexSb.AppendLine($"<h1 class=\"my-4\">{ctrl}</h1>");
                indexSb.AppendLine($"<p><a asp-action=\"Create\" class=\"btn btn-primary\">Crear Nuevo</a></p>");
                indexSb.AppendLine("<div class=\"table-responsive\">");
                indexSb.AppendLine("<table class=\"table table-striped table-hover align-middle\">");
                indexSb.AppendLine("<thead class=\"table-dark\"><tr>");
                foreach (var p in props.Take(5)) indexSb.AppendLine($"<th>{p.Name}</th>");
                indexSb.AppendLine("<th>Acciones</th></tr></thead><tbody>");
                indexSb.AppendLine("@foreach (var item in Model) {<tr>");
                foreach (var p in props.Take(5)) indexSb.AppendLine($"<td>@Html.DisplayFor(modelItem => item.{p.Name})</td>");
                indexSb.AppendLine("<td>");
                indexSb.AppendLine($"<a asp-action=\"Edit\" asp-route-id=\"@item.Id\" class=\"btn btn-sm btn-warning\">Editar</a>");
                indexSb.AppendLine($"<a asp-action=\"Details\" asp-route-id=\"@item.Id\" class=\"btn btn-sm btn-info text-white\">Detalles</a>");
                indexSb.AppendLine($"<a asp-action=\"Delete\" asp-route-id=\"@item.Id\" class=\"btn btn-sm btn-danger\">Eliminar</a>");
                indexSb.AppendLine("</td></tr>}");
                indexSb.AppendLine("</tbody></table></div>");
                File.WriteAllText(Path.Combine(viewFolder, "Index.cshtml"), indexSb.ToString());

                // CREATE
                var createSb = new StringBuilder();
                createSb.AppendLine($"@model Delivery.Modelos.Entidades.{entityName}");
                createSb.AppendLine($"@{{ ViewData[\"Title\"] = \"Crear {entityName}\"; }}");
                createSb.AppendLine($"<h1 class=\"my-4\">Crear {entityName}</h1><hr />");
                createSb.AppendLine($"<div class=\"row\"><div class=\"col-md-6\">");
                createSb.AppendLine($"<form asp-action=\"Create\">");
                createSb.AppendLine($"<div asp-validation-summary=\"ModelOnly\" class=\"text-danger\"></div>");
                foreach (var p in props.Where(p => p.Name != "Id"))
                {
                    createSb.AppendLine($"<div class=\"mb-3\">");
                    createSb.AppendLine($"<label asp-for=\"{p.Name}\" class=\"form-label\"></label>");
                    if (p.PropertyType == typeof(bool))
                        createSb.AppendLine($"<input asp-for=\"{p.Name}\" class=\"form-check-input\" type=\"checkbox\" />");
                    else if (p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?))
                        createSb.AppendLine($"<input asp-for=\"{p.Name}\" class=\"form-control\" type=\"datetime-local\" />");
                    else
                        createSb.AppendLine($"<input asp-for=\"{p.Name}\" class=\"form-control\" />");
                    createSb.AppendLine($"<span asp-validation-for=\"{p.Name}\" class=\"text-danger\"></span></div>");
                }
                createSb.AppendLine($"<div class=\"mb-3\"><button type=\"submit\" class=\"btn btn-primary\">Guardar</button></div>");
                createSb.AppendLine($"</form></div></div>");
                createSb.AppendLine($"<div><a asp-action=\"Index\" class=\"btn btn-outline-secondary\">Volver al listado</a></div>");
                File.WriteAllText(Path.Combine(viewFolder, "Create.cshtml"), createSb.ToString());

                // EDIT
                var editSb = new StringBuilder();
                editSb.AppendLine($"@model Delivery.Modelos.Entidades.{entityName}");
                editSb.AppendLine($"@{{ ViewData[\"Title\"] = \"Editar {entityName}\"; }}");
                editSb.AppendLine($"<h1 class=\"my-4\">Editar {entityName}</h1><hr />");
                editSb.AppendLine($"<div class=\"row\"><div class=\"col-md-6\">");
                editSb.AppendLine($"<form asp-action=\"Edit\">");
                editSb.AppendLine($"<div asp-validation-summary=\"ModelOnly\" class=\"text-danger\"></div>");
                editSb.AppendLine($"<input type=\"hidden\" asp-for=\"Id\" />");
                foreach (var p in props.Where(p => p.Name != "Id"))
                {
                    editSb.AppendLine($"<div class=\"mb-3\">");
                    editSb.AppendLine($"<label asp-for=\"{p.Name}\" class=\"form-label\"></label>");
                    if (p.PropertyType == typeof(bool))
                        editSb.AppendLine($"<input asp-for=\"{p.Name}\" class=\"form-check-input\" type=\"checkbox\" />");
                    else if (p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?))
                        editSb.AppendLine($"<input asp-for=\"{p.Name}\" class=\"form-control\" type=\"datetime-local\" />");
                    else
                        editSb.AppendLine($"<input asp-for=\"{p.Name}\" class=\"form-control\" />");
                    editSb.AppendLine($"<span asp-validation-for=\"{p.Name}\" class=\"text-danger\"></span></div>");
                }
                editSb.AppendLine($"<div class=\"mb-3\"><button type=\"submit\" class=\"btn btn-warning\">Actualizar</button></div>");
                editSb.AppendLine($"</form></div></div>");
                editSb.AppendLine($"<div><a asp-action=\"Index\" class=\"btn btn-outline-secondary\">Volver al listado</a></div>");
                File.WriteAllText(Path.Combine(viewFolder, "Edit.cshtml"), editSb.ToString());

                // DETAILS
                var detailsSb = new StringBuilder();
                detailsSb.AppendLine($"@model Delivery.Modelos.Entidades.{entityName}");
                detailsSb.AppendLine($"@{{ ViewData[\"Title\"] = \"Detalles {entityName}\"; }}");
                detailsSb.AppendLine($"<h1 class=\"my-4\">Detalles {entityName}</h1><hr />");
                detailsSb.AppendLine($"<dl class=\"row\">");
                foreach (var p in props)
                {
                    detailsSb.AppendLine($"<dt class=\"col-sm-3\">@Html.DisplayNameFor(model => model.{p.Name})</dt>");
                    detailsSb.AppendLine($"<dd class=\"col-sm-9\">@Html.DisplayFor(model => model.{p.Name})</dd>");
                }
                detailsSb.AppendLine($"</dl>");
                detailsSb.AppendLine($"<div><a asp-action=\"Edit\" asp-route-id=\"@Model?.Id\" class=\"btn btn-warning\">Editar</a>");
                detailsSb.AppendLine($"<a asp-action=\"Index\" class=\"btn btn-outline-secondary ms-2\">Volver al listado</a></div>");
                File.WriteAllText(Path.Combine(viewFolder, "Details.cshtml"), detailsSb.ToString());

                // DELETE
                var deleteSb = new StringBuilder();
                deleteSb.AppendLine($"@model Delivery.Modelos.Entidades.{entityName}");
                deleteSb.AppendLine($"@{{ ViewData[\"Title\"] = \"Eliminar {entityName}\"; }}");
                deleteSb.AppendLine($"<h1 class=\"my-4 text-danger\">¿Estás seguro de que quieres eliminar esto?</h1><hr />");
                deleteSb.AppendLine($"<dl class=\"row\">");
                foreach (var p in props)
                {
                    deleteSb.AppendLine($"<dt class=\"col-sm-3\">@Html.DisplayNameFor(model => model.{p.Name})</dt>");
                    deleteSb.AppendLine($"<dd class=\"col-sm-9\">@Html.DisplayFor(model => model.{p.Name})</dd>");
                }
                deleteSb.AppendLine($"</dl>");
                deleteSb.AppendLine($"<form asp-action=\"Delete\">");
                deleteSb.AppendLine($"<input type=\"hidden\" asp-for=\"Id\" />");
                deleteSb.AppendLine($"<button type=\"submit\" class=\"btn btn-danger\">Eliminar Confirmado</button>");
                deleteSb.AppendLine($"<a asp-action=\"Index\" class=\"btn btn-outline-secondary ms-2\">Cancelar</a>");
                deleteSb.AppendLine($"</form>");
                File.WriteAllText(Path.Combine(viewFolder, "Delete.cshtml"), deleteSb.ToString());

                Console.WriteLine($"Generated views for {ctrl}");
            }
        }
    }
}
