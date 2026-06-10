(() => {
    window.peekUi = {
        tryFullscreen: async function () {
            const root = document.documentElement;

            if (!root.requestFullscreen) {
                return false;
            }

            try {
                await root.requestFullscreen({
                    navigationUI: "hide"
                });
                return true;
            } catch {
                return false;
            }
        }
    };
})();
