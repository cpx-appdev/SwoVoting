using System;
using System.Collections.Generic;
using System.Threading;

namespace VotingTool.Features.Voting
{
    public class VotingService
    {
        private int _thumbUpVote;
        private int _thumbDownVote;
        public string Question { get; private set; }

        public event EventHandler<VotingResultEventArgs> VoteChangedEventHandler;
        public event EventHandler ResetEventHandler;

        private readonly List<Guid> _votedClients = new List<Guid>();

        public bool HasAlreadyVoted(Guid clientId)
        {
            return _votedClients.Contains(clientId);
        }

        public void VoteUp(Guid clientId)
        {
            if (HasAlreadyVoted(clientId))
                return;
            _votedClients.Add(clientId);

            Interlocked.Increment(ref _thumbUpVote);
            OnVoteChanged();
        }

        public void VoteDown(Guid clientId)
        {
            if (HasAlreadyVoted(clientId))
                return;
            _votedClients.Add(clientId);

            Interlocked.Increment(ref _thumbDownVote);
            OnVoteChanged();
        }

        public void Reset()
        {
            Monitor.Enter(this);
            _thumbUpVote = 0;
            _thumbDownVote = 0;
            _votedClients.Clear();
            Monitor.Exit(this);

            OnVoteChanged();
            OnReset();
        }

        public void UpdateQuestion(string newQuestion)
        {
            Question = newQuestion;
            Reset();
        }

        public VotingResultEventArgs GetCurrentState()
        {
            return new VotingResultEventArgs(_thumbUpVote, _thumbDownVote);
        }

        private void OnVoteChanged()
        {
            var handler = VoteChangedEventHandler;

            handler?.Invoke(this, new VotingResultEventArgs(_thumbUpVote, _thumbDownVote));
        }

        private void OnReset()
        {
            var handler = ResetEventHandler;
            handler?.Invoke(this, EventArgs.Empty);
        }
    }

    public class VotingResultEventArgs : EventArgs
    {
        public VotingResultEventArgs(int thumbsUp, int thumbsDown)
        {
            ThumbsUp = thumbsUp;
            ThumbsDown = thumbsDown;
        }
        public int ThumbsUp { get; set; }
        public int ThumbsDown { get; set; }
    }
}
