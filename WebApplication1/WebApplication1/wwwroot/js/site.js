document.addEventListener('DOMContentLoaded', () => {
    const btn = document.getElementById('theme-toggle');
    const html = document.documentElement;

    const applyTheme = (t) => {
        html.setAttribute('data-theme', t);
        localStorage.setItem('sd-theme', t);
        btn.textContent = t === 'dark' ? '☀️' : '🌙';
    };

    const saved = localStorage.getItem('sd-theme') || 'light';
    applyTheme(saved);

    btn.addEventListener('click', () => {
        const current = html.getAttribute('data-theme');
        applyTheme(current === 'dark' ? 'light' : 'dark');
    });
});
