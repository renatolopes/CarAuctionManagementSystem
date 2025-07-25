
namespace CarAuctionManagementSystem.Api.IntegrationTests.Utilities;
public static class Queries
{
    public const string DeleteAllRegisters = @"
        TRUNCATE TABLE bid CASCADE;
        TRUNCATE TABLE auction CASCADE;
        TRUNCATE TABLE vehicle CASCADE";
}
