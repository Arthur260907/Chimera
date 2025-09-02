async function loadCategory(genre, containerId) {
    try {
        // Use o parâmetro 'filtro' conforme o controller espera
        const response = await fetch(`https://localhost:7286 /api/catalogo?filtro=${genre}`);
        const data = await response.json();

        const container = document.querySelector(`#${containerId} .carousel-list`);
        container.innerHTML = ""; // limpa antes

        // Ajuste os nomes dos campos conforme o JSON da API
        data.forEach(item => {
            const div = document.createElement("div");
            div.classList.add("carousel-item");
            div.innerHTML = `
                <img src="${item.Imagem}" alt="${item.Titulo}" onerror="this.src='imagens/placeholder.png'">
                <span>${item.Titulo}</span>
            `;
            container.appendChild(div);
        });
    } catch (error) {
        console.error("Erro ao carregar categoria:", genre, error);
    }
}

document.addEventListener("DOMContentLoaded", () => {
    loadCategory("comedia", "comedy-section");
    loadCategory("acao", "action-section");
    loadCategory("aventura", "adventure-section");
});