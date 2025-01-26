fetch('http://localhost:8081/api')
    .then(response => response.text())
    .then(data => {
        document.getElementById('message').textContent = data;
    })
    .catch(error => {
        console.error('Error fetching data:', error);
        document.getElementById('message').textContent = "Error loading data from Sprint 1 WebServer.";
    });

