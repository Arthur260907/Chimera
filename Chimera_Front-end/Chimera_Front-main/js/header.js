// Arquivo: js/header.js (Substitua todo o conteúdo)

const API_BASE_URL = 'http://localhost:5012'; // Garanta que esta porta esteja correta

document.addEventListener('DOMContentLoaded', () => {
    // Funções que rodam em todas as páginas
    updateHeaderUI();
    setupSearchForm(); 

    // --- LÓGICA DO DROPDOWN (AGORA CENTRALIZADA) ---
    // Isso faz o menu abrir e fechar em TODAS as páginas
    const profileIcon = document.querySelector('.profile-icon');
    const dropdownMenu = document.querySelector('.dropdown-menu');

    if (profileIcon && dropdownMenu) {
        profileIcon.addEventListener('click', function (event) {
            event.stopPropagation();
            dropdownMenu.classList.toggle('active');
        });

        // Fechar o menu se clicar fora dele
        document.addEventListener('click', function (event) {
            // Verifica se o clique foi fora do menu E fora do ícone
            if (!dropdownMenu.contains(event.target) && !profileIcon.contains(event.target)) {
                if (dropdownMenu.classList.contains('active')) {
                    dropdownMenu.classList.remove('active');
                }
            }
        });

        // Impede que cliques dentro do menu fechem o menu
        dropdownMenu.addEventListener('click', function (event) {
            event.stopPropagation();
        });
    }
    // --- FIM DA LÓGICA DO DROPDOWN ---
});

// --- Gerenciamento de Autenticação e Header ---

function updateHeaderUI() {
    const signInLink = document.getElementById('signin-link');
    const signOutLink = document.getElementById('signout-link');
    // Encontra os elementos LI pais para esconder/mostrar (corrige layout)
    const signInListItem = signInLink ? signInLink.closest('li') : null;
    const signOutListItem = signOutLink ? signOutLink.closest('li') : null;
    const profileUsernameElement = document.getElementById('profile-username');
    const loggedInUser = getLoggedInUser();

    if (loggedInUser && loggedInUser.username) {
        // --- USUÁRIO LOGADO ---
        if (signInListItem) signInListItem.style.display = 'none'; // Esconde LI "Sign in"
        if (signOutListItem) {
            signOutListItem.style.display = 'list-item'; // Mostra LI "Sign out"

            // Adiciona listener de logout
            const currentSignOutLink = document.getElementById('signout-link');
            if(currentSignOutLink && !currentSignOutLink.dataset.listenerAttached) { 
                currentSignOutLink.addEventListener('click', (e) => {
                    e.preventDefault(); 
                    logoutUser();
                });
                currentSignOutLink.dataset.listenerAttached = 'true';
            }
        }
        if (profileUsernameElement) profileUsernameElement.textContent = loggedInUser.username; // Define nome

    } else {
        // --- USUÁRIO DESLOGADO ---
        if (signInListItem) signInListItem.style.display = 'list-item'; // Mostra LI "Sign in"
        if (signOutListItem) signOutListItem.style.display = 'none'; // Esconde LI "Sign out"
        if (profileUsernameElement) profileUsernameElement.textContent = 'Visitante'; // Nome padrão
    }
}

function saveLoginData(userData) {
    if (userData && userData.username) {
        localStorage.setItem('chimeraUser', JSON.stringify(userData));
    } else {
        console.error("Tentativa de salvar dados de login inválidos:", userData);
    }
}

function getLoggedInUser() {
    const userData = localStorage.getItem('chimeraUser');
    try {
        return userData ? JSON.parse(userData) : null;
    } catch (e) {
        console.error("Erro ao parsear dados do usuário do localStorage", e);
        localStorage.removeItem('chimeraUser');
        return null;
    }
}

