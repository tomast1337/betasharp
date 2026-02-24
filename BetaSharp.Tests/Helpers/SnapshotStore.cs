using System.Text.Json;
using BetaSharp.Util.Maths;

namespace BetaSharp.Tests.Helpers;

/// <summary>
/// Saves and loads path-waypoint snapshots as JSON files.
///
/// <b>Snapshot lifecycle</b>
/// <list type="number">
///   <item>First test run — snapshot does not exist yet →
///         <see cref="LoadOrSeed"/> calls the factory (current PathFinder output),
///         writes the file into the project source folder, and returns the value.
///         The test trivially passes (comparing result with itself).
///         Commit the generated files to source control.</item>
///   <item>Subsequent runs — snapshot exists →
///         <see cref="LoadOrSeed"/> loads the committed "golden" values.
///         Any regression in PathFinder will now cause assertion failures.</item>
/// </list>
///
/// Files are written to <c>BetaSharp.Tests/Snapshots/</c> (three levels above the
/// binary output directory).  The .csproj is expected to copy them to the output via
/// <c>&lt;None Include="Snapshots\*.json" CopyToOutputDirectory="PreserveNewest" /&gt;</c>.
/// </summary>
internal static class SnapshotStore
{
    // Output directory: where the test binary runs (Snapshots/ subfolder)
    private static string OutputSnapshotDir =>
        Path.Combine(AppContext.BaseDirectory, "Snapshots");

    // Source directory: BetaSharp.Tests/Snapshots/ (three levels up from bin/Debug/net10.0)
    private static string SourceSnapshotDir =>
        Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Snapshots"));

    /// <summary>
    /// Loads the snapshot for <paramref name="name"/>, or — if it does not exist yet —
    /// calls <paramref name="factory"/>, saves the result as the new golden value, and
    /// returns it.
    /// </summary>
    public static List<Vec3D>? LoadOrSeed(string name, Func<List<Vec3D>?> factory)
    {
        string outputPath = Path.Combine(OutputSnapshotDir, $"{name}.json");
        string sourcePath = Path.Combine(SourceSnapshotDir, $"{name}.json");

        if (File.Exists(outputPath))
            return Deserialize(File.ReadAllText(outputPath));

        if (File.Exists(sourcePath))
            return Deserialize(File.ReadAllText(sourcePath));

        // No snapshot yet — seed from current implementation
        var waypoints = factory();
        Save(name, waypoints);
        return waypoints;
    }

    /// <summary>
    /// Persists <paramref name="waypoints"/> to the <b>source</b> Snapshots folder so the
    /// file is immediately tracked by version control.  Also copies it to the output
    /// folder for use in the current test run.
    /// </summary>
    public static void Save(string name, List<Vec3D>? waypoints)
    {
        var dto = waypoints?.Select(v => new WaypointDto(v.x, v.y, v.z)).ToList();
        string json = JsonSerializer.Serialize(dto, new JsonSerializerOptions { WriteIndented = true });

        Directory.CreateDirectory(SourceSnapshotDir);
        File.WriteAllText(Path.Combine(SourceSnapshotDir, $"{name}.json"), json);

        Directory.CreateDirectory(OutputSnapshotDir);
        File.WriteAllText(Path.Combine(OutputSnapshotDir, $"{name}.json"), json);

        Console.WriteLine($"[SnapshotStore] Saved {name}.json → {SourceSnapshotDir}");
    }

    private static List<Vec3D>? Deserialize(string json)
    {
        var dto = JsonSerializer.Deserialize<List<WaypointDto>?>(json);
        return dto?.Select(d => new Vec3D(d.X, d.Y, d.Z)).ToList();
    }

    private record WaypointDto(double X, double Y, double Z);
}
