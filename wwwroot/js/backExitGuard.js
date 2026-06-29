window.nasreddinBackExitGuard = (() => {
    const guardStateKey = "nasreddinBackExitGuard";
    let dotNetReference = null;
    let isActive = false;
    let isExitWarningOpen = false;

    function pushGuardState() {
        window.history.pushState({ [guardStateKey]: true }, "", window.location.href);
    }

    function deactivate(shouldClearReference) {
        if (isActive) {
            window.removeEventListener("popstate", handlePopState);
        }

        isActive = false;
        isExitWarningOpen = false;

        if (shouldClearReference) {
            dotNetReference = null;
        }
    }

    function handlePopState() {
        if (!isActive) {
            return;
        }

        if (isExitWarningOpen) {
            deactivate(false);
            window.history.back();
            return;
        }

        isExitWarningOpen = true;

        if (dotNetReference) {
            dotNetReference.invokeMethodAsync("RequestExitAsync").catch(() => { });
        }

        pushGuardState();
    }

    function initialize(nextDotNetReference) {
        dotNetReference = nextDotNetReference;

        if (isActive) {
            return;
        }

        isActive = true;
        isExitWarningOpen = false;
        pushGuardState();
        window.addEventListener("popstate", handlePopState);
    }

    function cancelExit() {
        isExitWarningOpen = false;
    }

    function dispose() {
        deactivate(true);
    }

    return {
        initialize,
        cancelExit,
        dispose
    };
})();
