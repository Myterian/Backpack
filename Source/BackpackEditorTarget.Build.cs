using Flax.Build;

public class BackpackEditorTarget : GameProjectEditorTarget
{
    /// <inheritdoc />
    public override void Init()
    {
        base.Init();

        // Reference the modules for editor
        Modules.Add(nameof(BackpackExamples));
        Modules.Add(nameof(Backpack));
    }
}
