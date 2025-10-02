// Exemplo de chamada para buscar dados do back-end
async function fetchMovies() {
    try {
        const response = await fetch('http://localhost:7282/api/OmdbMovies'); // Substitua pelo endpoint correto
        if (!response.ok) {
            throw new Error('Erro ao buscar filmes');
        }
        const movies = await response.json();
        console.log(movies); // Exibe os dados no console
        renderMovies(movies); // Função para renderizar os filmes no front-end
    } catch (error) {
        console.error('Erro:', error);
    }
}

// Exemplo de função para renderizar os filmes no DOM
function renderMovies(movies) {
    const movieList = document.getElementById('movie-list'); // Substitua pelo ID correto no HTML
    movieList.innerHTML = ''; // Limpa a lista antes de renderizar
    movies.forEach(movie => {
        const movieItem = document.createElement('li');
        movieItem.textContent = `${movie.title} (${movie.year})`;
        movieList.appendChild(movieItem);
    });
}

// Chama a função ao carregar a página
fetchMovies();