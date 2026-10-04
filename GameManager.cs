using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Text.RegularExpressions;
using FileAccess = Godot.FileAccess;

namespace Blackjack;

public partial class GameManager : Control
{
    [Export] public GameSettings Settings { get; set; } = new();

    private AudioStreamPlayer _cardSound = null!;
    private AudioStreamPlayer _chipBetSound = null!;
    private AudioStreamPlayer _chipWinSound = null!;
    private AudioStreamPlayer _shuffleSound = null!;
    private AudioStreamPlayer _musicPlayer = null!;

    private Shoe _shoe = null!;
    private Player _player = null!;
    private Dealer _dealer = null!;
    private PackedScene _cardScene = null!;

    private Label _bankrollLabel = null!;
    private Label _shoeLabel = null!;
    private Label _messageLabel = null!;
    private Label _dealerLabel = null!;
    private Label _playerLabel = null!;
    private Label _betLabel = null!;
    private HBoxContainer _dealerCards = null!;
    private HBoxContainer _playerCards = null!;
    private HBoxContainer _betButtons = null!;
    private HBoxContainer _actionButtons = null!;
    private Button _hitButton = null!;
    private Button _standButton = null!;
    private Button _doubleButton = null!;
    private Button _splitButton = null!;
    private Button _restartButton = null!;
    private HBoxContainer _insurancePanel = null!;
    private Button _insuranceYes = null!;
    private Button _insuranceNo = null!;

    private Control _startScreen = null!;
    private Control _houseRulesScreen = null!;
    private Control _optionsScreen = null!;
	private Control _creditsScreen = null!;
	private Control _legalScreen = null!;
	private Button _creditsButton = null!;
	private Button _legalButton = null!;
	private Button _creditsBackButton = null!;
	private Button _legalBackButton = null!;
    private Button _startButton = null!;
    private Button _houseRulesButton = null!;
    private Button _optionsButton = null!;
    private Button _rulesBackButton = null!;
    private Button _sfxButton = null!;
    private Button _musicButton = null!;
    private Button _optionsBackButton = null!;
	private Button _decksButton = null!;
	private Button _hitSoft17Button = null!;

	private Label _licenseText = null!;

	private Label _rulesText = null!;

    private MarginContainer _mainLayout = null!;

    private CardVisual? _holeCardVisual;

    private readonly List<HBoxContainer> _handCardRows = new();
    private readonly List<Label> _handLabels = new();
    private int _activeHandIndex;

    private int _bankrollAtRoundStart;

    private const float FlipShrinkSeconds = 0.15f;
    private const float FlipExpandSeconds = 0.15f;
    private const float BankrollFlashSeconds = 0.6f;
    private const float CardSlideSeconds = 0.25f;
    private const float MusicVolumeDb = -14f;
    private const float MutedDb = -80f;
    private const float ShuffleFadeSeconds = 0.3f;
    private const float ShuffleTotalSeconds = 3.0f;

    private static readonly Vector2 CardSlideOffset = new(200, -400);

