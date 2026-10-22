document.addEventListener('DOMContentLoaded', () => {
  if (!requireAuth()) return;

  const permissions = getPermissions();
  const currentPage = window.location.pathname.split('/').pop() || 'dashboard.html';
  if (!canAccessPage(currentPage, permissions)) {
    const defaultPage = getDefaultPage(permissions);
    if (defaultPage === 'index.html') clearSession();
    window.location.href = defaultPage;
    return;
  }

  document.querySelectorAll('.sidebar-nav a[href]').forEach(link => {
    const page = link.getAttribute('href')?.split('/').pop();
    if (page && !canAccessPage(page, permissions)) {
      link.hidden = true;
    }
  });

  const user = getUser();
  const userInfo = document.getElementById('userInfo');
  if (user && userInfo) {
    userInfo.textContent = `${user.nombreCompleto} | ${user.rol}`;
  }

  const btnLogout = document.getElementById('btnLogout');
  if (btnLogout) {
    btnLogout.addEventListener('click', () => {
      clearSession();
      window.location.href = 'index.html';
    });
  }

  const btnToggleSidebar = document.getElementById('btnToggleSidebar');
  const sidebar = document.getElementById('sidebar');
  if (btnToggleSidebar && sidebar) {
    btnToggleSidebar.addEventListener('click', () => sidebar.classList.toggle('show'));
  }
});
