using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Song
{
	// fields

	public string name;
	public TimeSignature timeSignature;

	public TempoMap tempoMap;
	public List<Note> rawNotes = new List<Note>();
	public List<Note> notes = new List<Note>();
	public List<List<Note>> measures = new List<List<Note>>();

	// constructor

	public Song(object path)
	{
		// reads file
		MidiFile songFile = null;

		if (path.GetType() == typeof(string))
		{
			songFile = MidiFile.Read((string)path);
		}
		else
		{
			songFile = MidiFile.Read((Stream)path);
		}

		// gets tempo map
		TempoMap tempoMap = songFile.GetTempoMap();
				
		// gets notes
		IEnumerable<Note> readNotes = songFile.GetNotes().Select(note => new Note
		(
			note.NoteNumber,
			note.TimeAs<MetricTimeSpan>(tempoMap),
			note.TimeAs<MusicalTimeSpan>(tempoMap),
			note.LengthAs<MetricTimeSpan>(tempoMap),
			note.LengthAs<MusicalTimeSpan>(tempoMap)
		));

		// sorts notes chronologically just in case
		rawNotes = readNotes.ToList().OrderBy(note => note.metricStartTime.TotalSeconds).ToList();

		/*// inserts rests into notes

		// checks if first note isn't at 0
		if (rawNotes[0].startTime > new MusicalTimeSpan(0, 1))
		{
			// inserts rest at beginning
			notes.Add(new Note
			(
				0,
				new MusicalTimeSpan(0, 1),
				rawNotes[0].startTime,
				true
			));
		}

		// loops through notes
		for (int i = 0; i < rawNotes.Count; i++)
		{
			// gets current note
			Note currentNote = rawNotes[i];

			// inserts current note in notes
			notes.Add(currentNote);

			// returns if last note
			if (i + 1 >= rawNotes.Count) break;

			// gets next note
			Note nextNote = rawNotes[i + 1];
			
			// calculates time between current note's end and next note's start
			MusicalTimeSpan currentNoteEnd = currentNote.startTime + currentNote.duration;
			MusicalTimeSpan restDuration = nextNote.startTime - currentNoteEnd;

			// checks if a gap for a rest exists
			if (restDuration > new MusicalTimeSpan(0, 1))
			{
				// adds rest to fill gaps
				notes.Add(new Note
				(
					0,
					currentNoteEnd,
					restDuration,
					true
				));
			}
		}

		// splits notes into measures
		List<Note> currentMeasure = new List<Note>();
		MusicalTimeSpan currentMeasureDuration = new MusicalTimeSpan(0, 1);

		foreach(Note note in notes)
		{
			// checks if current measure is full
			if(currentMeasureDuration == new MusicalTimeSpan(4, 4))
			{
				// adds current measure to list
				measures.Add(new List<Note>(currentMeasure));

				// creates next measure
				currentMeasure = new List<Note>();
				currentMeasureDuration = new MusicalTimeSpan(0, 1);
			}

			// checks if measure doesn't fit note
			if(currentMeasureDuration + note.duration > new MusicalTimeSpan(4, 4))
			{
				// inserts what it can of current note
				Note firstNote = note.Clone();
				firstNote.duration = new MusicalTimeSpan(4, 4) - currentMeasureDuration;
				currentMeasure.Add(firstNote);

				// adds current measure to list
				measures.Add(new List<Note>(currentMeasure));

				// fits leftover of note into new measure
				Note secondNote = note.Clone();
				secondNote.duration = note.duration - firstNote.duration;
				secondNote.tiedToLast = true;

				// splits second note as many times as neccessary
				while(secondNote.duration > new MusicalTimeSpan(4, 4))
				{
					// creates measure with part of note
					Note splitNote = secondNote.Clone();
					splitNote.duration = new MusicalTimeSpan(4, 4);
					measures.Add(new List<Note>(new Note[] { splitNote }));

					// subtracts chunked off part from second note
					secondNote.duration -= new MusicalTimeSpan(4, 4);
				}

				// adds remainder of second note to a new measure
				currentMeasure = new List<Note>();
				if(secondNote.duration > new MusicalTimeSpan(0, 1)) currentMeasure.Add(secondNote);
				currentMeasureDuration = secondNote.duration;
			}
			else
			{
				// adds current note to measure
				currentMeasure.Add(note);
				currentMeasureDuration += note.duration;
			}
		}

		// adds leftover measure if applicable
		if (currentMeasure.Count > 0) measures.Add(currentMeasure);*/
	}


	// subclasses

	public class TimeSignature
	{
		public int numerator;
		public int denominator;
	}

	public class Note
	{
		public int pitch; // measured in MIDI notation
		//public bool rest;
		//public bool tiedToLast;
		
		public MetricTimeSpan metricStartTime; // time in seconds
		public MusicalTimeSpan musicalStartTime; // time in beats

		public MetricTimeSpan metricDuration; // length in seconds
		public MusicalTimeSpan musicalDuration; // length in beats

		public Note(int pitch, MetricTimeSpan metricStartTime, MusicalTimeSpan musicalStartTime, MetricTimeSpan metricDuration, MusicalTimeSpan musicalDuration) //bool rest = false, bool tiedToLast = false)
		{
			this.pitch = pitch;

			this.metricStartTime = metricStartTime;
			this.metricDuration = metricDuration;

			this.musicalStartTime = musicalStartTime;
			this.musicalDuration = musicalDuration;
			//this.rest = rest;
			//this.tiedToLast = tiedToLast;
		}

		/*public Note Clone()
		{
			return new Note(pitch, new MetricTimeSpan(startTime), new MetricTimeSpan(duration), rest, tiedToLast);
		}*/
	}
}

