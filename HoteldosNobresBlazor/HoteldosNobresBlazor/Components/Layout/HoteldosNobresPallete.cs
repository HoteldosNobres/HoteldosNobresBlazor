using MudBlazor;
using MudBlazor.Utilities;

namespace HoteldosNobresBlazor.Layout;

public sealed class HoteldosNobresPallete : PaletteLight
{
    private HoteldosNobresPallete()
    {
        Primary = new MudColor("#D2154E");
        Secondary = new MudColor("#F6AD31");
        Tertiary = new MudColor("#8AE491");
    }

    public static HoteldosNobresPallete CreatePallete => new();
}
