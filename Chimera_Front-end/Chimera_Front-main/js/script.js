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
            counter = 0; // volta ao primeiro
        }
        carouselSlide.style.transition = 'transform 0.5s ease-in-out';
        carouselSlide.style.transform = 'translateX(' + (-getSlideWidth() * counter) + 'px)';
        updateDots();
    });

    prevBtn.addEventListener('click', () => {
        counter--;
        if (counter < 0) {
            counter = carouselImages.length - 1; // vai ao último
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
  // Seleciona todos os carrosseis multi
  const multiCarousels = document.querySelectorAll('.multi-carousel-container');
  
  multiCarousels.forEach(multiCarousel => {
    const carouselList = multiCarousel.querySelector('.carousel-list');
    const items = carouselList.querySelectorAll('.carousel-item');
    let currentIndex = 0;

    function scrollToItem(index) {
      // Loop infinito
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
