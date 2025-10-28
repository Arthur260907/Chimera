// Arquivo: js/header.js (Substitua todo o conteúdo)

const API_BASE_URL = 'http://localhost:5012'; // Garanta que esta porta esteja correta

document.addEventListener('DOMContentLoaded', () => {
    updateHeaderUI();
    setupSearchForm(); // Configura o formulário de pesquisa do header

    // --- LÓGICA DO DROPDOWN ADICIONADA ---
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
    const signInListItem = signInLink ? signInLink.closest('li') : null;
    const signOutListItem = signOutLink ? signOutLink.closest('li') : null;
    const profileUsernameElement = document.getElementById('profile-username');
    const loggedInUser = getLoggedInUser();

    if (loggedInUser && loggedInUser.username) {
        // Usuário Logado
        if (signInListItem) signInListItem.style.display = 'none'; // Esconde LI do "Sign in"
        if (signOutListItem) {
            signOutListItem.style.display = 'list-item'; // Mostra LI do "Sign out"

            const currentSignOutLink = document.getElementById('signout-link');
            if (currentSignOutLink && !currentSignOutLink.dataset.listenerAttached) {
                currentSignOutLink.addEventListener('click', (e) => {
                    e.preventDefault();
                    logoutUser();
                });
                currentSignOutLink.dataset.listenerAttached = 'true';
            }
        }
        if (profileUsernameElement) profileUsernameElement.textContent = loggedInUser.username;

    } else {
        // Usuário Deslogado
        if (signInListItem) signInListItem.style.display = 'list-item'; // Mostra LI do "Sign in"
        if (signOutListItem) signOutListItem.style.display = 'none'; // Esconde LI do "Sign out"
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
    
    // CORREÇÃO DO CAMINHO DE REDIRECIONAMENTO:
    // Se já estiver na raiz (index.html), vai para 'html/Login.html'
    // Se estiver em outra página (ex: /html/pesquisa.html), vai para 'Login.html'
    
    let loginPath = 'Login.html'; // Padrão para páginas dentro de /html/
    
    if (currentPath.endsWith('/') || currentPath.endsWith('/index.html')) {
        loginPath = 'html/Login.html'; // Caminho se estiver na raiz
    }

    if (!currentPath.endsWith('Login.html')) {
        window.location.href = loginPath;
    } else {
        updateHeaderUI();
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
        return; 
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
                        let searchPath = 'pesquisa.html';
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
            suggestionsDropdown.style.display = 'none';
        }
    };

    const debouncedFetchSuggestions = debounce(fetchSuggestions, 350);
    searchInput.addEventListener('input', () => {
        debouncedFetchSuggestions(searchInput.value.trim());
    });

    searchForm.addEventListener('submit', (e) => {
        // Este listener agora é interceptado pelo 'pesquisa.html' se estiver naquela página
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