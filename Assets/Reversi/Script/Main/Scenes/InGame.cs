using System.Collections.Generic;
using System.Linq;
using Pommel.Reversi.Components.InGame;
using UniRx;
using UnityEngine;
using Zenject;
using static Pommel.Reversi.Domain.InGame.ValueObjects;
using static Pommel.Reversi.Domain.InGame.ValueObjects.State;

namespace Pommel.Reversi.Scenes
{
    public interface IInGame
    {
    }

    public sealed class InGame : MonoBehaviour, IInGame
    {
        private readonly IList<IPiece> m_pieces = new List<IPiece>();

        [Inject]
        public void Construct(
            IStateAsObservableCreator observableCreator,
            IFactory<IStateAsObservableCreator, Point, IPiece> factory
            )
        {
            observableCreator.Create(this, state => state.InGame.IsStateChanged, state => state.InGame)
                .Where(inGame => inGame.State == Playing
                    && !m_pieces.Any())
                .Subscribe(inGame =>
                {
                    var stones = inGame.Board.Stones.Select(stone => factory.Create(observableCreator, stone.Point));

                    foreach (var stone in stones)
                    {
                        m_pieces.Add(stone);
                    }
                });
        }
    }
}