using System;
using System.Collections.Generic;
using Pommel.Reversi.Infrastructure.Networking.Client;
using Pommel.Reversi.UseCase.InGame;
using UniRx;
using _State = Pommel.Reversi.Domain.InGame.State;

namespace Pommel.Reversi.Infrastructure.Service.InGame
{
    public sealed class LaidPieceMessageBroker : ILaidPieceMessageReciever, ILaidPieceMessagePublisher
    {
        private readonly IMessageReceiver m_messageReceiver;

        private readonly ISubject<ILaidPieceEvent> m_onResult = new Subject<ILaidPieceEvent>();

        private readonly ISubject<ILaidPieceEvent> m_onLaid = new Subject<ILaidPieceEvent>();

        private readonly ISubject<LayEvent> m_onLay = new Subject<LayEvent>();

        private readonly IDictionary<bool, IInGameClient> m_inGameClientContainer = new Dictionary<bool, IInGameClient>();

        public IObservable<ILaidPieceEvent> OnResult => m_onResult;

        public IObservable<ILaidPieceEvent> OnLaid => m_onLaid;

        public IObservable<LayEvent> OnLay => m_onLay;

        public LaidPieceMessageBroker(
            IMessageBroker messageReceiver,
            IInGameClientFactory inGameClientFatory
            )
        {
            m_messageReceiver = messageReceiver;

            // todo dispose
            m_messageReceiver.Receive<ILaidPieceEvent>()
                .Where(message => message.Game.State == _State.GameSet)
                .Subscribe(m_onResult.OnNext);

            // todo dispose
            m_messageReceiver.Receive<ILaidPieceEvent>()
                .Subscribe(m_onLaid.OnNext);

            var client = inGameClientFatory.Create(
                (string gameId, int x, int y) => m_onLay.OnNext(new LayEvent(gameId, x, y)));
            m_inGameClientContainer.Add(true, client);
        }

        public void Join()
        {
            UnityEngine.Debug.Log("call join");
            if (!m_inGameClientContainer.TryGetValue(true, out var client)) return;
            _ = client.JoinAsync();
        }

        public void Lay(string gameId, int x, int y)
        {
            UnityEngine.Debug.Log("call Lay");
            if (!m_inGameClientContainer.TryGetValue(true, out var client)) return;
            _ = client.LayAsync(gameId, x, y);
        }
    }
}