    public override void _Ready()
    {
        _cardScene = ResourceLoader.Load<PackedScene>("res://Card.tscn");

        _cardSound = MakeSoundPlayer("res://sounds/card_deal.ogg", maxPolyphony: 4);
        _chipBetSound = MakeSoundPlayer("res://sounds/chips_bet.ogg", maxPolyphony: 2);
        _chipWinSound = MakeSoundPlayer("res://sounds/chips_win.ogg", maxPolyphony: 4);
        _shuffleSound = MakeSoundPlayer("res://sounds/shuffle.ogg", maxPolyphony: 1);

        StartMusic("res://sounds/bjmusic.ogg");


		// Main Layout
        _bankrollLabel = GetNode<Label>("MainLayout/VBox/TopBar/BankrollLabel");
        _shoeLabel = GetNode<Label>("MainLayout/VBox/TopBar/ShoeLabel");
        _dealerLabel = GetNode<Label>("MainLayout/VBox/DealerLabel");
        _dealerCards = GetNode<HBoxContainer>("MainLayout/VBox/DealerCards");
        _betLabel = GetNode<Label>("MainLayout/VBox/BetLabel");
        _playerLabel = GetNode<Label>("MainLayout/VBox/PlayerLabel");
        _messageLabel = GetNode<Label>("MessageLayer/MessageLabel");
        _playerCards = GetNode<HBoxContainer>("MainLayout/VBox/PlayerCards");
        
		// BottomBar nodes
		_betButtons    = GetNode<HBoxContainer>("BottomBar/BottomVBox/BetButtons");
		_actionButtons = GetNode<HBoxContainer>("BottomBar/BottomVBox/ActionButtons");
		_hitButton     = GetNode<Button>("BottomBar/BottomVBox/ActionButtons/HitButton");
		_standButton   = GetNode<Button>("BottomBar/BottomVBox/ActionButtons/StandButton");
		_doubleButton  = GetNode<Button>("BottomBar/BottomVBox/ActionButtons/DoubleButton");
		_splitButton   = GetNode<Button>("BottomBar/BottomVBox/ActionButtons/SplitButton");
		_restartButton = GetNode<Button>("BottomBar/BottomVBox/RestartButton");
		_insurancePanel= GetNode<HBoxContainer>("BottomBar/BottomVBox/InsurancePanel");
		_insuranceYes  = GetNode<Button>("BottomBar/BottomVBox/InsurancePanel/InsuranceYes");
		_insuranceNo   = GetNode<Button>("BottomBar/BottomVBox/InsurancePanel/InsuranceNo");

        _startScreen = GetNode<Control>("StartScreen");
        _houseRulesScreen = GetNode<Control>("HouseRulesScreen");
        _optionsScreen = GetNode<Control>("OptionsScreen");
        _startButton = GetNode<Button>("StartScreen/CenterContainer/Menu/StartButton");
        _houseRulesButton = GetNode<Button>("StartScreen/CenterContainer/Menu/RulesButton");
        _optionsButton = GetNode<Button>("StartScreen/CenterContainer/Menu/OptionsButton");
        _rulesBackButton = GetNode<Button>("HouseRulesScreen/MarginContainer/Content/BackButton");
        _sfxButton = GetNode<Button>("OptionsScreen/CenterContainer/VBox/SfxButton");
        _musicButton = GetNode<Button>("OptionsScreen/CenterContainer/VBox/MusicButton");
        _optionsBackButton = GetNode<Button>("OptionsScreen/CenterContainer/VBox/OptionsBackButton");
		_creditsScreen = GetNode<Control>("CreditsScreen");
		_legalScreen = GetNode<Control>("LegalScreen");
		_creditsButton = GetNode<Button>("StartScreen/CenterContainer/Menu/CreditsButton");
		_legalButton = GetNode<Button>("CreditsScreen/CenterContainer/VBox/LegalButton");
		_creditsBackButton = GetNode<Button>("CreditsScreen/CenterContainer/VBox/CreditsBackButton");
		_legalBackButton = GetNode<Button>("LegalScreen/MarginContainer/VBox/LegalBackButton");
		_decksButton = GetNode<Button>("OptionsScreen/CenterContainer/VBox/DecksButton");
		_hitSoft17Button = GetNode<Button>("OptionsScreen/CenterContainer/VBox/HitSoft17Button");

        _mainLayout = GetNode<MarginContainer>("MainLayout");

		_licenseText = GetNode<Label>("LegalScreen/MarginContainer/VBox/ScrollContainer/LicenseText");

		_rulesText = GetNode<Label>("HouseRulesScreen/MarginContainer/Content/ScrollContainer/RulesText");


        GetNode<Button>("BottomBar/BottomVBox/BetButtons/Bet10").Pressed  += async () => await StartRoundAsync(10);
		GetNode<Button>("BottomBar/BottomVBox/BetButtons/Bet20").Pressed  += async () => await StartRoundAsync(20);
		GetNode<Button>("BottomBar/BottomVBox/BetButtons/Bet50").Pressed  += async () => await StartRoundAsync(50);
		GetNode<Button>("BottomBar/BottomVBox/BetButtons/Bet100").Pressed += async () => await StartRoundAsync(100);

		

        _hitButton.Pressed += async () => await OnHitAsync();
        _standButton.Pressed += async () => await OnStandAsync();
        _doubleButton.Pressed += async () => await OnDoubleAsync();
        _splitButton.Pressed += async () => await OnSplitAsync();
        _restartButton.Pressed += OnRestartPressed;
        _insuranceYes.Pressed += async () => await OnInsuranceYesAsync();
        _insuranceNo.Pressed += async () => await OnInsuranceNoAsync();

        _startButton.Pressed += OnStartPressed;
        _houseRulesButton.Pressed += OnHouseRulesPressed;
        _optionsButton.Pressed += OnOptionsPressed;
        _rulesBackButton.Pressed += OnRulesBackPressed;
        _sfxButton.Pressed += OnSfxToggled;
        _musicButton.Pressed += OnMusicToggled;
        _optionsBackButton.Pressed += OnOptionsBackPressed;
		_creditsButton.Pressed += OnCreditsPressed;
		_legalButton.Pressed += OnLegalPressed;
		_creditsBackButton.Pressed += OnCreditsBackPressed;
		_legalBackButton.Pressed += OnLegalBackPressed;
		_decksButton.Pressed += OnDecksPressed;
		_hitSoft17Button.Pressed += OnHitSoft17Toggled;

        _shoe = new Shoe(Settings.DeckCount);
        _shoe.Shuffle();
        _player = new Player("PLAYER", Settings.StartingBankroll);
		_dealer = new Dealer(Settings.HitSoft17);

		_licenseText.Text = LoadAllLicenses();

        ApplyAudioSettings();
        UpdateOptionsButtons();
		
        ShowStartScreen();
    }

