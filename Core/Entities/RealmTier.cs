namespace Core.Entities;

public class RealmTier
{
    public int Id { get; set; }
    public int RenderMinimum { get; set; }
    public int RenderMaximum { get; set; }
    public int RenderDefault { get; set; }
    public int SimulationMinimum { get; set; }
    public int SimulationMaximum { get; set; }
    public int SimulationDefault { get; set; }
}