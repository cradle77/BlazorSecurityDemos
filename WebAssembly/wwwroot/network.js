(function () {
    window.blazorNetwork = {
        init: (callback) => {
            const updateOnlineStatus = (e) => {
                callback.invokeMethodAsync('UpdateOnlineStatus', navigator.onLine);
            }

            window.addEventListener('online', updateOnlineStatus);
            window.addEventListener('offline', updateOnlineStatus);
            updateOnlineStatus();
        }
    };
})();