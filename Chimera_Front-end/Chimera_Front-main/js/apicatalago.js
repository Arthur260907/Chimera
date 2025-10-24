/*
  Busca filmes da API backend (/api/OmdbMovies) e popula:
   - #main-carousel (primeiros 4 posters)
   - cada .carousel-list dentro de .multi-carousel-container
  Ajuste API_PORT se necessário.
*/
(() => {
  // FORÇAR backend rodando em http://localhost:5012
  const API_BASE = window.API_BASE || 'http://localhost:5012';
  const FALLBACK_PLACEHOLDER = ''; // vazio: não usar imagens locais como fonte principal

  function proxyPosterUrl(posterUrl) {
    if (!posterUrl) return '';
    return `${API_BASE}/api/OmdbMovies/poster?url=${encodeURIComponent(posterUrl)}`;
  }

  async function fetchMovies() {
    try {
      const res = await fetch(`${API_BASE}/api/OmdbMovies`);
      if (!res.ok) throw new Error(`API error ${res.status}`);
      const movies = await res.json();
      return Array.isArray(movies) ? movies : [];
    } catch (err) {
      console.error('Erro ao buscar filmes da API:', err);
      return [];
    }
  }

  function shuffle(array) {
    for (let i = array.length - 1; i > 0; i--) {
      const j = Math.floor(Math.random() * (i + 1));
      [array[i], array[j]] = [array[j], array[i]];
    }
    return array;
  }

  function extractUniquePosters(movies) {
    const set = new Set();
    const posters = [];
    for (const m of movies) {
      const p = (m && m.Poster) ? m.Poster.trim() : '';
      if (!p || p.toLowerCase() === 'n/a') continue;
      if (!set.has(p)) {
        set.add(p);
        posters.push({ poster: p, title: m.Title || '' });
      }
    }
    return posters;
  }

  function createImgElement(src, alt = '') {
    const img = document.createElement('img');
    img.src = src ? proxyPosterUrl(src) : FALLBACK_PLACEHOLDER;
    img.alt = alt;
    img.loading = 'lazy';
    return img;
  }

  function populateFromPosters(posters) {
    shuffle(posters);

    const mainCount = Math.min(6, posters.length);
    const sectionCount = 12;

    const working = posters.slice(); // copia
    const mainPosters = working.splice(0, mainCount);
    const comedyPosters = working.splice(0, sectionCount);
    const actionPosters = working.splice(0, sectionCount);
    const adventurePosters = working.splice(0, sectionCount);

    const mainSlide = document.querySelector('#main-carousel');
    if (mainSlide) {
      mainSlide.innerHTML = '';
      mainPosters.forEach(p => {
        const wrapper = document.createElement('div');
        wrapper.className = 'carousel-item main';
        wrapper.appendChild(createImgElement(p.poster, p.title));
        mainSlide.appendChild(wrapper);
      });
    }

    function fillSection(sectionId, listItems) {
      const container = document.querySelector(sectionId);
      if (!container) return;
      // create markup expected by script.js
      container.innerHTML = `<div class="carousel-list" data-section="${sectionId.replace('#','')}"></div>`;
      const list = container.querySelector('.carousel-list');
      listItems.forEach(p => {
        const item = document.createElement('div');
        item.className = 'carousel-item';
        item.appendChild(createImgElement(p.poster, p.title));
        list.appendChild(item);
      });
    }

    fillSection('#comedy-section', comedyPosters);
    fillSection('#action-section', actionPosters);
    fillSection('#adventure-section', adventurePosters);
  }

  function waitForImagesLoad(selectorList = ['#main-carousel img', '.carousel-list img'], timeout = 5000) {
    const selectors = selectorList.join(', ');
    const imgs = Array.from(document.querySelectorAll(selectors));
    if (imgs.length === 0) return Promise.resolve();
    const promises = imgs.map(img => new Promise(resolve => {
      if (img.complete) return resolve();
      const done = () => {
        img.removeEventListener('load', done);
        img.removeEventListener('error', done);
        resolve();
      };
      img.addEventListener('load', done);
      img.addEventListener('error', done);
    }));
    return Promise.race([Promise.all(promises), new Promise(resolve => setTimeout(resolve, timeout))]);
  }

  async function initCatalog() {
    const movies = await fetchMovies();
    const posters = extractUniquePosters(movies);
    if (posters.length === 0) console.warn('Nenhum poster válido retornado pela API.');
    populateFromPosters(posters);
    await waitForImagesLoad();
    window.__CHIMERA_MOVIES = movies;
    document.dispatchEvent(new CustomEvent('catalog:loaded', { detail: { movies } }));
  }

  document.addEventListener('DOMContentLoaded', initCatalog);
})();