	private static readonly string[] LicenseFiles =
	{
		"game_license.txt",
		"font_license.txt",
		"godot_license.txt",
		"godot_third_party.txt",
	};

	private static readonly int[] DeckOptions = { 1, 2, 4, 6, 8 };

    private PlayerHand ActiveHand => _player.Hands[_activeHandIndex];

    // ----- Sound helpers -----

    private AudioStreamPlayer MakeSoundPlayer(string path, int maxPolyphony)
    {
        var player = new AudioStreamPlayer();
        player.Stream = GD.Load<AudioStream>(path);
        player.MaxPolyphony = maxPolyphony;
        AddChild(player);
        return player;
    }

    private void StartMusic(string path)
    {
        var music = GD.Load<AudioStreamOggVorbis>(path);
        if (music == null)
        {
            GD.PrintErr($"Music file not found: {path}");
            return;
        }

        music.Loop = true;

        _musicPlayer = new AudioStreamPlayer();
        _musicPlayer.Stream = music;
        _musicPlayer.VolumeDb = MusicVolumeDb;
        AddChild(_musicPlayer);
        _musicPlayer.Play();
    }

    private void ApplyAudioSettings()
    {
        float sfxDb = Settings.SoundEnabled ? 0f : MutedDb;
        _cardSound.VolumeDb = sfxDb;
        _chipBetSound.VolumeDb = sfxDb;
        _chipWinSound.VolumeDb = sfxDb;
        _shuffleSound.VolumeDb = sfxDb;

        if (_musicPlayer != null)
        {
            _musicPlayer.VolumeDb = Settings.MusicEnabled ? MusicVolumeDb : MutedDb;
        }
    }

    private void UpdateOptionsButtons()
	{
		_sfxButton.Text = Settings.SoundEnabled ? "SOUND: ON" : "SOUND: OFF";
		_musicButton.Text = Settings.MusicEnabled ? "MUSIC: ON" : "MUSIC: OFF";
		_decksButton.Text = $"DECKS: {Settings.DeckCount}";
		_hitSoft17Button.Text = Settings.HitSoft17 ? "HIT SOFT 17: YES" : "HIT SOFT 17: NO";
	}

    private void PlayCardSound()
    {
        _cardSound.PitchScale = (float)GD.RandRange(0.95, 1.05);
        _cardSound.Play();
    }

    private void PlayChipBetSound()
    {
        _chipBetSound.PitchScale = (float)GD.RandRange(0.97, 1.03);
        _chipBetSound.Play();
    }

    private void PlayChipWinSound()
    {
        _chipWinSound.PitchScale = (float)GD.RandRange(0.97, 1.03);
        _chipWinSound.Play();
    }

    private void PlayShuffleSound()
    {
        _shuffleSound.Play();
    }

    // ----- Timing helpers -----

    private async Task Delay(double seconds)
    {
        await ToSignal(GetTree().CreateTimer(seconds), SceneTreeTimer.SignalName.Timeout);
    }

    private Card DrawCard()
    {
        Card card = _shoe.Draw();
        UpdateTopBar();
        return card;
    }

    // ----- Card visuals -----

    private async Task SlideCardInAsync(Control card)
    {
        card.Modulate = new Color(1, 1, 1, 0);

        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        Vector2 finalPos = card.Position;
        card.Position = finalPos + CardSlideOffset;

        PlayCardSound();

        Tween tween = card.CreateTween();
        tween.SetParallel(true);
        tween.TweenProperty(card, "position", finalPos, CardSlideSeconds);
        tween.TweenProperty(card, "modulate:a", 1.0f, CardSlideSeconds);
        await ToSignal(tween, Tween.SignalName.Finished);
    }

