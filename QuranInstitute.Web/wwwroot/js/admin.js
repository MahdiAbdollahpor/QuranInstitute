// ═══════════════════════════════════════════
// تاگل سایدبار در موبایل
// ═══════════════════════════════════════════
document.addEventListener('DOMContentLoaded', () => {
    const toggle = document.getElementById('sidebarToggle');
    const sidebar = document.querySelector('.admin-sidebar');

    if (toggle && sidebar) {
        toggle.addEventListener('click', () => {
            sidebar.classList.toggle('open');
        });
    }

    // فعال کردن لینک فعلی
    const path = window.location.pathname.toLowerCase();
    document.querySelectorAll('.admin-nav a').forEach(a => {
        const href = (a.getAttribute('href') || '').toLowerCase();
        if (href && href !== '/' && path.startsWith(href)) {
            a.classList.add('active');
        }
    });
});