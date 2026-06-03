(() => {
    const streamsByElement = new Map();

    function getVideoElement(videoElementIdentifier) {
        const videoElement = document.getElementById(videoElementIdentifier);

        if (!videoElement) {
            throw new Error("Videoelement wurde nicht gefunden.");
        }

        return videoElement;
    }

    async function applyZoom(videoElement, zoom) {
        const stream = streamsByElement.get(videoElement.id);
        const track = stream?.getVideoTracks?.()[0];
        const capabilities = track?.getCapabilities?.();
        const requestedZoom = Math.max(0, Number(zoom) || 0);

        if (capabilities?.zoom && track.applyConstraints) {
            const minimumZoom = capabilities.zoom.min ?? 1;
            const maximumZoom = capabilities.zoom.max ?? 1;
            const hardwareZoom = minimumZoom + (maximumZoom - minimumZoom) * requestedZoom;
            await track.applyConstraints({ advanced: [{ zoom: hardwareZoom }] });
            videoElement.style.transform = "scale(1)";
            return;
        }

        videoElement.style.transform = `scale(${1 + requestedZoom})`;
    }

    window.nasreddinCamera = {
        checkSupport() {
            return Boolean(navigator.mediaDevices?.getUserMedia);
        },

        async start(videoElementIdentifier, zoom) {
            if (!navigator.mediaDevices?.getUserMedia) {
                throw new Error("Dieser Browser stellt keine Kamera-API bereit.");
            }

            const videoElement = getVideoElement(videoElementIdentifier);
            await this.stop(videoElementIdentifier);

            const stream = await navigator.mediaDevices.getUserMedia({
                video: {
                    facingMode: { ideal: "environment" },
                    width: { ideal: 1280 },
                    height: { ideal: 720 }
                },
                audio: false
            });

            streamsByElement.set(videoElementIdentifier, stream);
            videoElement.srcObject = stream;
            await videoElement.play();
            await applyZoom(videoElement, zoom);
        },

        async setZoom(videoElementIdentifier, zoom) {
            const videoElement = getVideoElement(videoElementIdentifier);
            await applyZoom(videoElement, zoom);
        },

        async stop(videoElementIdentifier) {
            const stream = streamsByElement.get(videoElementIdentifier);

            if (stream) {
                for (const track of stream.getTracks()) {
                    track.stop();
                }
            }

            streamsByElement.delete(videoElementIdentifier);

            const videoElement = document.getElementById(videoElementIdentifier);
            if (videoElement) {
                videoElement.srcObject = null;
            }
        }
    };
})();
