const API_BASE_URL = 'http://localhost:5012'; // <-- AJUSTE A PORTA SE NECESSÁRIO!

document.addEventListener('DOMContentLoaded', () => {
    updateHeaderUI();
    setupSearchForm(); // Configura o formulário de pesquisa do header
});

// --- Gerenciamento de Autenticação e Header ---

function updateHeaderUI() {
    const signInLink = document.getElementById('signin-link');
    const signOutLink = document.getElementById('signout-link');
    // Encontra os elementos LI pais para esconder/mostrar
    const signInListItem = signInLink ? signInLink.closest('li') : null;
    const signOutListItem = signOutLink ? signOutLink.closest('li') : null;
    const profileUsernameElement = document.getElementById('profile-username');
    const loggedInUser = getLoggedInUser();

    if (loggedInUser && loggedInUser.username) {
        // Usuário Logado
        if (signInListItem) signInListItem.style.display = 'none'; // Esconde LI do "Sign in"
        if (signOutListItem) {
            signOutListItem.style.display = 'list-item'; // Mostra LI do "Sign out"

            // Garante que o listener só é adicionado uma vez ou é atualizado
            const currentSignOutLink = document.getElementById('signout-link'); // Pega o link atual
            if(currentSignOutLink && !currentSignOutLink.dataset.listenerAttached) { // Verifica se já tem listener
                currentSignOutLink.addEventListener('click', (e) => {
                    e.preventDefault(); 
                    logoutUser();
                });
                currentSignOutLink.dataset.listenerAttached = 'true'; // Marca que adicionou listener
            }
        }
        if (profileUsernameElement) profileUsernameElement.textContent = loggedInUser.username; 

    } else {
        // Usuário Deslogado
        if (signInListItem) signInListItem.style.display = 'list-item'; // Mostra LI do "Sign in"
        if (signOutListItem) signOutListItem.style.display = 'none'; // Esconde LI do "Sign out"
        if (profileUsernameElement) profileUsernameElement.textContent = 'Visitante'; 
    }
}

function saveLoginData(userData) {
    // userData deve ser um objeto { username: 'nome', token: 'seu_token' }
    // A API de login precisa retornar o username e talvez um token
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
        localStorage.removeItem('chimeraUser'); // Limpa dados inválidos
        return null;
    }
}

function logoutUser() {
    localStorage.removeItem('chimeraUser');
    console.log("Usuário deslogado.");

    // Verifica se JÁ está na página de Login para evitar loop
    // (Considera caminhos que terminam com Login.html ou /Login.html)
    const currentPath = window.location.pathname;
    if (!currentPath.endsWith('Login.html') && !currentPath.endsWith('/Login.html')) {
        // SEMPRE redireciona para o caminho relativo a partir da raiz esperada
        window.location.href = 'html/Login.html'; 
    } else {
        // Se já está na página de login, apenas atualiza a interface
        updateHeaderUI(); 
    }
}

// --- Funcionalidade de Pesquisa do Header ---

// Função Debounce para evitar chamadas excessivas à API
function debounce(func, delay) {
    let timeoutId;
    return function(...args) {
        clearTimeout(timeoutId);
        timeoutId = setTimeout(() => {
            func.apply(this, args);
        }, delay);
    };
}

function setupSearchForm() {
    const searchForm = document.getElementById('search-form');
    const searchInput = document.getElementById('search-input');
    const suggestionsDropdown = document.getElementById('search-suggestions'); // Crie este elemento no HTML se não existir

    if (!searchForm || !searchInput || !suggestionsDropdown) {
        // console.warn("Elementos do formulário de pesquisa não encontrados.");
        return; // Sai se os elementos não existirem na página atual
    }

     // Limpa sugestões quando o input perde o foco (com um pequeno delay)
     searchInput.addEventListener('blur', () => {
        setTimeout(() => {
            suggestionsDropdown.innerHTML = '';
            suggestionsDropdown.style.display = 'none';
        }, 200); // Delay para permitir clicar na sugestão
    });


    // Auto-sugestão
    const fetchSuggestions = async (query) => {
        if (query.length < 2) { // Não busca com menos de 2 caracteres
            suggestionsDropdown.innerHTML = '';
            suggestionsDropdown.style.display = 'none';
            return;
        }

        try {
            const response = await fetch(`${API_BASE_URL}/api/search?q=${encodeURIComponent(query)}`);
            if (!response.ok) throw new Error('Falha ao buscar sugestões');
            const results = await response.json();

            suggestionsDropdown.innerHTML = ''; // Limpa sugestões anteriores
            if (results && results.length > 0) {
                results.slice(0, 5).forEach(item => { // Mostra no máximo 5 sugestões
                    const div = document.createElement('div');
                    div.textContent = item.title;
                    div.classList.add('suggestion-item'); // Adicione estilo CSS para .suggestion-item
                    div.addEventListener('mousedown', (e) => { // 'mousedown' ocorre antes do 'blur' do input
                         e.preventDefault(); // Evita que o input perca foco imediatamente
                        searchInput.value = item.title; // Preenche o input com o título clicado
                        suggestionsDropdown.innerHTML = '';
                        suggestionsDropdown.style.display = 'none';
                         // Opcional: Submeter o form automaticamente ao clicar na sugestão
                         searchForm.dispatchEvent(new Event('submit', { cancelable: true }));
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

    // Aplica debounce à função de busca
    const debouncedFetchSuggestions = debounce(fetchSuggestions, 350); // Delay de 350ms

    searchInput.addEventListener('input', () => {
        debouncedFetchSuggestions(searchInput.value.trim());
    });

    // Ação de Submissão
    searchForm.addEventListener('submit', (e) => {
        e.preventDefault(); // Impede o envio padrão
        const query = searchInput.value.trim();
        if (query) {
            // Redireciona para a página de pesquisa com o parâmetro
            window.location.href = `html/pesquisa.html?q=${encodeURIComponent(query)}`;
        }
    });
}

// Crie um elemento <div id="search-suggestions"></div> posicionado
// abaixo do input de pesquisa no seu header HTML. Estilize-o com CSS
// (position: absolute, background, border, etc.) e inicialmente hidden.
// Exemplo CSS:
/*
#search-suggestions {
    display: none;
    position: absolute;
    background-color: white;
    border: 1px solid #ccc;
    width: calc(100% - 2px); Ajuste conforme o input
    max-height: 200px;
    overflow-y: auto;
    z-index: 1000;
    margin-top: -1px; Para colar abaixo do input
}
.suggestion-item {
    padding: 8px 12px;
    cursor: pointer;
}
.suggestion-item:hover {
    background-color: #f0f0f0;
}
*/