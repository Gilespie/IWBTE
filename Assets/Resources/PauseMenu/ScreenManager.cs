using System.Collections.Generic;
using UnityEngine;

namespace PauseMenu
{
    internal interface IScreen
    {
        void Activate();
        void Deactivate();
        void Free();
    }

    internal sealed class PanelScreen : IScreen
    {
        private readonly GameObject panel;

        public PanelScreen(GameObject panel) => this.panel = panel;
        public void Activate() => panel.SetActive(true);
        public void Deactivate() => panel.SetActive(false);
        public void Free() => Deactivate();
    }

    internal sealed class ScreenManager
    {
        private readonly Stack<IScreen> screens = new();
        public IScreen Current => screens.TryPeek(out var screen) ? screen : null;

        public void Push(IScreen screen)
        {
            if (screens.Contains(screen)) return;
            Current?.Deactivate();
            screens.Push(screen);
            screen.Activate();
        }

        public void Pop()
        {
            if (screens.TryPop(out var screen)) screen.Free();
            Current?.Activate();
        }

        public void Clear()
        {
            while (screens.TryPop(out var screen)) screen.Free();
        }
    }
}
