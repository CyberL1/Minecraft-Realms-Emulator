window.serverLogsStream = {
    logs: "x",

    start: (dotNetObject, serverId) => {
        logs = new EventSource(`http://localhost:8080/api/admin/servers/${serverId}/logs`, { withCredentials: true });

        logs.onmessage = event => {
            dotNetObject.invokeMethodAsync("ReceiveLog", event.data);
        }
    },

    stop: () => logs.close()
}