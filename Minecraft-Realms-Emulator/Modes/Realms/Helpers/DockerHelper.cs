﻿using Minecraft_Realms_Emulator.Entities;
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
            ProcessStartInfo serverProcessInfo = new();

            serverProcessInfo.FileName = "docker";
            serverProcessInfo.Arguments = $"container stop realm-server-{world.Id}";

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
    }
}