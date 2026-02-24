const serviceBadge = document.getElementById("serviceBadge");
const emptyState = document.getElementById("emptyState");
const tableWrapper = document.getElementById("tableWrapper");
const herdTableBody = document.getElementById("herdTableBody");
const alpakaForm = document.getElementById("alpakaForm");
const formAlert = document.getElementById("formAlert");

const apiBaseUrl = "/api/alpakas";

function renderHerd(items) {
    herdTableBody.innerHTML = "";

    if (!items.length) {
        emptyState.classList.remove("d-none");
        tableWrapper.classList.add("d-none");
        return;
    }

    emptyState.classList.add("d-none");
    tableWrapper.classList.remove("d-none");

    for (const alpaka of items) {
        const row = document.createElement("tr");
        row.innerHTML = `
            <td>${alpaka.id}</td>
            <td>${alpaka.name}</td>
            <td>${alpaka.age}</td>
            <td>${alpaka.color}</td>
        `;
        herdTableBody.appendChild(row);
    }
}

function showAlert(type, text) {
    formAlert.className = `alert alert-${type} mt-3`;
    formAlert.textContent = text;
    formAlert.classList.remove("d-none");
}

async function loadHerd() {
    const response = await fetch(apiBaseUrl, { headers: { Accept: "application/json" } });
    if (!response.ok) {
        throw new Error("Failed to load alpaka herd.");
    }

    const payload = await response.json();
    serviceBadge.textContent = payload.serviceName ?? "Alpaka API";
    renderHerd(payload.items ?? []);
}

alpakaForm.addEventListener("submit", async (event) => {
    event.preventDefault();
    formAlert.classList.add("d-none");

    const formData = new FormData(alpakaForm);
    const body = {
        name: formData.get("name")?.toString().trim() ?? "",
        age: Number(formData.get("age")),
        color: formData.get("color")?.toString().trim() ?? ""
    };

    try {
        const response = await fetch(apiBaseUrl, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(body)
        });

        if (!response.ok) {
            const errorPayload = await response.json().catch(() => ({}));
            const message = errorPayload.message ?? "Could not add alpaka.";
            showAlert("danger", message);
            return;
        }

        alpakaForm.reset();
        showAlert("success", "Alpaka added to the herd.");
        await loadHerd();
    }
    catch (error) {
        showAlert("danger", error.message ?? "Unknown error while calling API.");
    }
});

loadHerd().catch((error) => {
    serviceBadge.textContent = "API unavailable";
    showAlert("danger", error.message ?? "Could not contact API.");
});
