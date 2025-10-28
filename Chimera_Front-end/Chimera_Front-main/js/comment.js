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

    // Atualiza visualmente o ícone (azul claro se já for favorito)
    function updateHeart() {
        const favs = getFavorites();
        if (favs.some(f => f.id === movieId)) {
            favoriteBtn.style.color = '#1877F2';
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
            favoriteBtn.style.color = '#1877F2';
        }

        saveFavorites(favs);
    });

    // Funcionalidade para like, dislike e share
    const likeBtn = document.getElementById('like-btn');
    const dislikeBtn = document.getElementById('dislike-btn');
    const shareBtn = document.getElementById('share-btn');

    // Função para atualizar visual dos botões like/dislike
    function updateLikeDislike() {
        // Assume localStorage for simplicity, or API call
        const likes = JSON.parse(localStorage.getItem('likes')) || {};
        const dislikes = JSON.parse(localStorage.getItem('dislikes')) || {};

        if (likes[movieId]) {
            likeBtn.style.color = '#1877F2';
            dislikeBtn.style.color = '';
        } else if (dislikes[movieId]) {
            dislikeBtn.style.color = 'red';
            likeBtn.style.color = '';
        } else {
            likeBtn.style.color = '';
            dislikeBtn.style.color = '';
        }
    }

    // Evento para like
    likeBtn.addEventListener('click', async () => {
        const likes = JSON.parse(localStorage.getItem('likes')) || {};
        const dislikes = JSON.parse(localStorage.getItem('dislikes')) || {};

        if (likes[movieId]) {
            // Já liked → remove
            delete likes[movieId];
            likeBtn.style.color = '';
        } else {
            // Add like, remove dislike if exists
            likes[movieId] = true;
            delete dislikes[movieId];
            likeBtn.style.color = 'green';
            dislikeBtn.style.color = '';
        }

        localStorage.setItem('likes', JSON.stringify(likes));
        localStorage.setItem('dislikes', JSON.stringify(dislikes));

        // Send to API (assume endpoint exists)
        try {
            await fetch('http://localhost:5012/api/likes', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ movieId, type: 'like', username: 'Guest' })
            });
        } catch (error) {
            console.error('Erro ao enviar like:', error);
        }
    });

    // Evento para dislike
    dislikeBtn.addEventListener('click', async () => {
        const likes = JSON.parse(localStorage.getItem('likes')) || {};
        const dislikes = JSON.parse(localStorage.getItem('dislikes')) || {};

        if (dislikes[movieId]) {
            // Já disliked → remove
            delete dislikes[movieId];
            dislikeBtn.style.color = '';
        } else {
            // Add dislike, remove like if exists
            dislikes[movieId] = true;
            delete likes[movieId];
            dislikeBtn.style.color = 'red';
            likeBtn.style.color = '';
        }

        localStorage.setItem('likes', JSON.stringify(likes));
        localStorage.setItem('dislikes', JSON.stringify(dislikes));

        // Send to API
        try {
            await fetch('http://localhost:5012/api/likes', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ movieId, type: 'dislike', username: 'Guest' })
            });
        } catch (error) {
            console.error('Erro ao enviar dislike:', error);
        }
    });

    // Evento para share (copia o link do trailer/página)
    shareBtn.addEventListener('click', () => {
        const trailerLink = window.location.href; // Assume current page URL as trailer link
        navigator.clipboard.writeText(trailerLink).then(() => {
            alert('Link copiado para a área de transferência!');
        }).catch(err => {
            console.error('Erro ao copiar link:', err);
            alert('Erro ao copiar link.');
        });
    });

    updateLikeDislike();

    updateHeart();
});
