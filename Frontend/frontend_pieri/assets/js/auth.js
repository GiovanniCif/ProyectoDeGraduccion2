document.addEventListener('DOMContentLoaded', () => {
  const form = document.getElementById('loginForm');
  if (!form) return;

  form.addEventListener('submit', async (e) => {
    e.preventDefault();
    hideAlert('loginAlert');

    const btn = document.getElementById('btnLogin');
    btn.disabled = true;
    btn.textContent = 'Ingresando...';

    try {
      const payload = {
        usernameOrCorreo: document.getElementById('usernameOrCorreo').value.trim(),
        password: document.getElementById('password').value
      };

      const data = await apiRequest('/Auth/login', {
        method: 'POST',
        body: JSON.stringify(payload)
      });

      setToken(data.token);
      setUser({
        username: data.username,
        nombreCompleto: data.nombreCompleto,
        rol: data.rol,
        expiracion: data.expiracion
      });

      window.location.href = getDefaultPage();
    } catch (error) {
      showAlert('loginAlert', error.message, 'danger');
    } finally {
      btn.disabled = false;
      btn.textContent = 'Iniciar sesión';
    }
  });
});