    private async Task<CardVisual> AddDealerCardVisualAsync(Card card, bool faceDown = false)
    {
        CardVisual visual = _cardScene.Instantiate<CardVisual>();
        _dealerCards.AddChild(visual);

        if (faceDown)
        {
            visual.SetFaceDown();
        }
        else
        {
            visual.SetCard(card);
        }

        await SlideCardInAsync(visual);
        return visual;
    }

    private async Task AppendPlayerCardVisualAsync(int handIndex, Card card)
    {
        HBoxContainer row = _handCardRows[handIndex];
        CardVisual visual = _cardScene.Instantiate<CardVisual>();
        row.AddChild(visual);
        visual.SetCard(card);
        await SlideCardInAsync(visual);
    }

    private void RenderHandCards(int handIndex)
    {
        HBoxContainer row = _handCardRows[handIndex];
        foreach (Node child in row.GetChildren())
        {
            child.QueueFree();
        }
        foreach (Card c in _player.Hands[handIndex].Hand.Cards)
        {
            CardVisual visual = _cardScene.Instantiate<CardVisual>();
            row.AddChild(visual);
            visual.SetCard(c);
        }
    }

    // ----- Bankroll flash -----

    private void FlashBankroll(Color flashColor)
    {
        _bankrollLabel.Modulate = flashColor;
        Tween tween = _bankrollLabel.CreateTween();
        tween.TweenProperty(_bankrollLabel, "modulate", new Color(1, 1, 1, 1), BankrollFlashSeconds);
    }

    // ----- Shuffle animation -----

    private async Task ShowShuffleMessageAsync()
    {
        _messageLabel.Text = "Shuffling...";
        _messageLabel.Modulate = new Color(1, 1, 1, 0);

        Tween fadeIn = _messageLabel.CreateTween();
        fadeIn.TweenProperty(_messageLabel, "modulate:a", 1.0f, ShuffleFadeSeconds);
        await ToSignal(fadeIn, Tween.SignalName.Finished);

        PlayShuffleSound();
        _shoe.Reshuffle();
        UpdateTopBar();

        await Delay(ShuffleTotalSeconds - 2 * ShuffleFadeSeconds);

        Tween fadeOut = _messageLabel.CreateTween();
        fadeOut.TweenProperty(_messageLabel, "modulate:a", 0.0f, ShuffleFadeSeconds);
        await ToSignal(fadeOut, Tween.SignalName.Finished);

        _messageLabel.Modulate = new Color(1, 1, 1, 1);
    }

    // ----- Screens -----

   private void ShowStartScreen()
	{
		ClearTableCards();
		_mainLayout.Visible = false;
		_startScreen.Visible = true;
		_houseRulesScreen.Visible = false;
		_optionsScreen.Visible = false;
		_creditsScreen.Visible = false;   
		_legalScreen.Visible = false;     
		_betButtons.Visible = false;
		_actionButtons.Visible = false;
		_restartButton.Visible = false;
		_insurancePanel.Visible = false;
		UpdateTopBar();
	}

    private void OnStartPressed()
    {
        _startScreen.Visible = false;
        _mainLayout.Visible = true;
        ShowBetButtons();
    }

    private void OnHouseRulesPressed()
	{
		UpdateRulesText();
		_startScreen.Visible = false;
		_houseRulesScreen.Visible = true;
	}

    private void OnRulesBackPressed()
    {
        _houseRulesScreen.Visible = false;
        _startScreen.Visible = true;
    }

    private void OnOptionsPressed()
    {
        UpdateOptionsButtons();
        _startScreen.Visible = false;
        _optionsScreen.Visible = true;
    }

    private void OnOptionsBackPressed()
    {
        _optionsScreen.Visible = false;
        _startScreen.Visible = true;
    }

	private void OnCreditsPressed()
	{
		_startScreen.Visible = false;
		_creditsScreen.Visible = true;
	}

	private void OnCreditsBackPressed()
	{
		_creditsScreen.Visible = false;
		_startScreen.Visible = true;
	}

	private void OnLegalPressed()
	{
		_creditsScreen.Visible = false;
		_legalScreen.Visible = true;
	}

	private void OnLegalBackPressed()
	{
		_legalScreen.Visible = false;
		_creditsScreen.Visible = true;
	}

    private void OnSfxToggled()
    {
        Settings.SoundEnabled = !Settings.SoundEnabled;
        ApplyAudioSettings();
        UpdateOptionsButtons();
    }

    private void OnMusicToggled()
    {
        Settings.MusicEnabled = !Settings.MusicEnabled;
        ApplyAudioSettings();
        UpdateOptionsButtons();
    }

