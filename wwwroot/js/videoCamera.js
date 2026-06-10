(() => {
    const streamsByElement = new Map();
    let testVideoObjectUrl = null;

    function getVideoElement(videoElementIdentifier) {
        const videoElement = document.getElementById(videoElementIdentifier);

        if (!videoElement) {
            throw new Error("Videoelement wurde nicht gefunden.");
        }

        return videoElement;
    }

    function getFileInputElement(fileInputElementIdentifier) {
        const fileInputElement = document.getElementById(fileInputElementIdentifier);

        if (!fileInputElement) {
            throw new Error("Dateiauswahl wurde nicht gefunden.");
        }

        return fileInputElement;
    }

    function releaseTestVideoObjectUrl() {
        if (testVideoObjectUrl) {
            URL.revokeObjectURL(testVideoObjectUrl);
            testVideoObjectUrl = null;
        }
    }

    function isMp4File(file) {
        const fileName = String(file?.name || "").toLowerCase();
        const fileType = String(file?.type || "").toLowerCase();
        return fileType === "video/mp4" || fileName.endsWith(".mp4");
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
            videoElement.removeAttribute("src");
            videoElement.loop = false;
            await videoElement.play();
            await applyZoom(videoElement, zoom);
        },

        selectTestVideoFile(fileInputElementIdentifier) {
            const fileInputElement = getFileInputElement(fileInputElementIdentifier);
            const file = fileInputElement.files?.[0];

            if (!file || !isMp4File(file)) {
                throw new Error("Bitte eine MP4-Datei auswählen.");
            }

            releaseTestVideoObjectUrl();
            testVideoObjectUrl = URL.createObjectURL(file);
            return file.name;
        },

        removeTestVideoFile(fileInputElementIdentifier) {
            releaseTestVideoObjectUrl();
            const fileInputElement = document.getElementById(fileInputElementIdentifier);

            if (fileInputElement) {
                fileInputElement.value = "";
            }
        },

        async startTestVideo(videoElementIdentifier, zoom) {
            if (!testVideoObjectUrl) {
                throw new Error("Bitte wähle in den Einstellungen eine MP4-Datei für diese App-Sitzung aus.");
            }

            const videoElement = getVideoElement(videoElementIdentifier);
            await this.stop(videoElementIdentifier);

            videoElement.srcObject = null;
            videoElement.src = testVideoObjectUrl;
            videoElement.loop = true;
            videoElement.muted = true;
            videoElement.playsInline = true;
            videoElement.autoplay = true;
            await videoElement.play();
            await applyZoom(videoElement, zoom);
        },

        async setZoom(videoElementIdentifier, zoom) {
            const videoElement = getVideoElement(videoElementIdentifier);
            await applyZoom(videoElement, zoom);
        },

        async setPaused(videoElementIdentifier, isPaused) {
            const videoElement = getVideoElement(videoElementIdentifier);

            if (isPaused) {
                videoElement.pause();
                return;
            }

            await videoElement.play();
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
                videoElement.pause();
                videoElement.srcObject = null;
                videoElement.removeAttribute("src");
                videoElement.loop = false;
                videoElement.load();
            }
        }
    };
})();
