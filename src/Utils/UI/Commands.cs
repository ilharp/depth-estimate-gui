using System.Reactive;
using Avalonia.Controls;
using ReactiveUI;

namespace DepthEstimateGui.Utils.UI
{
    public static class Commands
    {
        public static ReactiveCommand<Button, Unit> OpenContextMenuCommand =>
            ReactiveCommand.Create<Button>(button =>
            {
                if (button?.ContextMenu is null)
                    return;
                ContextMenu menu = button.ContextMenu;
                menu.PlacementMode = PlacementMode.Bottom;
                menu.PlacementTarget = button;
                menu.Open();
            });
    }
}
