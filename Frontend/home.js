let selectedSensor = "";

document.addEventListener('DOMContentLoaded', () => {
    // Home
    document.getElementById("livingRoom").addEventListener("click", () => {
        selectedSensor = "LivingRoom";
        localStorage.setItem("selectedSensor", selectedSensor);
        navigateToCurrentOrRangePage();
    });

    document.getElementById("bedroom").addEventListener("click", () => {
        selectedSensor = "Bedroom";
        localStorage.setItem("selectedSensor", selectedSensor);
        navigateToCurrentOrRangePage();
    });

    document.getElementById("workspace").addEventListener("click", () => {
        selectedSensor = "WorkSpace";
        localStorage.setItem("selectedSensor", selectedSensor);
        navigateToCurrentOrRangePage();
    });

    
    function navigateToCurrentOrRangePage() {
        window.location.href = "currentOrRange.html";
    }
});