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

// ═══════════════════════════════════════════
// Modal Management
// ═══════════════════════════════════════════
window.openModal = function (id) {
    var el = document.getElementById(id);
    if (el) {
        el.style.display = 'flex';
        document.body.style.overflow = 'hidden';
    }
};

window.closeModal = function (id) {
    var el = document.getElementById(id);
    if (el) {
        el.style.display = 'none';
        document.body.style.overflow = '';
    }
};

// بستن با کلیک بیرون
document.addEventListener('click', function (e) {
    if (e.target.classList && e.target.classList.contains('qi-modal-overlay')) {
        e.target.style.display = 'none';
        document.body.style.overflow = '';
    }
});

// بستن با Esc
document.addEventListener('keydown', function (e) {
    if (e.key === 'Escape') {
        document.querySelectorAll('.qi-modal-overlay').forEach(function (el) {
            if (el.style.display === 'flex') {
                el.style.display = 'none';
            }
        });
        document.body.style.overflow = '';
    }
});