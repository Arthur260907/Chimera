// Função para carregar filmes/animes de uma categoria
async function loadCategory(genre, containerId) {
    try {
        // Corrigido: parâmetro 'filtro' conforme o controller espera
        const response = await fetch(`https://localhost:5000/api/catalogo?filtro=${genre}`);
        const data = await response.json();

        const container = document.querySelector(`#${containerId} .carousel-list`);
        container.innerHTML = ""; // limpa antes

        data.forEach(item => {
            const div = document.createElement("div");
            div.classList.add("carousel-item");
            div.innerHTML = `
                <img src="${item.posterUrl || item.Imagem}" alt="${item.title || item.Titulo}" onerror="this.src='imagens/placeholder.png'">
                <span>${item.title || item.Titulo}</span>
            `;
            container.appendChild(div);
        });
    } catch (error) {
        console.error("Erro ao carregar categoria:", genre, error);
    }
}

document.addEventListener("DOMContentLoaded", () => {
    loadCategory("comedy", "comedy-section");
    loadCategory("action", "action-section");
    loadCategory("adventure", "adventure-section");
});