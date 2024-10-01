using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PianoTiles
{
	class Sheet
	{
		private List<SoundUnit> sounds;
		public<SoundUnit> Sounds { get { return sounds; } }
		
		public Sheet(string src = "")
		{
			sounds = new List<SoundUnit>();
		}
		
		public void Load(string src)
		{
			
		}
	}
	
	enum Rhythm
	{
		fullNote, halfNote, quarterNote, eighthNote, sixteenthNote, thirtysecondNote,
		dottedHalfNote, dottedQuarterNote, dottedEighthNote, dottedSixteenthNote, dottedThirtysecondNote,
		fullRest, halfRest, quarterRest, eighthRest, sixteenthRest, thirtysecondRest,
		dottedHalfRest, dottedQuarterRest, dottedEighthRest, dottedSixteenthRest, dottedThirtysecondRest
	}
	
	enum PitchName { C, Cs, D, Ds, E, F, Fs, G, Gs, A, As, B }
	
	struct Pitch
	{
		public Pitch(PitchName pitchname = PitchName.A, int octave = 0)
		{
			this.pitchname = pitchname;
			this.octave = octave;
		}
		public PitchName pitchname;
		public int octave;
	}
	
	struct SountUnit
	{
		
	}
}