	private void OnDecksPressed()
	{
		int currentIndex = System.Array.IndexOf(DeckOptions, Settings.DeckCount);
		int nextIndex = currentIndex < 0 ? 0 : (currentIndex + 1) % DeckOptions.Length;
		Settings.DeckCount = DeckOptions[nextIndex];

		// Build a fresh shoe with the new deck count.
		_shoe = new Shoe(Settings.DeckCount);
		_shoe.Shuffle();
		UpdateTopBar();

		UpdateOptionsButtons();
	}

	private void OnHitSoft17Toggled()
	{
		Settings.HitSoft17 = !Settings.HitSoft17;
		_dealer = new Dealer(Settings.HitSoft17);
		UpdateOptionsButtons();
	}

    // ----- UI state -----

    /// <summary>
    /// Prepares the betting state, showing the shuffle animation first if
    /// the shoe has passed its cut card. Called at the end of every round.
    /// </summary>
    private async Task PrepareForBettingAsync()
    {
        if (_shoe.NeedsReshuffle)
        {
            ClearTableCards();
            await ShowShuffleMessageAsync();
        }
        ShowBetButtons();
    }

    /// <summary>
    /// Shows the betting UI. Assumes the shoe is fresh or has already been
    /// reshuffled — see PrepareForBettingAsync.
    /// </summary>
    private void ShowBetButtons()
    {
        ClearTableCards();
        _betButtons.Visible = true;
        _actionButtons.Visible = false;
        _restartButton.Visible = false;
        _insurancePanel.Visible = false;
        _messageLabel.Text = "Place your bet";
        UpdateTopBar();
    }

    private void ShowActionButtons()
    {
        _betButtons.Visible = false;
        _actionButtons.Visible = true;
        UpdateButtons();
        UpdateHandLabels();
    }

    private void UpdateButtons()
    {
        PlayerHand ph = ActiveHand;
        _hitButton.Disabled = ph.IsFinished;
        _standButton.Disabled = ph.IsFinished;
        _doubleButton.Disabled = !_player.CanDoubleFor(ph);
        _splitButton.Disabled = !_player.CanSplitHand(ph);
    }

    private void UpdateTopBar()
    {
        _bankrollLabel.Text = $"Bankroll: ${_player.Bankroll:N0}";
        _shoeLabel.Text = $"Shoe: {_shoe.Count} / {_shoe.TotalCards}";
    }

    private void UpdatePlayerLabel()
    {
        if (_player.Hands.Count == 0)
        {
            _playerLabel.Text = "PLAYER";
            return;
        }

        if (_player.Hands.Count == 1)
        {
            _playerLabel.Text = $"PLAYER: {_player.Hands[0].Hand.Total}";
        }
        else
        {
            var parts = new List<string>();
            for (int i = 0; i < _player.Hands.Count; i++)
            {
                parts.Add($"{_player.Hands[i].Hand.Total}");
            }
            _playerLabel.Text = "PLAYER: " + string.Join("   ", parts);
        }
    }

    private void UpdateBetLabel()
	{
		if (_player.Hands.Count == 0)
		{
			_betLabel.Text = "";
			return;
		}

		if (_player.Hands.Count == 1)
		{
			_betLabel.Text = $"Bet: ${_player.Hands[0].Bet:N0}";
		}
		else
		{
			var parts = new List<string>();
			for (int i = 0; i < _player.Hands.Count; i++)
			{
				parts.Add($"${_player.Hands[i].Bet:N0}");
			}
			_betLabel.Text = "Bets: " + string.Join(" + ", parts);
		}
	}

    private void RebuildHandsUI()
    {
        foreach (Node child in _playerCards.GetChildren())
        {
            child.QueueFree();
        }
        _handCardRows.Clear();
        _handLabels.Clear();

        for (int i = 0; i < _player.Hands.Count; i++)
        {
            var panel = new VBoxContainer();
            panel.AddThemeConstantOverride("separation", 4);

            var label = new Label();
            label.HorizontalAlignment = HorizontalAlignment.Center;
            panel.AddChild(label);
            _handLabels.Add(label);

            var row = new HBoxContainer();
            row.Alignment = BoxContainer.AlignmentMode.Center;
            row.AddThemeConstantOverride("separation", -20);
            panel.AddChild(row);
            _handCardRows.Add(row);

            _playerCards.AddChild(panel);
        }
    }

    private void UpdateHandLabels()
    {
        bool split = _player.Hands.Count > 1;
        for (int i = 0; i < _player.Hands.Count; i++)
        {
            _handLabels[i].Visible = split;
            if (!split) continue;

            string marker = (i == _activeHandIndex && !_player.Hands[i].IsFinished)
                ? " ◄" : "";
            _handLabels[i].Text = $"Hand {i + 1}: {_player.Hands[i].Hand.Total}{marker}";
        }
    }

