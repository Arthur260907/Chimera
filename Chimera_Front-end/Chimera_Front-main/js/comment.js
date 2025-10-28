document.addEventListener('DOMContentLoaded', () => {
    // Pega o ID do filme da URL (?id=tt1234567)
    const urlParams = new URLSearchParams(window.location.search);
    const movieId = urlParams.get('id');

    // Elementos da interface
    const commentInput = document.querySelector('.related-search input[name="comment"]');
    const submitButton = document.querySelector('.related-search button');
    
    // Cria um container para mostrar os comentários
    let commentsContainer = document.createElement('div');
    commentsContainer.id = 'comments-list';
    commentsContainer.style.marginTop = '20px';
    document.querySelector('.related-search').after(commentsContainer);

    // URL base da API
    const API_COMMENTS_URL = 'http://localhost:5012/api/comments';

    // -------------------------------
    // Função: carregar comentários do back-end
    // -------------------------------
    async function loadComments() {
        if (!movieId) return;
        commentsContainer.innerHTML = "<p style='color:#aaa;'>Carregando comentários...</p>";

        try {
            const response = await fetch(`${API_COMMENTS_URL}/${movieId}`);
            if (!response.ok) throw new Error("Erro ao buscar comentários");

            const comments = await response.json();
            commentsContainer.innerHTML = '';

            if (comments.length === 0) {
                commentsContainer.innerHTML = "<p style='color:#aaa;'>Nenhum comentário ainda. Seja o primeiro!</p>";
            } else {
                comments.forEach(c => {
                    const commentDiv = document.createElement('div');
                    commentDiv.className = 'comment-item';
                    commentDiv.style.marginBottom = '15px';
                    commentDiv.style.background = '#1a1a1a';
                    commentDiv.style.padding = '10px';
                    commentDiv.style.borderRadius = '8px';

                    commentDiv.innerHTML = `
                        <strong>${c.username}</strong>
                        <span style="font-size:12px;color:#888;"> • ${new Date(c.createdAt).toLocaleString()}</span>
                        <p style="margin-top:5px;">${c.text}</p>
                    `;
                    commentsContainer.appendChild(commentDiv);
                });
            }
        } catch (error) {
            commentsContainer.innerHTML = `<p style="color:red;">Erro ao carregar comentários: ${error.message}</p>`;
            console.error(error);
        }
    }

    // -------------------------------
    // Função: enviar novo comentário
    // -------------------------------
    async function postComment() {
        const text = commentInput.value.trim();
        if (!text) return alert("Escreva algo antes de enviar!");

        const newComment = {
            movieId,
            username: 'Guest', // Depois você pode trocar para o usuário logado
            text
        };

        try {
            const response = await fetch(API_COMMENTS_URL, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(newComment)
            });

            if (response.ok) {
                commentInput.value = '';
                await loadComments(); // Atualiza lista
            } else {
                const errorText = await response.text();
                console.error("Erro ao postar comentário:", errorText);
                alert("Erro ao enviar comentário.");
            }
        } catch (error) {
            console.error(error);
            alert("Falha na conexão com o servidor.");
        }
    }

    // Evento de clique no botão de enviar
    submitButton.addEventListener('click', postComment);

    // Enter envia o comentário também
    commentInput.addEventListener('keypress', (e) => {
        if (e.key === 'Enter') {
            e.preventDefault();
            postComment();
        }
    });

    // Carrega comentários ao abrir a página
    loadComments();
});
document.addEventListener('DOMContentLoaded', () => {
    const favoriteBtn = document.getElementById('favorite-btn');
    const urlParams = new URLSearchParams(window.location.search);
    const movieId = urlParams.get('id');

    // Recupera os dados do filme na tela (já preenchidos)
    const title = document.getElementById('movie-title')?.textContent || "Unknown";
    const poster = document.getElementById('movie-poster')?.src || "";

    // Função: carrega lista de favoritos do localStorage
    function getFavorites() {
        return JSON.parse(localStorage.getItem('favorites')) || [];
    }

    // Função: salva lista de favoritos
    function saveFavorites(favs) {
        localStorage.setItem('favorites', JSON.stringify(favs));
    }

    // Atualiza visualmente o ícone (vermelho se já for favorito)
    function updateHeart() {
        const favs = getFavorites();
        if (favs.some(f => f.id === movieId)) {
            favoriteBtn.style.color = 'blue';
        } else {
            favoriteBtn.style.color = '';
        }
    }

    // Evento de clique no coração ❤️
    favoriteBtn.addEventListener('click', () => {
        const favs = getFavorites();
        const index = favs.findIndex(f => f.id === movieId);

        if (index >= 0) {
            // Já é favorito → remove
            favs.splice(index, 1);
            favoriteBtn.style.color = '';
        } else {
            // Adiciona novo favorito
            favs.push({ id: movieId, title, poster });
            favoriteBtn.style.color = 'blue';
        }

        saveFavorites(favs);
    });

    updateHeart();
});