function logoutUser() {
    localStorage.removeItem('chimeraUser');
    console.log("Usuário deslogado.");

    const currentPath = window.location.pathname;
    
    // --- CORREÇÃO DE CAMINHO PARA LOGOUT ---
    let loginPath = 'Login.html'; // Padrão se já estiver em /html/
    
    // Se estiver na raiz (index.html), o caminho precisa incluir /html/
    if (currentPath.endsWith('/') || currentPath.endsWith('/index.html') || currentPath.includes('/index.html')) {
        loginPath = 'html/Login.html';
    }

    if (!currentPath.endsWith('Login.html')) {
        window.location.href = loginPath;
    } else {
        updateHeaderUI(); // Apenas atualiza a UI se já estiver no login
    }
}


// --- Funcionalidade de Pesquisa do Header ---

function debounce(func, delay) {
    let timeoutId;
    return function (...args) {
        clearTimeout(timeoutId);
        timeoutId = setTimeout(() => {
            func.apply(this, args);
        }, delay);
    };
}

function setupSearchForm() {
    const searchForm = document.getElementById('search-form');
    const searchInput = document.getElementById('search-input');
    const suggestionsDropdown = document.getElementById('search-suggestions');

    if (!searchForm || !searchInput || !suggestionsDropdown) {
        return; // Sai se os elementos não existirem na página
    }

    searchInput.addEventListener('blur', () => {
        setTimeout(() => {
            suggestionsDropdown.innerHTML = '';
            suggestionsDropdown.style.display = 'none';
        }, 200);
    });

    const fetchSuggestions = async (query) => {
        if (query.length < 2) {
            suggestionsDropdown.innerHTML = '';
            suggestionsDropdown.style.display = 'none';
            return;
        }
        try {
            // CORREÇÃO: Usar 'Search' (S maiúsculo) como no Controller
            const response = await fetch(`${API_BASE_URL}/api/Search?q=${encodeURIComponent(query)}`);
            if (!response.ok) throw new Error('Falha ao buscar sugestões');
            
            const results = await response.json();
            suggestionsDropdown.innerHTML = '';

            if (results && results.length > 0) {
                results.slice(0, 5).forEach(item => {
                    const div = document.createElement('div');
                    // CORREÇÃO: Usar 'Title' (T maiúsculo) como vem da API
                    div.textContent = item.Title; 
                    div.classList.add('suggestion-item');
                    div.addEventListener('mousedown', (e) => {
                        e.preventDefault();
                        searchInput.value = item.Title; // Usar 'Title'
                        suggestionsDropdown.innerHTML = '';
                        suggestionsDropdown.style.display = 'none';
                        
                        // CORREÇÃO: lógica de caminho para pesquisa.html
                        let searchPath = 'pesquisa.html'; // Padrão se já estiver em /html/
                        // Se NÃO estiver na pasta /html/ (ou seja, está na raiz index.html)
                        if (!window.location.pathname.includes('/html/')) { 
                             searchPath = 'html/pesquisa.html';
                        }
                        window.location.href = `${searchPath}?q=${encodeURIComponent(item.Title)}`;
                    });
                    suggestionsDropdown.appendChild(div);
                });
                suggestionsDropdown.style.display = 'block';
            } else {
                suggestionsDropdown.style.display = 'none';
            }
        } catch (error) {
            console.error('Erro buscando sugestões:', error);
            suggestionsDropdown.innerHTML = '';
            suggestionsDropdown.style.display = 'none';
        }
    };

    const debouncedFetchSuggestions = debounce(fetchSuggestions, 350);
    searchInput.addEventListener('input', () => {
        debouncedFetchSuggestions(searchInput.value.trim());
    });

    searchForm.addEventListener('submit', (e) => {
        // (Será interceptado pelo pesquisa.html se estivermos lá)
        e.preventDefault();
        const query = searchInput.value.trim();
        if (query) {
             // CORREÇÃO: lógica de caminho para pesquisa.html
            let searchPath = 'pesquisa.html';
             if (!window.location.pathname.includes('/html/')) {
                 searchPath = 'html/pesquisa.html';
             }
            window.location.href = `${searchPath}?q=${encodeURIComponent(query)}`;
        }
    });
}