// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// ==========================================
// UI ENHANCEMENTS: Skeleton & SweetAlert2
// ==========================================

document.addEventListener('DOMContentLoaded', function () {
    // 1. Remove Skeleton Loader once page loads
    const skeletonTables = document.querySelectorAll('.skeleton-loader');
    
    if (skeletonTables.length > 0) {
        // Simulate network delay or wait for actual load
        setTimeout(() => {
            skeletonTables.forEach(table => {
                table.classList.remove('skeleton-loader');
            });
        }, 800); // 800ms delay to show the effect for UX
    }

    // 2. SweetAlert2 Confirmation Hooks
    // usage: <form class="form-confirm" data-title="¿Estás seguro?" data-text="Esta acción no se puede deshacer" data-confirm="Sí, eliminar">
    const confirmForms = document.querySelectorAll('.form-confirm');
    
    confirmForms.forEach(form => {
        form.addEventListener('submit', function (e) {
            e.preventDefault();
            const title = form.getAttribute('data-title') || '¿Estás seguro?';
            const text = form.getAttribute('data-text') || 'Esta acción no se puede deshacer';
            const confirmText = form.getAttribute('data-confirm') || 'Sí, confirmar';
            const isDanger = form.hasAttribute('data-danger');

            Swal.fire({
                title: title,
                text: text,
                icon: isDanger ? 'warning' : 'question',
                showCancelButton: true,
                confirmButtonColor: isDanger ? '#ef4444' : '#a855f7',
                cancelButtonColor: '#3f3f46',
                confirmButtonText: confirmText,
                cancelButtonText: 'Cancelar',
                background: 'rgba(20, 20, 20, 0.95)',
                color: '#fff',
                customClass: {
                    popup: 'glass-card border border-secondary',
                    confirmButton: 'btn-liquid btn-liquid-primary',
                    cancelButton: 'btn btn-outline-secondary rounded-pill'
                }
            }).then((result) => {
                if (result.isConfirmed) {
                    // Si confirmó, hace submit normal
                    form.submit();
                }
            });
        });
    });

    // 3. SweetAlert2 Confirmation Hooks for Links (anchor tags)
    // usage: <a href="..." class="btn-confirm" data-title="¿Eliminar?" data-text="..." data-confirm="Sí">
    const confirmLinks = document.querySelectorAll('.btn-confirm');
    
    confirmLinks.forEach(link => {
        link.addEventListener('click', function (e) {
            e.preventDefault();
            const href = link.getAttribute('href');
            const title = link.getAttribute('data-title') || '¿Estás seguro?';
            const text = link.getAttribute('data-text') || 'Esta acción no se puede deshacer';
            const confirmText = link.getAttribute('data-confirm') || 'Sí, confirmar';
            const isDanger = link.hasAttribute('data-danger') || link.classList.contains('btn-danger') || link.classList.contains('text-danger');

            Swal.fire({
                title: title,
                text: text,
                icon: isDanger ? 'warning' : 'question',
                showCancelButton: true,
                confirmButtonColor: isDanger ? '#ef4444' : '#a855f7',
                cancelButtonColor: '#3f3f46',
                confirmButtonText: confirmText,
                cancelButtonText: 'Cancelar',
                background: 'rgba(20, 20, 20, 0.95)',
                color: '#fff',
                customClass: {
                    popup: 'glass-card border border-secondary',
                    confirmButton: 'btn-liquid btn-liquid-primary',
                    cancelButton: 'btn btn-outline-secondary rounded-pill'
                }
            }).then((result) => {
                if (result.isConfirmed) {
                    window.location.href = href;
                }
            });
        });
    });
});
