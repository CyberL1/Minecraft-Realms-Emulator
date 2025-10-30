using Microsoft.AspNetCore.Mvc.Testing;
using Minecraft_Realms_Emulator.Requests;
using Minecraft_Realms_Emulator.Responses;
using Newtonsoft.Json;
using System.Text;

namespace UnitTests
{
    [TestClass]
    public class WorldsControllerTests
    {
        [TestInitialize]
        public void Setup()
        {
            Environment.SetEnvironmentVariable("CONNECTION_STRING", "User Id=postgres;Password=postgres;Server=localhost;Port=5432;Database=Minecraft-Realms-Emulator;");
        }

        [TestMethod]
        public async Task GetWorlds_Always_ReturnsPlayerWorlds()
        {
            var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Add("Cookie", "sid=token:fakeToken:069a79f444e94726a5befca90e38aaf5;user=testPlayer;version=1.21.1");

            var response = await client.GetAsync("worlds");

            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.IsNotNull(await response.Content.ReadAsStringAsync());
        }

        [TestMethod]
        public async Task GetWorldsSnapshot_Always_ReturnsPlayerWorlds()
        {
            var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Add("Cookie", "sid=token:fakeToken:069a79f444e94726a5befca90e38aaf5;user=testPlayer;version=1.21.1");

            var response = await client.GetAsync("worlds/listUserWorldsOfType/any");

            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.IsNotNull(await response.Content.ReadAsStringAsync());
        }

        [TestMethod]
        public async Task GetPrereleaseWorlds_Always_ReturnsPrereleaseEligibleWorlds()
        {
            var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Add("Cookie", "sid=token:fakeToken:069a79f444e94726a5befca90e38aaf5;user=testPlayer;version=1.21.1");
            client.DefaultRequestHeaders.Add("Is-Prerelease", "true");

            var response = await client.GetAsync("worlds/listPrereleaseEligibleWorlds");

            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.IsNotNull(await response.Content.ReadAsStringAsync());
        }

        [TestMethod]
        public async Task GetWorldById_NotInitialized_ReturnsError()
        {
            var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Add("Cookie", "sid=token:fakeToken:069a79f444e94726a5befca90e38aaf5;user=testPlayer;version=1.21.1");

            var response = await client.GetAsync("worlds/1");

            Assert.IsFalse(response.IsSuccessStatusCode);

            string expectedResponse = "{\"errorCode\":400,\"errorMsg\":\"Initialize the world first\"}";

            Assert.AreEqual(expectedResponse, await response.Content.ReadAsStringAsync());
        }

        [TestMethod]
        public async Task Initialize_NotInitializedAndOwner_ReturnsWorld()
        {
            var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Add("Cookie", "sid=token:fakeToken:069a79f444e94726a5befca90e38aaf5;user=testPlayer;version=1.21.1");

            WorldCreateRequest request = new()
            {
                Name = "Testing",
                Description = "Created by unit tests"
            };

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("worlds/1/initialize", content);

            Assert.IsTrue(response.IsSuccessStatusCode);

            var jsonResponse = JsonConvert.DeserializeObject<WorldResponse>(await response.Content.ReadAsStringAsync());

            string expectedResponse = $"{{\"id\":1,\"subscription\":{{\"id\":1,\"worldId\":1,\"world\":null,\"startDate\":\"{jsonResponse.Subscription.StartDate.ToString("O")}\",\"subscriptionType\":\"NORMAL\"}},\"owner\":\"testPlayer\",\"ownerUUID\":\"069a79f444e94726a5befca90e38aaf5\",\"name\":\"Testing\",\"motd\":\"Created by unit tests\",\"state\":\"OPEN\",\"worldType\":\"NORMAL\",\"players\":[],\"maxPlayers\":10,\"minigame\":null,\"activeSlot\":1,\"slots\":[{{\"id\":1,\"world\":null,\"slotId\":1,\"slotName\":\"\",\"version\":\"1.21.1\",\"gameMode\":0,\"difficulty\":2,\"spawnProtection\":0,\"forceGameMode\":false,\"pvp\":true,\"spawnAnimals\":true,\"spawnMonsters\":true,\"spawnNPCs\":true,\"commandBlocks\":false}}],\"member\":false,\"parentWorld\":null}}";
            Assert.AreEqual(expectedResponse, await response.Content.ReadAsStringAsync());
        }

