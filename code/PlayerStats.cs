using System;

public sealed class PlayerStats : Component {
	/// <summary>
	/// The rate at which stats decrease
	/// </summary>
	[Property] float StatDecaySpeed { get; set; } = 1f;


	
	/// <summary>
	/// CopScript referenced to call cops after a certain score is achieved
	/// </summary>
	[Property] CopScript Cops { get; set; }


	/// <summary>
	/// Game Over Screen referenced to Enable when Hearts hits 0
	/// </summary>
	[Property] GameObject GameOverScreen { get; set; }


	// So that Cops are only called once
	bool CopsCalled = false;


	// Stat Values are represented as floats between 0-100
	public Dictionary<string, float> Stats = new() {
		{ "hunger",		50f },
		{ "thirst",		50f },
		{ "buzz",		50f },
		{ "boredom",	50f }
	};

	
	// These values are rapidly decreased if a stat is at 0
	// If these backup stats hit 0, player dies :(
	public Dictionary<string, float> StatDeath = new() {
		{ "hunger",     10f },
		{ "thirst",     10f },
		{ "buzz",       10f },
		{ "boredom",    10f }
	};


	// Post Processing for over using Items
	Sharpen Sharp;

	// DrunkScript is referenced to control drunkiness when Nourishing drinking
	// This is also used to modify the Score rate (More drunk = More points!)
	//		This calculation is done in PlayerMovement
	public DrunkScript Drunk;

	// Score which increased based on speed and alcohol consumption
	public int Score = 0;

	// Player Health
	public int Hearts = 3;

	// Disabled when GameOver screen is reached
	public bool Playing = true;

	// Rand used to decrease stat by random offsets
	Random Rnd = new();

	void OverUse(string stat, float amount=1f) {
		if (stat == "buzz" && Sharp != null) {
			Sharp.Scale = Math.Min( Sharp.Scale + (amount * 0.1f), 5f );
		}
	}

	// Increment stat or control intoxication
	public void Nourish(string stat, float amount=1f) {
		if (!Stats.ContainsKey(stat)) {
			if (stat == "drinking") {
				Drunk.Intoxication = Math.Min( Drunk.Intoxication + amount, 50f );
			}

			return;
		}

		Stats[stat] = Math.Min( Stats[stat] + amount, 100f);
		StatDeath[stat] = 10f;

		if (Stats[stat] == 100f) { OverUse( stat, amount ); }
	}

	// Returns what percent that stat is filled
	public float Percent(string stat) {
		if (stat == "drinking") {
			return (float)Math.Round( (double)(Drunk.Intoxication / 10f), 2 ) * 50f;
		}

		if (Stats.ContainsKey(stat)) {
			return (float)Math.Round( (double)(Stats[stat] / 100f), 2 ) * 100f;
		} else {
			return 0f;
		}
	}

	public void HurtPlayer() {
		Hearts--;

		if (Hearts <= 0) {
			if (GameOverScreen != null) {
				GameOverScreen.Enabled = true;
				GameOverScreen.Components.Get<GameOver>().EndGame( "ded" );
			}
		}
	}

	protected override void OnStart() {
		Sharp = Scene.Camera.Components.Get<Sharpen>();
		Drunk = GameObject.Components.Get<DrunkScript>();
	}

	protected override void OnFixedUpdate() {
		if (Hearts <= 0 || !Playing) { return; }

		float statOffset;

		foreach (string stat in Stats.Keys) {
			statOffset = Rnd.Next( 1, 60 ) / 10f;
			Stats[stat] = Math.Max( Stats[stat] - (StatDecaySpeed * statOffset), 0f );

			if (Stats[stat] == 0f) {
				StatDeath[stat] = Math.Max( StatDeath[stat] - Time.Delta, 0f );

				if (StatDeath[stat] == 0f) {
					GameOverScreen.Enabled = true;
					GameOverScreen.Components.Get<GameOver>().EndGame(stat);
				}
			}
		}

		if (Sharp != null) {
			Sharp.Scale = Math.Max( Sharp.Scale - StatDecaySpeed * 2f, 0f );
		}

		if (Cops == null) { return; }

		if (!CopsCalled && Score >= 25000) {
			Cops.StartFlashing();
			CopsCalled = true;
		}
	}
}
