using System;
using System.Threading.Tasks;
using VotingTool.Features.Voting;

namespace VotingTool.Pages
{
    public partial class Index
    {
        private string _question;
        private int _up;
        private int _down;
        private bool _voted;
        private Guid _clientId;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            _question = VotingService.Question;

            var currentResults = VotingService.GetCurrentState();
            _up = currentResults.ThumbsUp;
            _down = currentResults.ThumbsDown;
            _question = VotingService.Question;
            VotingService.VoteChangedEventHandler += OnVoteHasChanged;
            VotingService.ResetEventHandler += OnReset;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await SetClientIp();
        }

        private async Task SetClientIp()
        {
            const string identifier = "VotingClientID";
            if (await LocalStorage.ContainKeyAsync(identifier))
                _clientId = await LocalStorage.GetItemAsync<Guid>(identifier);
            else
            {
                _clientId = Guid.NewGuid();
                await LocalStorage.SetItemAsync(identifier, _clientId);
            }

            _voted = VotingService.HasAlreadyVoted(_clientId);
            StateHasChanged();
        }

        private void OnReset(object sender, EventArgs e)
        {
            _voted = false;
            _question = VotingService.Question;
            InvokeAsync(StateHasChanged);
        }

        private void OnVoteHasChanged(object sender, VotingResultEventArgs e)
        {
            _up = e.ThumbsUp;
            _down = e.ThumbsDown;
            InvokeAsync(StateHasChanged);
        }

        private void VoteUp()
        {
            if (_voted) return;
            VotingService.VoteUp(_clientId);
            _voted = true;
        }

        private void VoteDown()
        {
            if (_voted) return;
            VotingService.VoteDown(_clientId);
            _voted = true;
        }

        private void Reset()
        {
            VotingService.Reset();
        }

        private void UpdateQuestion()
        {
            VotingService.UpdateQuestion(_question);
        }

        public void Dispose()
        {
            VotingService.VoteChangedEventHandler -= OnVoteHasChanged;
            VotingService.ResetEventHandler -= OnReset;
            Console.WriteLine("removed event handlers");
        }
    }
}
