window.nasreddinAudioSpy = (() => {
    const handlers = new Map();
    const wordPattern = /[\p{L}\p{N}_ÄÖÜäöüß-]+/gu;
    const wordEndPattern = /[^\p{L}\p{N}_ÄÖÜäöüß-]$/u;

    function attach(elementId, dotNetReference) {
        const element = document.getElementById(elementId);

        if (!element) {
            return false;
        }

        detach(elementId);

        const state = { processedWordCount: 0 };
        const handler = () => handleInput(element, dotNetReference, state);
        element.addEventListener("input", handler);
        handlers.set(elementId, { element, handler, state });
        return true;
    }

    function clear(elementId) {
        const element = document.getElementById(elementId);

        if (element) {
            element.value = "";
            element.focus({ preventScroll: true });
        }

        const registration = handlers.get(elementId);

        if (registration) {
            registration.state.processedWordCount = 0;
        }
    }

    function detach(elementId) {
        const registration = handlers.get(elementId);

        if (!registration) {
            return;
        }

        registration.element.removeEventListener("input", registration.handler);
        handlers.delete(elementId);
    }

    function handleInput(element, dotNetReference, state) {
        trimToFiveLines(element);

        const text = element.value;

        if (!wordEndPattern.test(text)) {
            return;
        }

        const words = Array.from(text.matchAll(wordPattern), match => match[0]);

        if (words.length === 0) {
            return;
        }

        if (words.length < state.processedWordCount) {
            state.processedWordCount = 0;
        }

        const nextWords = words.slice(state.processedWordCount);
        state.processedWordCount = words.length;

        for (const word of nextWords) {
            dotNetReference.invokeMethodAsync("HandleCompletedWord", word);
        }
    }

    function trimToFiveLines(element) {
        const lines = element.value.split(/\r?\n/);

        if (lines.length <= 5) {
            return;
        }

        element.value = lines.slice(lines.length - 5).join("\n");
        element.selectionStart = element.value.length;
        element.selectionEnd = element.value.length;
    }

    return { attach, clear, detach };
})();
