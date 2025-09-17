async function loadCategory(genre, containerId) {
    try {
        // Use the 'filter' parameter as expected by the controller
        const response = await fetch(`https://localhost:7286/api/catalogo?filtro=${genre}`);
        const data = await response.json();

        const container = document.querySelector(`#${containerId} .carousel-list`);
        container.innerHTML = ""; // clear before

        // Adjust the field names according to the API JSON
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
        console.error("Error loading category:", genre, error);
    }
}

document.addEventListener("DOMContentLoaded", () => {
    loadCategory("comedy", "comedy-section");
    loadCategory("action", "action-section");
    loadCategory("adventure", "adventure-section");
});