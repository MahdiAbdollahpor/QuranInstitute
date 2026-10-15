// ═══════════════════════════════════════════
// Toast helper
// ═══════════════════════════════════════════
window.qiToast = function (message, type = 'default', duration = 3000) {
    const el = document.createElement('div');
    el.className = `qi-toast ${type}`;
    el.textContent = message;
    document.body.appendChild(el);

    requestAnimationFrame(() => el.classList.add('show'));

    setTimeout(() => {
        el.classList.remove('show');
        setTimeout(() => el.remove(), 300);
    }, duration);
};