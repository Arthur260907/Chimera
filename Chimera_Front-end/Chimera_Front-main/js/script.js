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

  document.addEventListener('click', function(e) {
    if (sidebar.classList.contains('active') && !sidebar.contains(e.target) && e.target !== menuToggle) {
      sidebar.classList.remove('active');
    }
  });
});

// ---------- main carousel + multi-carousel (inicializa APÓS apicatalago.js) ----------
(function() {
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

    const items = carouselSlide.querySelectorAll('.carousel-item');
    const images = carouselSlide.querySelectorAll('.carousel-item img');
    if (items.length === 0) return;

    // largura visível do container (viewport do carrossel)
    const visibleWidth = carouselSlide.parentElement ? carouselSlide.parentElement.clientWidth : carouselSlide.clientWidth;
    const itemWidth = images[0] && images[0].clientWidth ? images[0].clientWidth : Math.floor(visibleWidth / 4);

    // quantos itens cabem por "página"
    const itemsPerSlide = Math.max(1, Math.floor(visibleWidth / itemWidth));
    const slideCount = Math.max(1, Math.ceil(items.length / itemsPerSlide));

    mainState.itemsPerSlide = itemsPerSlide;
    mainState.slideWidth = visibleWidth;
    mainState.counter = 0;
    mainState.maxIndex = slideCount - 1;

    carouselSlide.style.transition = 'transform 0.5s ease-in-out';
    carouselSlide.style.transform = 'translateX(0px)';

    const dots = createDots(slideCount);

    function goTo(index) {
      if (slideCount === 0) return;
      const clamped = Math.max(0, Math.min(mainState.maxIndex, index));
      mainState.counter = clamped;
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

    // recompute width on resize / when images load
    function recompute() {
      const newVisible = carouselSlide.parentElement ? carouselSlide.parentElement.clientWidth : carouselSlide.clientWidth;
      const newItemWidth = carouselSlide.querySelector('.carousel-item img') ? carouselSlide.querySelector('.carousel-item img').clientWidth : Math.floor(newVisible / 4);
      const newItemsPerSlide = Math.max(1, Math.floor(newVisible / (newItemWidth || Math.floor(newVisible/4))));
      const newSlideCount = Math.max(1, Math.ceil(items.length / newItemsPerSlide));
      mainState.itemsPerSlide = newItemsPerSlide;
      mainState.slideWidth = newVisible;
      mainState.maxIndex = newSlideCount - 1;
      // reajusta counter se necessário
      if (mainState.counter > mainState.maxIndex) mainState.counter = mainState.maxIndex;
      carouselSlide.style.transform = 'translateX(' + (-mainState.counter * mainState.slideWidth) + 'px)';
      // recria dots
      createDots(newSlideCount);
      updateDots(document.querySelectorAll('.carousel-dots .dot'), mainState.counter);
    }
    window.addEventListener('resize', recompute);
    images.forEach(img => img.addEventListener('load', recompute));

    mainState.initialized = true;
  }

  function initMultiCarousels() {
    const multiCarousels = document.querySelectorAll('.multi-carousel-container');
    multiCarousels.forEach(multiCarousel => {
      const carouselList = multiCarousel.querySelector('.carousel-list');
      if (!carouselList) return;
      const items = carouselList.querySelectorAll('.carousel-item');
      if (!items.length) return;

      // limitar a quantidade exibida (por exemplo 12)
      const maxItems = 12;
      if (items.length > maxItems) {
        // remove extras
        for (let i = items.length - 1; i >= maxItems; i--) {
          const el = items[i];
          if (el && el.parentNode) el.parentNode.removeChild(el);
        }
      }

      const newItems = carouselList.querySelectorAll('.carousel-item');
      // garantir listeners limpos clonando
      newItems.forEach((item, idx) => {
        const clone = item.cloneNode(true);
        item.parentNode.replaceChild(clone, item);
      });

      const finalItems = carouselList.querySelectorAll('.carousel-item');
      finalItems.forEach((item, idx) => {
        item.addEventListener('click', () => {
          item.scrollIntoView({ behavior: 'smooth', inline: 'center', block: 'nearest' });
        });
      });
    });
  }

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



