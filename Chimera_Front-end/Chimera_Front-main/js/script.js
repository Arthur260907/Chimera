// Bloco CORRIGIDO para substituir o original em js/script.js
document.addEventListener('DOMContentLoaded', function () {
  const sidebar = document.getElementById('sidebar');
  const menuToggle = document.getElementById('menu-toggle');
  const closeSidebar = document.getElementById('close-sidebar');

  // Verifica se os elementos existem antes de adicionar listeners
  if (menuToggle) { // <-- Verificação adicionada
    menuToggle.addEventListener('click', (e) => {
      e.stopPropagation();
      if (sidebar) sidebar.classList.add('active'); // <-- Verificação adicionada
    });
  }

  if (closeSidebar) { // <-- Verificação adicionada
    closeSidebar.addEventListener('click', () => {
      if (sidebar) sidebar.classList.remove('active'); // <-- Verificação adicionada
    });
  }

  document.addEventListener('click', function (e) {
    // Verificações adicionadas aqui também (garante que sidebar e menuToggle não são null)
    if (sidebar && menuToggle && sidebar.classList.contains('active') && !sidebar.contains(e.target) && e.target !== menuToggle) {
      sidebar.classList.remove('active');
    }
  });
});

// ---------- main carousel + multi-carousel (inicializa APÓS apicatalago.js) ----------
(function () {
  let mainState = { initialized: false, counter: 0, slideWidth: 0, itemsPerSlide: 1, maxIndex: 0 };

  function resetButton(id) {
    const old = document.getElementById(id);
    if (!old || !old.parentNode) return null;
    const clone = old.cloneNode(true);
    old.parentNode.replaceChild(clone, old);
    return document.getElementById(id);
  }

  function createDots(count) {
    const dotsContainer = document.querySelector('.carousel-dots');
    if (!dotsContainer) return [];
    dotsContainer.innerHTML = '';
    const dots = [];
    for (let i = 0; i < count; i++) {
      const dot = document.createElement('span');
      dot.className = 'dot' + (i === 0 ? ' active' : '');
      dot.dataset.slide = i;
      dotsContainer.appendChild(dot);
      dots.push(dot);
    }
    return dots;
  }

  function updateDots(dots, index) {
    if (!dots) return;
    dots.forEach((dot, i) => {
      dot.classList.toggle('active', i === index);
    });
  }

  function initMainCarousel() {
    const carouselSlide = document.querySelector('.carousel-slide');
    if (!carouselSlide) return;

    // reset buttons to avoid listeners duplication
    const prevBtn = resetButton('prevBtn');
    const nextBtn = resetButton('nextBtn');

    const items = carouselSlide.querySelectorAll('.carousel-item.main');
    if (items.length === 0) return;

    // Para o carrossel principal, cada item ocupa 100% da largura
    const visibleWidth = carouselSlide.parentElement ? carouselSlide.parentElement.clientWidth : carouselSlide.clientWidth;
    const itemsPerSlide = 1; // Um item por vez no carrossel principal
    const slideCount = items.length; // Total de slides

    mainState.itemsPerSlide = itemsPerSlide;
    mainState.slideWidth = visibleWidth;
    mainState.counter = 0;
    mainState.maxIndex = slideCount - 1;

    carouselSlide.style.transition = 'transform 0.5s ease-in-out';
    carouselSlide.style.transform = 'translateX(0px)';

    const dots = createDots(slideCount);

    function goTo(index) {
      if (slideCount === 0) return;
      
      // Loop infinito: volta ao início ou final
      let newIndex = index;
      if (index < 0) {
        newIndex = mainState.maxIndex;
      } else if (index > mainState.maxIndex) {
        newIndex = 0;
      }
      
      mainState.counter = newIndex;
      const translateX = -mainState.counter * mainState.slideWidth;
      carouselSlide.style.transform = 'translateX(' + translateX + 'px)';
      updateDots(dots, mainState.counter);
    }

    if (prevBtn) prevBtn.addEventListener('click', () => goTo(mainState.counter - 1));
    if (nextBtn) nextBtn.addEventListener('click', () => goTo(mainState.counter + 1));

    // dot clicks
    const dotsNodes = document.querySelectorAll('.carousel-dots .dot');
    dotsNodes.forEach(dot => dot.addEventListener('click', (e) => {
      const idx = parseInt(e.target.dataset.slide, 10);
      goTo(idx);
    }));

    // recompute width on resize
    function recompute() {
      const newVisible = carouselSlide.parentElement ? carouselSlide.parentElement.clientWidth : carouselSlide.clientWidth;
      mainState.slideWidth = newVisible;
      // reajusta counter se necessário
      if (mainState.counter > mainState.maxIndex) mainState.counter = mainState.maxIndex;
      carouselSlide.style.transform = 'translateX(' + (-mainState.counter * mainState.slideWidth) + 'px)';
    }
    window.addEventListener('resize', recompute);

    mainState.initialized = true;
  }

  //
  // --- FUNÇÃO MODIFICADA PARA NAVEGAÇÃO COM TRANSFORM E LOOP ---
  //
  function initMultiCarousels() {
    const multiCarousels = document.querySelectorAll('.multi-carousel-container');
    
    multiCarousels.forEach(multiCarousel => {
      const carouselList = multiCarousel.querySelector('.carousel-list');
      if (!carouselList) return;

      const prevBtn = multiCarousel.querySelector('.multi-carousel-btn.prev');
      const nextBtn = multiCarousel.querySelector('.multi-carousel-btn.next');
      const items = carouselList.querySelectorAll('.carousel-item');
      if (!items.length) return;

      // Limitar itens exibidos
      const maxItems = 12;
      if (items.length > maxItems) {
        for (let i = items.length - 1; i >= maxItems; i--) {
          const el = items[i];
          if (el && el.parentNode) el.parentNode.removeChild(el);
        }
      }

      const finalItems = carouselList.querySelectorAll('.carousel-item');
      if (!finalItems.length) return;

      // Estado do carrossel
      let currentPosition = 0;
      const itemWidth = finalItems[0].offsetWidth + parseFloat(getComputedStyle(finalItems[0]).marginLeft) + parseFloat(getComputedStyle(finalItems[0]).marginRight);
      const visibleWidth = carouselList.parentElement.clientWidth;
      const itemsPerView = Math.floor(visibleWidth / itemWidth);
      const totalItems = finalItems.length;

      carouselList.style.transform = 'translateX(0px)';

      function updatePosition() {
        const translateX = -currentPosition * itemWidth;
        carouselList.style.transform = `translateX(${translateX}px)`;
      }

      if (prevBtn) {
        prevBtn.addEventListener('click', () => {
          // Loop: volta ao final se estiver no início
          currentPosition = currentPosition - itemsPerView;
          if (currentPosition < 0) {
            currentPosition = Math.max(0, totalItems - itemsPerView);
          }
          updatePosition();
        });
      }

      if (nextBtn) {
        nextBtn.addEventListener('click', () => {
          // Loop: volta ao início se chegar ao final
          currentPosition = currentPosition + itemsPerView;
          if (currentPosition >= totalItems) {
            currentPosition = 0;
          }
          updatePosition();
        });
      }
    });
  }
  // --- FIM DA FUNÇÃO MODIFICADA ---


  // handler executado quando apicatalago.js terminar de inserir imagens e carregar
  document.addEventListener('catalog:loaded', () => {
    initMainCarousel();
    initMultiCarousels();
  });

  // se já foi carregado antes, tenta inicializar (caso scripts executados em ordem diferente)
  if (window.__CHIMERA_MOVIES) {
    document.dispatchEvent(new Event('catalog:loaded'));
  }
})();

document.addEventListener('DOMContentLoaded', () => {
  const registrationForm = document.getElementById('registration-form'); // Certifique-se de que seu formulário em cadastro.html tem este id

  if (registrationForm) {
    registrationForm.addEventListener('submit', async (event) => {
      event.preventDefault(); // Impede o envio padrão do formulário

      // Obtém os dados dos campos do formulário. Certifique-se de que seus inputs têm esses ids.
      const nome = document.getElementById('username').value;
      const email = document.getElementById('email').value;
      const senha = document.getElementById('senha').value;

      // A URL do endpoint de cadastro do seu backend
      const apiUrl = 'https://localhost:DESKTOP-UVKH0C1/api/Usuario/registrar'; // Supondo que sua API está rodando nesta porta

      try {
        const response = await fetch(apiUrl, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({ nome, email, senha }),
        });

        if (response.ok) {
          alert('Cadastro realizado com sucesso!');
          window.location.href = 'Login.html'; // Redireciona para a página de login
        } else {
          const errorData = await response.json();
          alert(`Falha no cadastro: ${errorData.message}`);
        }
      } catch (error) {
        console.error('Erro durante o cadastro:', error);
        alert('Ocorreu um erro. Por favor, tente novamente.');
      }
    });
  }
});