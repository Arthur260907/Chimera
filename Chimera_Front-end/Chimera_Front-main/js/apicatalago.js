// Função para carregar filmes/animes de uma categoria
async function loadCategory(genre, containerId) {
    try {
        const response = await fetch(`https://localhost:5001/api/catalog?genre=${genre}`);
        const data = await response.json();

        const container = document.querySelector(`#${containerId} .carousel-list`);
        container.innerHTML = ""; // limpa antes

        data.forEach(item => {
            const li = document.createElement("li");
            li.classList.add("carousel-item");
            li.innerHTML = `
                <img src="${item.posterUrl}" alt="${item.title}">
            `;
            container.appendChild(li);
        });
    } catch (error) {
        console.error("Erro ao carregar categoria:", genre, error);
    }
}

// Carregar as categorias quando abrir a página
document.addEventListener("DOMContentLoaded", () => {
    loadCategory("comedy", "comedy-section");
    loadCategory("action", "action-section");
    loadCategory("adventure", "adventure-section");
});