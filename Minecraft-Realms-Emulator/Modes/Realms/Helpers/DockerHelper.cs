using Minecraft_Realms_Emulator.Shared.Entities;
using System.Diagnostics;

namespace Minecraft_Realms_Emulator.Modes.Realms.Helpers
{
    public class DockerHelper(World world)
    {
        public void CreateServer(int port)
        {
            ProcessStartInfo serverProcessInfo = new();

            serverProcessInfo.FileName = "docker";
            serverProcessInfo.Arguments = $"run -d --name realm-server-{world.Id} -p {port}:25565 realm-server";

            Process serverProcess = new();
            serverProcess.StartInfo = serverProcessInfo;
            serverProcess.Start();
        }

        public bool IsRunning()
        {
            ProcessStartInfo containerStateProcessInfo = new();
            
            containerStateProcessInfo.FileName = "docker";
            containerStateProcessInfo.Arguments = $"inspect realm-server-{world.Id} -f {{{{.State.Running}}}}";

            containerStateProcessInfo.RedirectStandardOutput = true;

            Process containerStateProcess = new();
            containerStateProcess.StartInfo = containerStateProcessInfo;
            containerStateProcess.Start();

            containerStateProcess.WaitForExit();
            return bool.Parse(containerStateProcess.StandardOutput.ReadToEnd());
        }

        public void StartServer()
        {
            ProcessStartInfo serverProcessInfo = new();

            serverProcessInfo.FileName = "docker";
            serverProcessInfo.Arguments = $"container start realm-server-{world.Id}";

            Process serverProcess = new();
            serverProcess.StartInfo = serverProcessInfo;
            serverProcess.Start();
        }

        public void StopServer()
        {
            ExecuteCommand("stop");
        }

        public void RebootServer()
        {
            ProcessStartInfo serverProcessInfo = new();

            serverProcessInfo.FileName = "docker";
            serverProcessInfo.Arguments = $"container restart realm-server-{world.Id}";

            Process serverProcess = new();
            serverProcess.StartInfo = serverProcessInfo;
            serverProcess.Start();
        }

        public void DeleteServer()
        {
            ProcessStartInfo serverProcessInfo = new();

            serverProcessInfo.FileName = "docker";
            serverProcessInfo.Arguments = $"container rm realm-server-{world.Id}";

            Process serverProcess = new();
            serverProcess.StartInfo = serverProcessInfo;
            serverProcess.Start();
        }

        public async Task GetServerLogsStreamAsync(Action<string> handler)
        {
            ProcessStartInfo serverProcessInfo = new();

            serverProcessInfo.FileName = "docker";
            serverProcessInfo.Arguments = $"container logs -f realm-server-{world.Id} --tail 100";
            serverProcessInfo.RedirectStandardOutput = true;

            Process serverProcess = new();
            serverProcess.StartInfo = serverProcessInfo;
            serverProcess.Start();

            List<string> logs = [];

            await Task.Run(() =>
            {
                while (!serverProcess.StandardOutput.EndOfStream)
                {
                    string line = serverProcess.StandardOutput.ReadLine();

                    if (line != null)
                    {
                        handler(line);
                    }
                }
            });
        }

        public void ExecuteCommand(string command)
        {
            ProcessStartInfo commandProcessInfo = new();

            commandProcessInfo.FileName = "docker";
            commandProcessInfo.Arguments = $"exec realm-server-{world.Id} rcon-cli {command}";

            Process commandProcess = new();
            commandProcess.StartInfo = commandProcessInfo;

            commandProcess.Start();
        }

        public void RunCommand(string command)
        {
            ProcessStartInfo commandProcessInfo = new();

            commandProcessInfo.FileName = "docker";
            commandProcessInfo.Arguments = $"exec realm-server-{world.Id} /bin/sh -c \"{command}\"";

            Process commandProcess = new();
            commandProcess.StartInfo = commandProcessInfo;

            commandProcess.Start();
        }
    }
}