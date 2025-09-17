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

document.addEventListener('DOMContentLoaded', () => {
    const carouselSlide = document.querySelector('.carousel-slide');
    const carouselImages = document.querySelectorAll('.carousel-slide img');
    const prevBtn = document.getElementById('prevBtn');
    const nextBtn = document.getElementById('nextBtn');
    const dotsContainer = document.querySelector('.carousel-dots');
    const dots = document.querySelectorAll('.dot');

    let counter = 0;

    function getSlideWidth() {
        return carouselImages[0].clientWidth;
    }

    carouselSlide.style.transform = 'translateX(' + (-getSlideWidth() * counter) + 'px)';

    nextBtn.addEventListener('click', () => {
        counter++;
        if (counter > carouselImages.length - 1) {
            counter = 0; // goes back to the first
        }
        carouselSlide.style.transition = 'transform 0.5s ease-in-out';
        carouselSlide.style.transform = 'translateX(' + (-getSlideWidth() * counter) + 'px)';
        updateDots();
    });

    prevBtn.addEventListener('click', () => {
        counter--;
        if (counter < 0) {
            counter = carouselImages.length - 1; // goes to the last
        }
        carouselSlide.style.transition = 'transform 0.5s ease-in-out';
        carouselSlide.style.transform = 'translateX(' + (-getSlideWidth() * counter) + 'px)';
        updateDots();
    });
    dots.forEach(dot => {
        dot.addEventListener('click', (e) => {
            const slideIndex = parseInt(e.target.dataset.slide);
            counter = slideIndex;
            carouselSlide.style.transition = 'transform 0.5s ease-in-out';
            carouselSlide.style.transform = 'translateX(' + (-getSlideWidth() * counter) + 'px)';
            updateDots();
        });
    });
    function updateDots() {
        dots.forEach((dot, index) => {
            if (index === counter) {
                dot.classList.add('active');
            } else {
                dot.classList.remove('active');
            }
        });
    }
    updateDots();
});

document.addEventListener('DOMContentLoaded', () => {
  // Selects all multi carousels
  const multiCarousels = document.querySelectorAll('.multi-carousel-container');
  
  multiCarousels.forEach(multiCarousel => {
    const carouselList = multiCarousel.querySelector('.carousel-list');
    const items = carouselList.querySelectorAll('.carousel-item');
    let currentIndex = 0;

    function scrollToItem(index) {
      // Infinite loop
      if (index < 0) {
        index = items.length - 1;
      }
      if (index > items.length - 1) {
        index = 0;
      }
      currentIndex = index;
      items[currentIndex].scrollIntoView({ behavior: 'smooth', inline: 'center', block: 'nearest' });
    }

    items.forEach((item, idx) => {
      item.addEventListener('click', () => {
        // If you click on the last, go back to the first; if you click on the first, go to the last
        // Se clicar no último, volta ao primeiro; se clicar no primeiro, vai ao último
        if (idx === items.length - 1) {
          scrollToItem(0);
        } else if (idx === 0) {
          scrollToItem(items.length - 1);
        } else {
          scrollToItem(idx);
        }
      });
    });
  });
});

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