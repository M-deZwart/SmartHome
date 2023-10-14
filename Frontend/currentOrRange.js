document.addEventListener('DOMContentLoaded', () => {
    //CurrentOrRange
    document.getElementById("currentData").addEventListener("click", async () => {
        try {
            window.location.href = "current.html";
        }
        catch (error) {
            console.error(error);
        }
    });

    document.getElementById("dateRange").addEventListener("click", () => {
        try {
            window.location.href = "dateRange.html";
        }
        catch (error) {
            console.error(error);
        }
    });
});
