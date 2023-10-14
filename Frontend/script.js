const url = "http://NBNL865.rademaker.nl:5233/api/";










    


































// const currentUrl = window.location.href;


// document.addEventListener('DOMContentLoaded', function () {
//     // Home
//     document.getElementById("livingRoom").addEventListener("click", function () {
//         navigateToCurrentOrRangePage("LivingRoom");
//     });

//     document.getElementById("bedroom").addEventListener("click", function () {
//         navigateToCurrentOrRangePage("Bedroom");
//     });

//     document.getElementById("workspace").addEventListener("click", function () {
//         navigateToCurrentOrRangePage("WorkSpace");
//     });
    

//     // currentOrRange page navigation function
//     async function navigateToCurrentOrRangePage(sensor) {
//         const currentOrRangePageName = "currentOrRange.html";
//         // add selected sensor as queryparameter to URL
//         const currentOrRangePageURL = `${currentOrRangePageName}?sensor=${sensor}`;
//         // navigate to new page
//         window.location.href = currentOrRangePageURL;
//     }


//     // currentOrRange
//     const urlParams = new URLSearchParams(window.location.search);
//     const selectedSensor = urlParams.get("sensor");


//     // Get the data for the sensor
//     async function getDataForSensor(sensor) {
//         const humidityId = "currentHumidity";
//         const temperatureId = "currentTemperature";

//         try {
//             // get current humidity for selected sensor
//             const humidityResponse = await fetch(`${url}humidity/getCurrentHumidity/${sensor}`);
//             if (humidityResponse.ok) {
//                 const humidityDTO = await humidityResponse.json();
//                 document.getElementById(humidityId).innerText = `${humidityDTO.percentage}%`;
//             } else {
//                 document.getElementById(humidityId).innerText = "Percentage could not be retrieved";
//             }
//             // get current temperature for selected sensor
//             const temperatureResponse = await fetch(`${url}temperature/getCurrentTemperature/${sensor}`)
//             if (temperatureResponse.ok) {
//                 const temperatureDTO = await temperatureResponse.json();
//                 document.getElementById(temperatureId).innerText = `${temperatureDTO.celsius} °C`;
//             } else {
//                 document.getElementById(temperatureId).innerText = "Celsius could not be retrieved";
//             }
//         } catch (error) {
//             console.error(error);
//             document.getElementById(humidityId).innerText = "Error while retrieving humidity";
//             document.getElementById(temperatureId).innerText = "Error while retrieving temperature";
//         }
//     }


//     // Button for current data page
//     document.getElementById("currentData").addEventListener("click", async e => {
//         try {
//             console.log(e);
//             console.log("Click event handler invoked.");
//             window.location.href = "current.html";
//             console.log("Navigating to currentOrRange.html.");
//             await getDataForSensor("LivingRoom");
//             console.log("getDataForSensor completed.");         
//         }
//         catch(error) {
//             console.error(error);
//         }
//     });


//     // Date range function
//     async function getDataAndDisplay(endpointName, valueName, sensor) {
//         const getDataButton = document.getElementById(`getData${endpointName}`);
//         const startDateInput = document.getElementById(`startDate${endpointName}`);
//         const endDateInput = document.getElementById(`endDate${endpointName}`);
//         const dataDiv = document.getElementById(`${endpointName.toLowerCase()}Data`);

//         getDataButton.addEventListener('click', async function () {
//             const startDate = new Date(startDateInput.value).toISOString();
//             const endDate = new Date(endDateInput.value).toISOString();

//             try {
//                 const response = await fetch(
//                     `${url}${endpointName.toLowerCase()}/${endpointName.toLowerCase()}ByDateRange/${sensor}?startDate=${startDate}&endDate=${endDate}`);

//                 if (response.ok) {
//                     const data = await response.json();

//                     dataDiv.innerHTML = '';
//                     data.forEach(item => {
//                         const itemElement = document.createElement('p');
//                         const formattedDate = new Date(item.date).toLocaleString();
//                         const formattedValue = item[valueName.toLowerCase()].toFixed(1);
//                         itemElement.textContent = `Date: ${formattedDate}, ${valueName}: ${formattedValue}%`;
//                         dataDiv.appendChild(itemElement);
//                     });
//                 } else {
//                     console.error(`Error during ${endpointName.toLowerCase()} list retrieval: `, response.statusText)
//                 }
//             } catch (error) {
//                 console.error(`Error during ${endpointName.toLowerCase()} list retrieval: `, error);
//             }
//         });
//     }


//     // Button for Date Range
//     console.log(document.getElementById("dateRange"));
//     document.getElementById("dateRange").addEventListener("click", async function () {
//         try {
//             console.log("Click event handler for dateRange invoked.");
//             await getDataAndDisplay("Humidity", "Percentage", selectedSensor);
//             await getDataAndDisplay("Temperature", "Celsius", selectedSensor);

//             window.location.href = "dateRange.html";
//         }
//         catch(error) {
//             console.error(error);
//         }
//     })
    
// });