        [TestMethod]
        public async Task GetWorldById_IsInitializedAndOwner_ReturnsWorld()
        {
            var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Add("Cookie", "sid=token:fakeToken:069a79f444e94726a5befca90e38aaf5;user=testPlayer;version=1.21.1");

            var response = await client.GetAsync("worlds/1");
            Assert.IsTrue(response.IsSuccessStatusCode);

            string expectedResponse = "{\"remoteSubscriptionId\":\"00000000-0000-0000-0000-000000000000\",\"daysLeft\":30,\"expired\":false,\"expiredTrial\":false,\"compatibility\":\"COMPATIBLE\",\"slots\":[{\"slotId\":1,\"options\":\"{\\\"slotName\\\":\\\"\\\",\\\"gameMode\\\":0,\\\"difficulty\\\":2,\\\"spawnProtection\\\":0,\\\"forceGameMode\\\":false,\\\"pvp\\\":true,\\\"spawnAnimals\\\":true,\\\"spawnMonsters\\\":true,\\\"spawnNPCs\\\":true,\\\"commandBlocks\\\":false,\\\"version\\\":\\\"1.21.1\\\",\\\"compatibility\\\":\\\"COMPATIBLE\\\"}\"}],\"activeVersion\":\"1.21.1\",\"parentWorldId\":null,\"parentWorldName\":null,\"minigameId\":null,\"minigameName\":null,\"minigameImage\":null,\"id\":1,\"subscription\":null,\"owner\":\"testPlayer\",\"ownerUUID\":\"069a79f444e94726a5befca90e38aaf5\",\"name\":\"Testing\",\"motd\":\"Created by unit tests\",\"state\":\"OPEN\",\"worldType\":\"NORMAL\",\"players\":[],\"maxPlayers\":10,\"minigame\":null,\"activeSlot\":1,\"member\":false,\"parentWorld\":null}";
            Assert.AreEqual(expectedResponse, await response.Content.ReadAsStringAsync());
        }

        [TestMethod]
        public async Task GetWorldById_isInitializedAndNotOwner_ReturnsError()
        {
            var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Add("Cookie", "sid=token:fakeToken:d33b406631524e31805528e304f7401f;user=testPlayer2;version=1.21.1");

            var response = await client.GetAsync("worlds/1");

            Assert.IsFalse(response.IsSuccessStatusCode);

            string expectedResponse = "You don't own this world";
            Assert.AreEqual(expectedResponse, await response.Content.ReadAsStringAsync());
        }

        [TestMethod]
        public async Task GetWorldById_NotFound_ReturnsError()
        {
            var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Add("Cookie", "sid=token:fakeToken:069a79f444e94726a5befca90e38aaf5;user=testPlayer;version=1.21.1");

            var response = await client.GetAsync("worlds/2");

            Assert.IsFalse(response.IsSuccessStatusCode);

            string expectedResponse = "{\"errorCode\":404,\"errorMsg\":\"World not found\"}";
            Assert.AreEqual(expectedResponse, await response.Content.ReadAsStringAsync());
        }

        [TestMethod]
        public async Task Initialize_NotFound_ReturnsError()
        {
            var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Add("Cookie", "sid=token:fakeToken:069a79f444e94726a5befca90e38aaf5;user=testPlayer;version=1.21.1");

            WorldCreateRequest request = new()
            {
                Name = "Testing",
                Description = "Created by unit tests"
            };

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("worlds/2/initialize", content);

            Assert.IsFalse(response.IsSuccessStatusCode);

            string expectedResponse = "{\"errorCode\":404,\"errorMsg\":\"World not found\"}";
            Assert.AreEqual(expectedResponse, await response.Content.ReadAsStringAsync());
        }

        [TestMethod]
        public async Task Initialize_AlreadyInitialized_ReturnsError()
        {
            var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Add("Cookie", "sid=token:fakeToken:069a79f444e94726a5befca90e38aaf5;user=testPlayer;version=1.21.1");

            WorldCreateRequest request = new()
            {
                Name = "Testing",
                Description = "Created by unit tests"
            };

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("worlds/1/initialize", content);

            Assert.IsFalse(response.IsSuccessStatusCode);

            string expectedResponse = "{\"errorCode\":401,\"errorMsg\":\"World already initialized\"}";

            Assert.AreEqual(expectedResponse, await response.Content.ReadAsStringAsync());
        }
        [TestMethod]
        public async Task Initialize_NotOwner_ReturnsError()
        {
            var factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient();

            client.DefaultRequestHeaders.Add("Cookie", "sid=token:fakeToken:d33b406631524e31805528e304f7401f;user=testPlayer2;version=1.21.1");

            WorldCreateRequest request = new()
            {
                Name = "Testing",
                Description = "Created by unit tests"
            };

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("worlds/1/initialize", content);

            Assert.IsFalse(response.IsSuccessStatusCode);

            string expectedResponse = "You don't own this world";

            Assert.AreEqual(expectedResponse, await response.Content.ReadAsStringAsync());
        }
    }
}