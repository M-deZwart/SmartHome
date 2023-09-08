document.addEventListener("DOMContentLoaded", function() {
    fetch("navbar.html")
    .then(response => response.text())
    .then(data => {
        const navbarContainer = document.querySelector("#navbar-container");
        navbarContainer.innerHTML = data;
    })
    .catch(error => console.error("Could not retrieve navbar: ", error));
})