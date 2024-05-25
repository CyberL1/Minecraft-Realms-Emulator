using System.Diagnostics;

namespace Minecraft_Realms_Emulator.Modes.Realms.Helpers
{
    public class DockerHelper
    {
        public void CreateServer(int id, int port)
        {
            ProcessStartInfo serverProcessInfo = new();

            serverProcessInfo.FileName = "docker";
            serverProcessInfo.Arguments = $"run -d --name realm-server-{id} -p {port}:25565 realm-server";

            Process serverProcess = new();
            serverProcess.StartInfo = serverProcessInfo;
            serverProcess.Start();
        }

        public void StartServer(int id)
        {
            ProcessStartInfo serverProcessInfo = new();

            serverProcessInfo.FileName = "docker";
            serverProcessInfo.Arguments = $"container start realm-server-{id}";

            Process serverProcess = new();
            serverProcess.StartInfo = serverProcessInfo;
            serverProcess.Start();
        }

        public void DeleteServer(int id)
        {
            ProcessStartInfo serverProcessInfo = new();

            serverProcessInfo.FileName = "docker";
            serverProcessInfo.Arguments = $"container rm realm-server-{id}";

            Process serverProcess = new();
            serverProcess.StartInfo = serverProcessInfo;
            serverProcess.Start();
        }
    }
}
