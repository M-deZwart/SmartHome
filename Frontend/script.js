async function getCurrentHumidity() {
    try {
        const response = await fetch("http://NBNL865.rademaker.nl:5233/api/humidity/getCurrentHumidity");
        if (response.ok) {
            const humidityDTO = await response.json();
            document.getElementById("currentHumidity").innerText = humidityDTO.percentage + "%";           
        } else {
            document.getElementById("currentHumidity").innerText = "Percentage could not be retrieved";
        }
    } catch (error) {
        console.error(error);
        document.getElementById("currentHumidity").innerText = "Error while retrieving the percentage"
    }
}

async function getCurrentTemperature() {
    try {
        const response = await fetch("http://NBNL865.rademaker.nl:5233/api/temperature/getCurrentTemperature");
        if (response.ok) {
            const temperatureDTO = await response.json();
            document.getElementById("currentTemperature").innerText = temperatureDTO.celsius + " °C";           
        } else {
            document.getElementById("currentTemperature").innerText = "Celsius could not be retrieved";
        }
    } catch (error) {
        console.error(error);
        document.getElementById("currentTemperature").innerText = "Error while retrieving the degree in celsius"
    }
}

window.onload = getCurrentHumidity;
window.onload = getCurrentTemperature;