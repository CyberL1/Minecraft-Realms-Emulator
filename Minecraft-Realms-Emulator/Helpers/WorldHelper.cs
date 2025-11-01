using Minecraft_Realms_Emulator.Data;
using Minecraft_Realms_Emulator.Enums;

namespace Minecraft_Realms_Emulator.Helpers;

public class WorldHelper(DataContext context, int worldId)
{
    public async Task<string> GetState()
    {
        var world = context.Worlds.First(w => w.Id == worldId);

        if (world.Name == null)
        {
            return nameof(StateEnum.UNINITIALIZED);
        }

        if (await new DockerHelper(world.Id).IsRunning())
        {
            return nameof(StateEnum.OPEN);
        }

        return nameof(StateEnum.CLOSED);
    }
}