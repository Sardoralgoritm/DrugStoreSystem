window.getLocation = function () {
    return new Promise(function (resolve, reject) {
        if (!navigator.geolocation) {
            reject('Geolocation not supported');
            return;
        }
        navigator.geolocation.getCurrentPosition(
            function (pos) {
                resolve({ lat: pos.coords.latitude, lng: pos.coords.longitude });
            },
            function (err) {
                reject(err.message);
            },
            { timeout: 10000 }
        );
    });
};
