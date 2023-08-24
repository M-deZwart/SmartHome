const url = "http://NBNL865.rademaker.nl:5233/api/";
const currentUrl = window.location.href;

document.addEventListener('DOMContentLoaded', function() {
    async function getCurrentHumidity() {
        const humidityId = "currentHumidity";
        try {
            const response = await fetch(`${url}humidity/getCurrentHumidity`);
            if (response.ok) {
                const humidityDTO = await response.json();
                document.getElementById(humidityId).innerText = `${humidityDTO.percentage}%`;
            } else {
                document.getElementById(humidityId).innerText = "Percentage could not be retrieved";
            }
        } catch (error) {
            console.error(error);
            document.getElementById(humidityId).innerText = "Error while retrieving the percentage"
        }
    }
    
    async function getCurrentTemperature() {
        const temperatureId = "currentTemperature";
        try {
            const response = await fetch(`${url}temperature/getCurrentTemperature`);
            if (response.ok) {
                const temperatureDTO = await response.json();
                document.getElementById(temperatureId).innerText = `${temperatureDTO.celsius} °C`;
            } else {
                document.getElementById(temperatureId).innerText = "Celsius could not be retrieved";
            }
        } catch (error) {
            console.error(error);
            document.getElementById(temperatureId).innerText = "Error while retrieving the degree in celsius"
        }
    }

    if (currentUrl.includes("humidity")) {
        getCurrentHumidity();
    } else if (currentUrl.includes("temperature")) {
        getCurrentTemperature();
    }
});

document.addEventListener('DOMContentLoaded', function () {
    async function getDataAndDisplay(endpointName, valueName) {
        const getDataButton = document.getElementById(`getData${endpointName}`);
        const startDateInput = document.getElementById(`startDate${endpointName}`);
        const endDateInput = document.getElementById(`endDate${endpointName}`);
        const dataDiv = document.getElementById(`${endpointName.toLowerCase()}Data`);

        getDataButton.addEventListener('click', async function () {
            const startDate = startDateInput.value;
            const endDate = endDateInput.value;

            try {
                const response = await fetch(`${url}${endpointName}ByDateRange?startDate=${startDate}&endDate=${endDate}`);
                const data = await response.json();

                dataDiv.innerHTML = '';
                data.forEach(item => {
                    const itemElement = document.createElement('p');
                    itemElement.textContent = `Date: ${item.date}, ${valueName}: ${item[valueName.toLowerCase()]}%`;
                    dataDiv.appendChild(itemElement);
                });
            } catch (error) {
                console.error(`Error during ${endpointName.toLowerCase()} list retrieval: `, error);
            }
        });
    }
    if(currentUrl.includes("humidityDateRange")) {
        getDataAndDisplay("Humidity", "Percentage");
    } else if (currentUrl.includes("temperatureDateRange")) {
        getDataAndDisplay("Temperature", "Celsius");
    }
});