    // ----- Round flow -----

    private async Task StartRoundAsync(int bet)
    {
        if (!_player.CanAfford(bet))
        {
            _messageLabel.Text = "Not enough chips";
            return;
        }

        PlayChipBetSound();

        _bankrollAtRoundStart = _player.Bankroll;

        _betButtons.Visible = false;
        _insurancePanel.Visible = false;

        _player.StartRound(bet);
        _activeHandIndex = 0;

        _dealer.Hand.Clear();
        _dealer.HideHoleCard();

        foreach (Node child in _dealerCards.GetChildren()) child.QueueFree();
        foreach (Node child in _playerCards.GetChildren()) child.QueueFree();

        RebuildHandsUI();
        UpdateTopBar();
        UpdatePlayerLabel();
        UpdateBetLabel();
        UpdateHandLabels();

        await Delay(0.5);

        await DealInitialCardsAsync();
        await Delay(0.3);

        if (_dealer.UpCardIsAce && _player.CanAffordInsurance(_player.Hands[0].Bet))
        {
            _actionButtons.Visible = false;
            _insurancePanel.Visible = true;
            _messageLabel.Text = "Dealer shows an Ace";
            return;
        }

        await ContinueAfterInsuranceAsync(0);
    }

    private async Task DealInitialCardsAsync()
    {
        PlayerHand ph = _player.Hands[0];

        Card c1 = DrawCard();
        ph.Hand.Add(c1);
        await AppendPlayerCardVisualAsync(0, c1);
        UpdatePlayerLabel();
        UpdateHandLabels();
        await Delay(0.15);

        Card d1 = DrawCard();
        _dealer.Hand.Add(d1);
        await AddDealerCardVisualAsync(d1);
        await Delay(0.15);

        Card c2 = DrawCard();
        ph.Hand.Add(c2);
        await AppendPlayerCardVisualAsync(0, c2);
        UpdatePlayerLabel();
        UpdateHandLabels();
        await Delay(0.15);

        Card d2 = DrawCard();
        _dealer.Hand.Add(d2);
        _holeCardVisual = await AddDealerCardVisualAsync(d2, faceDown: true);
    }

    private async Task RevealHoleCardAsync()
    {
        if (_holeCardVisual == null) return;

        _holeCardVisual.PivotOffset = new Vector2(40, 60);

        Tween shrink = _holeCardVisual.CreateTween();
        shrink.TweenProperty(_holeCardVisual, "scale:x", 0.0f, FlipShrinkSeconds);
        await ToSignal(shrink, Tween.SignalName.Finished);

        _dealer.RevealHoleCard();
        _holeCardVisual.SetCard(_dealer.Hand.Cards[1]);
        PlayCardSound();

        Tween expand = _holeCardVisual.CreateTween();
        expand.TweenProperty(_holeCardVisual, "scale:x", 1.0f, FlipExpandSeconds);
        await ToSignal(expand, Tween.SignalName.Finished);
    }

    // ----- Player actions -----

    private async Task OnHitAsync()
    {
        PlayerHand ph = ActiveHand;
        Card drawn = DrawCard();
        ph.Hand.Add(drawn);
        await AppendPlayerCardVisualAsync(_activeHandIndex, drawn);
        UpdatePlayerLabel();
        UpdateHandLabels();

        if (ph.Hand.IsBust)
        {
            ph.IsFinished = true;
            await AdvanceToNextHandAsync();
            return;
        }

        UpdateButtons();
    }

    private async Task OnStandAsync()
    {
        ActiveHand.IsFinished = true;
        await AdvanceToNextHandAsync();
    }

    private async Task OnDoubleAsync()
    {
        PlayerHand ph = ActiveHand;
        _player.DoubleBet(ph);
        PlayChipBetSound();

        await Delay(0.5);

        Card drawn = DrawCard();
        ph.Hand.Add(drawn);
        ph.IsFinished = true;

        await AppendPlayerCardVisualAsync(_activeHandIndex, drawn);
        UpdatePlayerLabel();
        UpdateBetLabel();
        UpdateHandLabels();
        UpdateTopBar();

        await AdvanceToNextHandAsync();
    }

