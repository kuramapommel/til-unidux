using System;
using System.Collections.Generic;
using Pommel.Reversi.Domain.InGame;
using Pommel.Reversi.Presentation.Scene.InGame.State;
using UniRx;
using UniRx.Async;
using UnityEngine;
using Zenject;
using _Stone = Pommel.Reversi.Domain.InGame.Stone;

namespace Pommel.Reversi.Presentation.Scene.InGame.View
{
    public interface IGameBoard
    {
        void Instantiate(IGameBoardState state, Func<Point, UniTask<IEnumerable<_Stone>>> putAsync);
    }

    [RequireComponent(typeof(RectTransform))]
    public sealed class GameBoard : MonoBehaviour, IGameBoard
    {
        private IFactory<IStoneState, IStone> m_stoneFactory;

        private IList<IStone> Stones { get; } = new List<IStone>();

        [Inject]
        public void Construct(IFactory<IStoneState, IStone> stoneFactory)
        {
            m_stoneFactory = stoneFactory;
        }

        public void Instantiate(IGameBoardState state, Func<Point, UniTask<IEnumerable<_Stone>>> putAsync)
        {
            foreach (var stoneState in state.Stones)
            {
                var stone = m_stoneFactory.Create(stoneState);
                stone.OnPutAsObservable()
                    .SelectMany(_ => putAsync(stoneState.Point).ToObservable())
                    .Subscribe(state.Refresh);
                Stones.Add(stone);
            }
        }
    }
}