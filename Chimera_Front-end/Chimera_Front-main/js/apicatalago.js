// js/apicatalago.js
// API_BASE_URL já deve estar definido em script.js (certifique-se que script.js seja carregado ANTES deste)
// const API_BASE_URL = 'https://localhost:7196'; // Remova se já estiver em script.js

document.addEventListener('DOMContentLoaded', () => {
    loadAllCategories();
    loadMainCarousel(); // Chama o carregamento do carrossel principal
    setupFilterLinks();
});

/**
 * Função para buscar dados de uma categoria/pesquisa e popular o container.
 * @param {string} query O termo de busca (nome da categoria).
 * @param {HTMLElement} containerElement O elemento HTML onde os cards serão adicionados.
 * @param {boolean} [clearContainer=true] Se deve limpar o container antes de adicionar os cards.
 */
async function fetchAndPopulateCategory(query, containerElement, clearContainer = true) {
    if (!containerElement) return;

    const currentFilter = sessionStorage.getItem('mediaFilter'); // Pega o filtro atual ('movie', 'series' ou null)
    let searchUrl = `${API_BASE_URL}/api/search?q=${encodeURIComponent(query)}`;

    if (currentFilter) {
        searchUrl += `&type=${currentFilter}`; // Adiciona o filtro de tipo se existir
    }

    try {
        const response = await fetch(searchUrl);
        if (!response.ok) throw new Error(`Erro ao buscar ${query}`);
        const data = await response.json();

        if (clearContainer) {
            containerElement.innerHTML = ''; // Limpa o container antes de adicionar
        }

        if (data && data.length > 0) {
            data.forEach(item => {
                const card = createMovieCard(item); // Cria o HTML do card
                if (card) {
                    containerElement.appendChild(card);
                }
            });
        } else if (clearContainer) {
             // Só mostra a mensagem se estivermos limpando o container e não for o carrossel principal
            containerElement.innerHTML = '<p>Nenhum item encontrado nesta categoria.</p>';
        }
    } catch (error) {
        console.error(`Falha ao carregar categoria ${query}:`, error);
        if (clearContainer) {
            containerElement.innerHTML = '<p>Erro ao carregar itens.</p>'; // Mensagem de erro
        }
    }
}

/**
 * Função para criar o HTML de um card de filme/série.
 * (Mantida do código original, garantindo a compatibilidade com a API /api/search)
 * @param {object} item Objeto do item (filme ou série) com campos como imdbId, title, poster, year.
 * @returns {HTMLLIElement|null} O elemento li contendo o card do filme, ou null.
 */
function createMovieCard(item) {
if (!item || !item.imdbID) return null; // Validação básica

    const listItem = document.createElement('li');

    const movieCard = document.createElement('div');
    movieCard.className = 'movie-card';

    const anchor = document.createElement('a');
    // Link para a página de detalhes (filmeSerie.html), passando o ID do IMDB
   anchor.href = `html/filmeSerie.html?id=${item.imdbID}`;

    const figure = document.createElement('figure');
    figure.className = 'card-banner';

    const img = document.createElement('img');
    // Usa o pôster da API. Se não tiver, pode usar uma imagem padrão.
    img.src = (item.Poster && item.Poster !== 'N/A') ? item.Poster : '../imagens/image_placeholder.png'; // Crie uma imagem placeholder
    img.alt = item.Title;
    img.loading = 'lazy'; // Adiciona carregamento lazy

    figure.appendChild(img);
    anchor.appendChild(figure);

    const titleWrapper = document.createElement('div');
    titleWrapper.className = 'title-wrapper';
    const titleLink = document.createElement('a');
    titleLink.href = anchor.href; // Mesmo link
    const titleH3 = document.createElement('h3');
    titleH3.className = 'card-title';
    titleH3.textContent = item.Title;
    titleLink.appendChild(titleH3);
    titleWrapper.appendChild(titleLink);

    const dateSpan = document.createElement('span');
    dateSpan.className = 'date';
    dateSpan.textContent = item.Year || 'N/A'; // Mostra o ano
    titleWrapper.appendChild(dateSpan);

    anchor.appendChild(titleWrapper);
    movieCard.appendChild(anchor);
    listItem.appendChild(movieCard);

    return listItem;
}

/**
 * Função para carregar todas as categorias na página.
 * Agora utiliza o .category-title para buscar o termo, respeitando o filtro de mídia.
 */
function loadAllCategories() {
    const categorySections = document.querySelectorAll('.movies-list-section'); // Seleciona todas as seções

    categorySections.forEach(section => {
        const titleElement = section.querySelector('.category-title');
        // Acha a lista (carrossel ou grid), priorizando .carousel-list ou .grid-list
        const listElement = section.querySelector('.carousel-list, .grid-list');

        if (titleElement && listElement) {
            const categoryName = titleElement.textContent.trim(); // Pega o nome da categoria do H2
            fetchAndPopulateCategory(categoryName, listElement); // Busca e popula
        }
    });
}

/**
 * Função para popular o carrossel principal (Hero Section), usando uma busca genérica.
 * Assumimos que o #main-carousel é preenchido com cards grandes.
 */
