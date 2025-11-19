// ~/js/sidebar.js
function toggleSidebar() {
    const sidebar = document.getElementById('sidebar');
    const overlay = document.getElementById('sidebarOverlay');
    if (sidebar && overlay) {
        sidebar.classList.toggle('open');
        overlay.classList.toggle('active');
    }
}

function initializeSidebar() {
    // Додаємо обробники кліків на посилання сайдбару
    document.querySelectorAll('.sidebar-link').forEach(link => {
        link.addEventListener('click', () => {
            toggleSidebar();
        });
    });

    // Закриття сайдбару по ESC
    document.addEventListener('keydown', (e) => {
        if (e.key === 'Escape') {
            const sidebar = document.getElementById('sidebar');
            if (sidebar && sidebar.classList.contains('open')) {
                toggleSidebar();
            }
        }
    });

    // Закриття сайдбару по кліку поза ним
    document.addEventListener('click', (e) => {
        const sidebar = document.getElementById('sidebar');
        const toggleBtn = document.querySelector('.sidebar-toggle-nav');
        if (sidebar && sidebar.classList.contains('open') &&
            !sidebar.contains(e.target) &&
            (!toggleBtn || !toggleBtn.contains(e.target))) {
            toggleSidebar();
        }
    });
}

// Ініціалізація при завантаженні сторінки
document.addEventListener('DOMContentLoaded', initializeSidebar);