using System;

namespace Pommel.Reversi.Reducks.Title
{
    public static class Selectors
    {
        public static Func<IState, bool> GetDisplayGameModal = state => state.Title.IsDisplayGameStartModal;
    }
}