    private async Task OnSplitAsync()
    {
        PlayerHand ph = ActiveHand;
        PlayChipBetSound();

        await Delay(0.5);

        Card c1 = DrawCard();
        PlayerHand newHand = _player.Split(ph, c1);

        if (newHand.IsFromAceSplit)
        {
            await Delay(0.3);
            Card c2 = DrawCard();
            newHand.Hand.Add(c2);
        }

        RebuildHandsUI();
        for (int i = 0; i < _player.Hands.Count; i++)
        {
            RenderHandCards(i);
        }
        UpdateTopBar();
        UpdatePlayerLabel();
        UpdateBetLabel();
        UpdateHandLabels();

        if (ph.IsFinished)
        {
            await AdvanceToNextHandAsync();
            return;
        }

        UpdateButtons();
    }

    private async Task AdvanceToNextHandAsync()
    {
        for (int i = _activeHandIndex + 1; i < _player.Hands.Count; i++)
        {
            if (!_player.Hands[i].IsFinished)
            {
                _activeHandIndex = i;
                PlayerHand ph = _player.Hands[i];

                if (ph.Hand.Cards.Count == 1)
                {
                    await Delay(0.3);
                    Card c = DrawCard();
                    ph.Hand.Add(c);
                    await AppendPlayerCardVisualAsync(i, c);
                    UpdatePlayerLabel();
                }

                UpdateButtons();
                UpdateHandLabels();
                return;
            }
        }

        await EndRoundAsync();
    }

    // ----- Settlement -----

    private async Task EndRoundAsync()
    {
        _actionButtons.Visible = false;
        await Delay(0.4);

        bool anyoneAlive = false;
        foreach (PlayerHand ph in _player.Hands)
        {
            if (!ph.Hand.IsBust) { anyoneAlive = true; break; }
        }

        await RevealHoleCardAsync();
        await Delay(0.4);

        if (anyoneAlive)
        {
            while (_dealer.MustHit)
            {
                Card drawn = DrawCard();
                _dealer.Hand.Add(drawn);
                await AddDealerCardVisualAsync(drawn);
                await Delay(0.3);
            }
            await Delay(0.4);
        }

        _dealerLabel.Text = $"DEALER: {_dealer.Hand.Total}";
        int dealerTotal = _dealer.Hand.Total;

        var results = new List<string>();
        bool split = _player.Hands.Count > 1;
        bool anyWin = false;

        for (int i = 0; i < _player.Hands.Count; i++)
        {
            PlayerHand ph = _player.Hands[i];
            string prefix = split ? $"Hand {i + 1}: " : "";

            if (ph.Hand.IsBust)
            {
                results.Add($"{prefix}Bust");
                continue;
            }

            if (_dealer.IsBust || ph.Hand.Total > dealerTotal)
            {
                _player.WinBet(ph);
                results.Add($"{prefix}Win!");
                anyWin = true;
            }
            else if (ph.Hand.Total < dealerTotal)
            {
                results.Add($"{prefix}Lose");
            }
            else
            {
                _player.PushBet(ph);
                results.Add($"{prefix}Push");
            }
        }

        _messageLabel.Text = string.Join("   ", results);
        UpdateTopBar();

        if (anyWin)
        {
            await Delay(0.5);
            PlayChipWinSound();
        }

        FinishRound();
    }

    private void FinishRound()
    {
        _actionButtons.Visible = false;
        UpdatePlayerLabel();
        UpdateHandLabels();
        UpdateTopBar();

        int delta = _player.Bankroll - _bankrollAtRoundStart;
        if (delta > 0)
        {
            FlashBankroll(new Color(0.3f, 1.0f, 0.3f));
        }
        else if (delta < 0)
        {
            FlashBankroll(new Color(1.0f, 0.3f, 0.3f));
        }

        if (_player.Bankroll < Settings.MinimumBet)
        {
            _messageLabel.Text += _player.Bankroll == 0
                ? "  |  You're out of chips!"
                : "  |  Not enough chips to place a bet";
            _betButtons.Visible = false;
            _actionButtons.Visible = false;
            _restartButton.Visible = true;
            return;
        }

        var timer = GetTree().CreateTimer(3);
        timer.Timeout += async () => await PrepareForBettingAsync();
    }

    private void ClearTableCards()
    {
        foreach (Node child in _dealerCards.GetChildren()) child.QueueFree();
        foreach (Node child in _playerCards.GetChildren()) child.QueueFree();

        _handCardRows.Clear();
        _handLabels.Clear();
        _holeCardVisual = null;

        _dealerLabel.Text = "DEALER";
        _playerLabel.Text = "PLAYER";
        _betLabel.Text = "";
		_messageLabel.Modulate = new Color(1, 1, 1, 1);
    }