async function loadMainCarousel() {
    const mainCarousel = document.getElementById('main-carousel');
    if (!mainCarousel) return;
    
    // Limpa o carrossel antes de preencher
    mainCarousel.innerHTML = '';

    const currentFilter = sessionStorage.getItem('mediaFilter') || ''; // 'movie', 'series' ou vazio
    const searchUrl = `${API_BASE_URL}/api/search?q=popular&type=${currentFilter}&count=6`; // Busca por 'popular' ou algo genérico

    try {
        const response = await fetch(searchUrl);
        if (!response.ok) throw new Error('Erro ao buscar carrossel principal');
        const data = await response.json();

        if (data && data.length > 0) {
            data.slice(0, 6).forEach(item => { // Limita a 6 itens, como no bloco antigo
                const card = createMainCarouselCard(item); // Usa uma função de card adaptada para o layout principal
                if (card) {
                    mainCarousel.appendChild(card);
                }
            });
        } else {
             mainCarousel.innerHTML = '<p>Nenhum item encontrado para o carrossel principal.</p>';
        }

        // Você pode disparar um evento aqui para inicializar o slider/carrossel JS se necessário
        document.dispatchEvent(new CustomEvent('mainCarousel:loaded'));

    } catch (error) {
        console.error('Falha ao carregar carrossel principal:', error);
        mainCarousel.innerHTML = '<p>Erro ao carregar itens principais.</p>';
    }
}

/**
 * Função para criar o HTML de um card para o carrossel principal (Hero).
 * Deve ser ajustado ao HTML esperado para esta seção.
 * @param {object} item Objeto do item (filme ou série).
 * @returns {HTMLElement|null} O elemento div do carrossel principal.
 */
function createMainCarouselCard(item) {
    if (!item || !item.imdbId) return null;

    const wrapper = document.createElement('div');
    wrapper.className = 'carousel-item main'; // Classe esperada

    const anchor = document.createElement('a');
    anchor.href = `html/filmeSerie.html?id=${item.imdbID}`;

    const img = document.createElement('img');
    img.src = (item.Poster && item.Poster !== 'N/A') ? item.Poster : '../imagens/image_placeholder.png';
    img.alt = item.Title;
    img.loading = 'lazy'; 

    anchor.appendChild(img);
    wrapper.appendChild(anchor);
    
    // Opcional: Adicionar Título e Meta Info sobre a imagem principal, se o layout permitir
    /*
    const contentDiv = document.createElement('div');
    contentDiv.className = 'main-carousel-content';
    contentDiv.innerHTML = `<h3>${item.title}</h3><p>${item.year}</p>`;
    wrapper.appendChild(contentDiv);
    */

    return wrapper;
}


// Configura os links de filtro no header
function setupFilterLinks() {
    const moviesLink = document.getElementById('filter-movies'); // Dê ID 'filter-movies' ao span/h5 Movies
    const seriesLink = document.getElementById('filter-series'); // Dê ID 'filter-series' ao span/h5 Series
    const homeLink = document.getElementById('filter-home');     // Dê ID 'filter-home' ao span/a Home

    const links = [moviesLink, seriesLink, homeLink].filter(l => l);

    const handleFilterClick = (filterType, linkToActivate) => (e) => {
        if (e && linkToActivate !== homeLink) e.preventDefault();
        
        if (filterType === null) {
            sessionStorage.removeItem('mediaFilter');
        } else {
            sessionStorage.setItem('mediaFilter', filterType);
        }

        // Recarrega todos os dados com o novo filtro
        loadAllCategories();
        loadMainCarousel(); 
        
        setActiveFilterLink(linkToActivate, links.filter(l => l !== linkToActivate));
        // Se for o link Home, permitir a navegação (se for um <a> com href)
        if (linkToActivate === homeLink && e && homeLink.tagName.toLowerCase() === 'a') return; 
    };

    if (moviesLink) {
        moviesLink.addEventListener('click', handleFilterClick('movie', moviesLink));
    }

    if (seriesLink) {
        seriesLink.addEventListener('click', handleFilterClick('series', seriesLink));
    }

    if (homeLink) {
         // Se o Home link for um <a>, removemos o preventDefault para permitir a navegação, mas limpamos o filtro
         homeLink.addEventListener('click', (e) => {
             sessionStorage.removeItem('mediaFilter'); 
             setActiveFilterLink(homeLink, links.filter(l => l !== homeLink));
             // Se for um botão ou span, recarregar. Se for um link <a> com href, apenas limpa e navega.
             if (homeLink.tagName.toLowerCase() !== 'a') {
                 e.preventDefault();
                 loadAllCategories(); 
                 loadMainCarousel();
             }
         });
    }

     // Define o link ativo visualmente ao carregar a página
     const currentFilter = sessionStorage.getItem('mediaFilter');
     if (currentFilter === 'movie' && moviesLink) {
          setActiveFilterLink(moviesLink, links.filter(l => l !== moviesLink));
     } else if (currentFilter === 'series' && seriesLink) {
          setActiveFilterLink(seriesLink, links.filter(l => l !== seriesLink));
     } else if (homeLink) {
         setActiveFilterLink(homeLink, links.filter(l => l !== homeLink));
     }
}

// Função auxiliar para destacar o link de filtro ativo (opcional)
function setActiveFilterLink(activeLink, otherLinks) {
     if (activeLink) {
          activeLink.style.fontWeight = 'bold'; // Exemplo de destaque
          activeLink.style.textDecoration = 'underline';
     }
     otherLinks.forEach(link => {
          if (link) {
               link.style.fontWeight = 'normal';
               link.style.textDecoration = 'none';
          }
     });
}