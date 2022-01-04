using ProtoBuf;
using Vintagestory.API.MathTools;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace ApacheTech.VintageMods.CampaignCartographer.Features.CentreMap.Packets
{
    [ProtoContract]
    public class PlayerSpawnPositionDto
    {
        [ProtoMember(1)]
        public BlockPos SpawnPosition { get; set; }

        public PlayerSpawnPositionDto()
        {
            SpawnPosition = new BlockPos();
        }

        public PlayerSpawnPositionDto(BlockPos spawnPosition)
        {
            SpawnPosition = spawnPosition;
        }
    }
}