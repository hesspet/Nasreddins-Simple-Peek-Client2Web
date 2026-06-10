(() => {
    const state = {
        device: null,
        receiveCharacteristic: null,
        transmitCharacteristic: null,
        dotNetReference: null,
        operationAbortController: null
    };

    const textEncoder = new TextEncoder();
    const textDecoder = new TextDecoder("utf-8");

    function normalizeUuid(uuid) {
        return String(uuid).toLowerCase();
    }

    function getLastSeenText() {
        return `zuletzt gesehen ${new Intl.DateTimeFormat("de-DE", {
            hour: "2-digit",
            minute: "2-digit",
            second: "2-digit"
        }).format(new Date())}`;
    }

    function getDeviceIdentifier(deviceName) {
        const match = /BlePrompter-([0-9A-Fa-f]{4})/.exec(deviceName || "");
        return match ? `BP-${match[1].toUpperCase()}` : null;
    }

    function createDeviceInformation(device) {
        const deviceName = device?.name || "Unbekanntes Gerät";
        return {
            id: device?.id || deviceName,
            name: deviceName,
            deviceIdentifier: getDeviceIdentifier(deviceName),
            signalStrength: null,
            lastSeenText: getLastSeenText()
        };
    }

    function ensureSupported() {
        if (!("bluetooth" in navigator)) {
            throw new Error("Web Bluetooth not supported");
        }
    }

    function buildRequestOptions(scanMode, serviceUuid, namePrefix) {
        const optionalServices = [serviceUuid];

        if (scanMode === "AllDevices") {
            return { acceptAllDevices: true, optionalServices };
        }

        return {
            filters: [
                { services: [serviceUuid] },
                { namePrefix }
            ],
            optionalServices
        };
    }

    async function connectDevice(device, dotNetReference, serviceUuid, receiveCharacteristicUuid, transmitCharacteristicUuid) {
        state.dotNetReference = dotNetReference;
        state.device = device;

        device.removeEventListener("gattserverdisconnected", handleDisconnected);
        device.addEventListener("gattserverdisconnected", handleDisconnected);

        const server = await device.gatt.connect();
        const service = await server.getPrimaryService(normalizeUuid(serviceUuid));
        state.receiveCharacteristic = await service.getCharacteristic(normalizeUuid(receiveCharacteristicUuid));
        state.transmitCharacteristic = await service.getCharacteristic(normalizeUuid(transmitCharacteristicUuid));

        await state.transmitCharacteristic.startNotifications();
        state.transmitCharacteristic.removeEventListener("characteristicvaluechanged", handleNotification);
        state.transmitCharacteristic.addEventListener("characteristicvaluechanged", handleNotification);

        return createDeviceInformation(device);
    }

    function startCancelableOperation() {
        state.operationAbortController?.abort();
        state.operationAbortController = new AbortController();
        return state.operationAbortController;
    }

    function finishCancelableOperation(operationAbortController) {
        if (state.operationAbortController === operationAbortController) {
            state.operationAbortController = null;
        }
    }

    function throwIfOperationCanceled(operationAbortController) {
        if (operationAbortController.signal.aborted) {
            throw new Error("Bluetooth operation canceled");
        }
    }

    function waitForRetry(milliseconds, operationAbortController) {
        return new Promise((resolve, reject) => {
            const timeoutIdentifier = window.setTimeout(resolve, milliseconds);
            operationAbortController.signal.addEventListener("abort", () => {
                window.clearTimeout(timeoutIdentifier);
                reject(new Error("Bluetooth operation canceled"));
            }, { once: true });
        });
    }

    function handleNotification(event) {
        const responseText = textDecoder.decode(event.target.value.buffer);
        state.dotNetReference?.invokeMethodAsync("HandleBluetoothResponse", responseText);
    }

    function handleDisconnected() {
        state.receiveCharacteristic = null;
        state.transmitCharacteristic = null;
        state.dotNetReference?.invokeMethodAsync("HandleBluetoothDisconnected");
    }

    window.nasreddinBluetooth = {
        checkSupport() {
            return "bluetooth" in navigator;
        },

        canReconnectRememberedDevice() {
            return "bluetooth" in navigator && Boolean(navigator.bluetooth.getDevices);
        },

        async requestAndConnect(dotNetReference, scanMode, serviceUuid, receiveCharacteristicUuid, transmitCharacteristicUuid, namePrefix) {
            ensureSupported();
            const requestOptions = buildRequestOptions(scanMode, serviceUuid, namePrefix);
            const device = await navigator.bluetooth.requestDevice(requestOptions);
            return await connectDevice(device, dotNetReference, serviceUuid, receiveCharacteristicUuid, transmitCharacteristicUuid);
        },

        async reconnectRememberedDevice(dotNetReference, rememberedDeviceIdentifier, rememberedDeviceName, serviceUuid, receiveCharacteristicUuid, transmitCharacteristicUuid, timeoutMilliseconds) {
            ensureSupported();

            if (!navigator.bluetooth.getDevices) {
                throw new Error("Remembered device lookup not found");
            }

            const operationAbortController = startCancelableOperation();

            try {
                const devices = await navigator.bluetooth.getDevices();
                const device = devices.find(currentDevice =>
                    currentDevice.id === rememberedDeviceIdentifier || currentDevice.name === rememberedDeviceName);

                if (!device) {
                    throw new Error("Remembered device not found");
                }

                const reconnectDeadline = Date.now() + Math.max(0, timeoutMilliseconds || 0);
                let lastConnectError = null;

                while (Date.now() <= reconnectDeadline) {
                    throwIfOperationCanceled(operationAbortController);

                    try {
                        const bluetoothDevice = await connectDevice(device, dotNetReference, serviceUuid, receiveCharacteristicUuid, transmitCharacteristicUuid);
                        if (operationAbortController.signal.aborted) {
                            if (device.gatt?.connected) {
                                device.gatt.disconnect();
                            }

                            throw new Error("Bluetooth operation canceled");
                        }

                        return bluetoothDevice;
                    } catch (connectError) {
                        lastConnectError = connectError;

                        if (device.gatt?.connected) {
                            device.gatt.disconnect();
                        }

                        await waitForRetry(2000, operationAbortController);
                    }
                }

                throw lastConnectError || new Error("Remembered device not found");
            } finally {
                finishCancelableOperation(operationAbortController);
            }
        },

        cancelCurrentOperation() {
            state.operationAbortController?.abort();
            state.operationAbortController = null;
        },

        async sendCommand(command) {
            if (!state.receiveCharacteristic) {
                throw new Error("Bluetooth not connected");
            }

            const commandBytes = textEncoder.encode(command);

            if (state.receiveCharacteristic.writeValueWithoutResponse) {
                await state.receiveCharacteristic.writeValueWithoutResponse(commandBytes);
                return;
            }

            await state.receiveCharacteristic.writeValue(commandBytes);
        },

        async disconnect() {
            if (state.transmitCharacteristic) {
                try {
                    state.transmitCharacteristic.removeEventListener("characteristicvaluechanged", handleNotification);
                    await state.transmitCharacteristic.stopNotifications();
                } catch {
                    // Der Browser kann Notifications bereits beendet haben.
                }
            }

            if (state.device?.gatt?.connected) {
                state.device.gatt.disconnect();
            }

            state.device = null;
            state.receiveCharacteristic = null;
            state.transmitCharacteristic = null;
        }
    };
})();
