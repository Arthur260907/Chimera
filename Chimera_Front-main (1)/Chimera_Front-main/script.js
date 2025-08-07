document.addEventListener('DOMContentLoaded', function() {
  const sidebar = document.getElementById('sidebar');
  const menuToggle = document.getElementById('menu-toggle');
  const closeSidebar = document.getElementById('close-sidebar');
  menuToggle.addEventListener('click', (e) => {
    e.stopPropagation();
    sidebar.classList.add('active');
  });
  closeSidebar.addEventListener('click', () => {
    sidebar.classList.remove('active');
  });
  // Fecha ao clicar fora
  document.addEventListener('click', function(e) {
    if (sidebar.classList.contains('active') && !sidebar.contains(e.target) && e.target !== menuToggle) {
      sidebar.classList.remove('active');
    }
  });
});
