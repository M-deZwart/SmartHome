const url = "http://NBNL865.rademaker.nl:5233/api/";
const currentUrl = window.location.href;

document.addEventListener('DOMContentLoaded', function () {
    // home
    let button1 = document.getElementById("livingRoom").addEventListener("click", function () {
        navigateToCurrentOrRangePage("LivingRoom");
    });

    let button2 = document.getElementById("bedroom").addEventListener("click", function () {
        navigateToCurrentOrRangePage("Bedroom");
    });

    let button3 = document.getElementById("workspace").addEventListener("click", function () {
        navigateToCurrentOrRangePage("WorkSpace");
    });

    console.log(button1);
    console.log(button2);
    console.log(button3);
    
    async function navigateToCurrentOrRangePage(sensor) {
        const currentOrRangePageName = "currentOrRange.html";
        // add selected sensor as queryparameter to URL
        const currentOrRangePageURL = `${currentOrRangePageName}?sensor=${sensor}`;
        // navigate to new page
        window.location.href = currentOrRangePageURL;
    }

    // currentOrRange
    const urlParams = new URLSearchParams(window.location.search);
    const selectedSensor = urlParams.get("sensor");

    async function getDataForSensor(sensor) {
        const humidityId = "currentHumidity";
        const temperatureId = "currentTemperature";

        try {
            // get current humidity for selected sensor
            const humidityResponse = await fetch(`${url}humidity/getCurrentHumidity/${sensor}`);
            if (humidityResponse.ok) {
                const humidityDTO = await humidityResponse.json();
                document.getElementById(humidityId).innerText = `${humidityDTO.percentage}%`;
            } else {
                document.getElementById(humidityId).innerText = "Percentage could not be retrieved";
            }
            // get current temperature for selected sensor
            const temperatureResponse = await fetch(`${url}temperature/getCurrentTemperature/${sensor}`)
            if (temperatureResponse.ok) {
                const temperatureDTO = await temperatureResponse.json();
                document.getElementById(temperatureId).innerText = `${temperatureDTO.celsius} °C`;
            } else {
                document.getElementById(temperatureId).innerText = "Celsius could not be retrieved";
            }
        } catch (error) {
            console.error(error);
            document.getElementById(humidityId).innerText = "Error while retrieving humidity";
            document.getElementById(temperatureId).innerText = "Error while retrieving temperature";
        }
    }

    document.getElementById("currentData").addEventListener("click", async function () {
        console.log("Click event handler invoked.");
        await getDataForSensor(selectedSensor);
        console.log("getDataForSensor completed.");
        window.location.href = "currentOrRange.html";
        console.log("Navigating to currentOrRange.html.");
    });

    async function getDataAndDisplay(endpointName, valueName, sensor) {
        const getDataButton = document.getElementById(`getData${endpointName}`);
        const startDateInput = document.getElementById(`startDate${endpointName}`);
        const endDateInput = document.getElementById(`endDate${endpointName}`);
        const dataDiv = document.getElementById(`${endpointName.toLowerCase()}Data`);

        getDataButton.addEventListener('click', async function () {
            const startDate = new Date(startDateInput.value).toISOString();
            const endDate = new Date(endDateInput.value).toISOString();

            try {
                const response = await fetch(
                    `${url}${endpointName.toLowerCase()}/${endpointName.toLowerCase()}ByDateRange/${sensor}?startDate=${startDate}&endDate=${endDate}`);

                if (response.ok) {
                    const data = await response.json();

                    dataDiv.innerHTML = '';
                    data.forEach(item => {
                        const itemElement = document.createElement('p');
                        const formattedDate = new Date(item.date).toLocaleString();
                        const formattedValue = item[valueName.toLowerCase()].toFixed(1);
                        itemElement.textContent = `Date: ${formattedDate}, ${valueName}: ${formattedValue}%`;
                        dataDiv.appendChild(itemElement);
                    });
                } else {
                    console.error(`Error during ${endpointName.toLowerCase()} list retrieval: `, response.statusText)
                }
            } catch (error) {
                console.error(`Error during ${endpointName.toLowerCase()} list retrieval: `, error);
            }
        });
    }

    document.getElementById("dateRange").addEventListener("click", function () {
        getDataAndDisplay("Humidity", "Percentage", selectedSensor);
        getDataAndDisplay("Temperature", "Celsius", selectedSensor);
    })
    
});




