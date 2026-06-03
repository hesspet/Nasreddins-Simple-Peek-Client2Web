(() => {
    const state = {
        device: null,
        receiveCharacteristic: null,
        transmitCharacteristic: null,
        dotNetReference: null
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

        async requestAndConnect(dotNetReference, scanMode, serviceUuid, receiveCharacteristicUuid, transmitCharacteristicUuid, namePrefix) {
            ensureSupported();
            const requestOptions = buildRequestOptions(scanMode, serviceUuid, namePrefix);
            const device = await navigator.bluetooth.requestDevice(requestOptions);
            return await connectDevice(device, dotNetReference, serviceUuid, receiveCharacteristicUuid, transmitCharacteristicUuid);
        },

        async reconnectRememberedDevice(dotNetReference, rememberedDeviceIdentifier, rememberedDeviceName, serviceUuid, receiveCharacteristicUuid, transmitCharacteristicUuid) {
            ensureSupported();

            if (!navigator.bluetooth.getDevices) {
                throw new Error("Remembered device lookup not found");
            }

            const devices = await navigator.bluetooth.getDevices();
            const device = devices.find(currentDevice =>
                currentDevice.id === rememberedDeviceIdentifier || currentDevice.name === rememberedDeviceName);

            if (!device) {
                throw new Error("Remembered device not found");
            }

            return await connectDevice(device, dotNetReference, serviceUuid, receiveCharacteristicUuid, transmitCharacteristicUuid);
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
