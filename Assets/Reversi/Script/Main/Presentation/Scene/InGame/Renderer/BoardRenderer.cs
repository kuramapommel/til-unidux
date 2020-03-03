using System;
using System.Collections.Generic;
using System.Linq;
using Pommel.Reversi.Presentation.Scene.InGame.UI;
using UniRx;

namespace Pommel.Reversi.Presentation.Scene.InGame.Renderer
{
    public static class BoardRenderer
    {
        public static IDisposable ApplySwitchStoneColorRendering(this Board board, IEnumerable<Stone> stones)
        {
           var disposable = Unidux
                .Subject
                .TakeUntilDisable(board)
                .StartWith(Unidux.State)
                .Subscribe(state =>
                {
                    foreach (var (stone, stoneState) in stones.Zip(
                        state.Stones.SelectMany(stateStones => stateStones),
                        (stone, stoneState) => (stone, stoneState)))
                    {
                        switch (stoneState.Color)
                        {
                            case StoneStateElement.State.None when stone.IsNone: continue;
                            case StoneStateElement.State.Black when stone.IsBlack: continue;
                            case StoneStateElement.State.White when stone.IsWhite: continue;

                            case StoneStateElement.State.None:
                                stone.None();
                                continue;

                            case StoneStateElement.State.Black:
                                stone.TurnBlack();
                                continue;

                            case StoneStateElement.State.White:
                                stone.TurnWhite();
                                continue;
                        }

                        throw new ArgumentOutOfRangeException();
                    }
                })
                .AddTo(board);

            return disposable;
        }
    }
}