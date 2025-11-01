using Docker.DotNet;
using Docker.DotNet.Models;

namespace Minecraft_Realms_Emulator.Helpers
{
    public class DockerHelper(int worldId)
    {
        private readonly DockerClient _dockerClient = new DockerClientConfiguration().CreateClient();

        public async Task CreateServer(int port)
        {
            var containerConfig = new CreateContainerParameters
            {
                Name = $"realm-server-{worldId}",
                Image = "realm-server",
                ExposedPorts = new Dictionary<string, EmptyStruct>
                {
                    { "25565", new EmptyStruct() }
                },
                HostConfig = new HostConfig
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        {
                            "25565", new List<PortBinding>
                            {
                                new()
                                {
                                    HostIP = "0.0.0.0",
                                    HostPort = port.ToString()
                                }
                            }
                        }
                    },
                    Mounts = new List<Mount>
                    {
                        new()
                        {
                            Type = "volume",
                            Source = $"realm-server-{worldId}",
                            Target = "/mc"
                        }
                    }
                }
            };

            await _dockerClient.Containers.CreateContainerAsync(containerConfig);
        }

        public async Task<bool> IsRunning()
        {
            var container = await _dockerClient.Containers.InspectContainerAsync($"realm-server-{worldId}");
            return container.State.Running;
        }

        public async Task StartServer()
        {
            await _dockerClient.Containers.StartContainerAsync($"realm-server-{worldId}",
                new ContainerStartParameters());
        }

        public async Task StopServer()
        {
            await ExecuteCommand("stop");
        }

        public async Task RebootServer()
        {
            await _dockerClient.Containers.RestartContainerAsync($"realm-server-{worldId}",
                new ContainerRestartParameters());
        }

        public async Task DeleteServer()
        {
            await _dockerClient.Containers.RemoveContainerAsync($"realm-server-{worldId}",
                new ContainerRemoveParameters { Force = true });
            await _dockerClient.Volumes.RemoveAsync($"realm-server-{worldId}");
        }

        public async Task GetServerLogsStreamAsync(Action<string> handler)
        {
            var parameters = new ContainerLogsParameters
            {
                ShowStdout = true,
                ShowStderr = true,
                Follow = true,
                Tail = "100"
            };

            using var stream = await _dockerClient.Containers.GetContainerLogsAsync($"realm-server-{worldId}", false,
                parameters, CancellationToken.None);

            var buffer = new byte[1024];

            while (true)
            {
                var bytesRead = await stream.ReadOutputAsync(buffer, 0, buffer.Length, CancellationToken.None);
                if (bytesRead.Count == 0) break;

                var text = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead.Count);
                foreach (var line in text.Split('\n', StringSplitOptions.RemoveEmptyEntries))
                {
                    handler(line);
                }
            }
        }

        public async Task<long> ExecuteCommand(string command)
        {
            var execCreateResponse = await _dockerClient.Exec.ExecCreateContainerAsync($"realm-server-{worldId}",
                new ContainerExecCreateParameters
                {
                    Cmd = new List<string> { "rcon-cli", command },
                    AttachStderr = true,
                    AttachStdout = true
                });

            using (var stream =
                   await _dockerClient.Exec.StartAndAttachContainerExecAsync(execCreateResponse.ID, false))
            {
                var buffer = new byte[1024];
                while (true)
                {
                    var result = await stream.ReadOutputAsync(buffer, 0, buffer.Length, CancellationToken.None);
                    if (result.EOF) break;
                    var output = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.Write(output);
                }
            }

            var execInspect = await _dockerClient.Exec.InspectContainerExecAsync(execCreateResponse.ID);
            return execInspect.ExitCode;
        }

        public async Task<long> RunCommand(string command)
        {
            var execCreateResponse = await _dockerClient.Exec.ExecCreateContainerAsync($"realm-server-{worldId}",
                new ContainerExecCreateParameters
                {
                    Cmd = new List<string> { "/bin/sh", "-c", command },
                    AttachStderr = true,
                    AttachStdout = true
                });

            using (var stream =
                   await _dockerClient.Exec.StartAndAttachContainerExecAsync(execCreateResponse.ID, false))
            {
                var buffer = new byte[1024];
                while (true)
                {
                    var result = await stream.ReadOutputAsync(buffer, 0, buffer.Length, CancellationToken.None);
                    if (result.EOF) break;
                    var output = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.Write(output);
                }
            }

            var execInspect = await _dockerClient.Exec.InspectContainerExecAsync(execCreateResponse.ID);
            return execInspect.ExitCode;
        }
    }
}