    private void OnRestartPressed()
    {
        _player = new Player("PLAYER", Settings.StartingBankroll);
        _dealer = new Dealer(Settings.HitSoft17);
        _shoe = new Shoe(Settings.DeckCount);
        _shoe.Shuffle();

        _restartButton.Visible = false;
        ClearTableCards();
        ShowBetButtons();
    }

    private async Task ContinueAfterInsuranceAsync(int insuranceBet)
    {
        _insurancePanel.Visible = false;

        bool dealerBJ = _dealer.IsBlackjack;
        bool playerBJ = _player.Hands[0].Hand.IsBlackjack;

        if (dealerBJ)
        {
            await RevealHoleCardAsync();
            _dealerLabel.Text = $"DEALER: {_dealer.Hand.Total}";
        }

        if (insuranceBet > 0 && dealerBJ)
        {
            _player.WinInsurance(insuranceBet);
            await Delay(0.5);
            PlayChipWinSound();
        }

        if (dealerBJ && playerBJ)
        {
            _player.PushBet(_player.Hands[0]);
            _messageLabel.Text = insuranceBet > 0
                ? "Both blackjack — push. Insurance pays."
                : "Both blackjack — push.";
            FinishRound();
            return;
        }

        if (dealerBJ)
        {
            _messageLabel.Text = insuranceBet > 0
                ? "Dealer blackjack -- you lose, but insurance pays"
                : "Dealer blackjack -- you lose";
            FinishRound();
            return;
        }

        if (playerBJ)
        {
            _player.WinBlackjack(_player.Hands[0]);
            await RevealHoleCardAsync();
            await Delay(0.5);
            PlayChipWinSound();
            _dealerLabel.Text = $"DEALER: {_dealer.Hand.Total}";
            _messageLabel.Text = insuranceBet > 0
                ? "Blackjack! You win 3:2 -- insurance lost"
                : "Blackjack! You win 3:2";
            FinishRound();
            return;
        }

        if (insuranceBet > 0)
        {
            _messageLabel.Text = $"Insurance lost (${insuranceBet:N0}). Your move";
        }
        else
        {
            _messageLabel.Text = "Your move";
        }
        ShowActionButtons();
    }

    private async Task OnInsuranceYesAsync()
    {
        int mainBet = _player.Hands[0].Bet;
        int insuranceBet = _player.PlaceInsurance(mainBet);
        PlayChipBetSound();
        UpdateTopBar();

        await Delay(0.5);

        await ContinueAfterInsuranceAsync(insuranceBet);
    }

    private async Task OnInsuranceNoAsync()
    {
        await ContinueAfterInsuranceAsync(0);
    }

	private void UpdateRulesText()
	{
		string h17 = Settings.HitSoft17
			? "Dealer hits on soft 17."
			: "Dealer stands on all 17s.";

		_rulesText.Text =
			$"{Settings.DeckCount}-deck shoe.\n\n" +
			$"{h17}\n\n" +
			"Blackjack pays 3:2.\n\n" +
			"Double down on any first two cards.\n" +
			"Double after split allowed.\n" +
			"No double after splitting Aces.\n\n" +
			"Split up to 4 hands.\n" +
			"Split Aces receive one card each and cannot be resplit.\n" +
			"21 on a split hand pays 1:1.\n\n" +
			"Insurance offered when the dealer shows an Ace.\n" +
			"Insurance pays 2:1.\n\n" +
			"No surrender.\n\n" +
			$"Starting bankroll: ${Settings.StartingBankroll:N0}";
	}

	/// <summary>
	/// Reads every .txt in LicenseFiles from res://licenses/ and concatenates
	/// them into one string. Requires the export preset to include
	/// "licenses/*.txt" under Filters to export non-resource files.
	/// </summary>
	private static string LoadAllLicenses()
	{
		var sb = new StringBuilder();
		foreach (string name in LicenseFiles)
		{
			string path = $"res://licenses/{name}";
			if (!FileAccess.FileExists(path))
			{
				GD.PrintErr($"License file missing: {path}");
				continue;
			}

			using FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
			if (file == null)
			{
				GD.PrintErr($"Could not open license file: {path}");
				continue;
			}

			string text = file.GetAsText();

			// Debian copyright format uses a lone "." on its own line to represent
			// blank lines. Convert them back to empty lines for display.
			text = Regex.Replace(text, @"^\.$", "", RegexOptions.Multiline);

			sb.AppendLine($"===== {name} =====");
			sb.AppendLine();
			sb.AppendLine(text);
			sb.AppendLine();
			sb.AppendLine();
		}

		return sb.ToString().TrimEnd();
	}
}