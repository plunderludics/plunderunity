using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Plunderludics.Tools {

public class NoteSequenceRecorder : MonoBehaviour {
    [Header("config")]
    public IntReference MaxSteps;

    [TextArea]
    public string InitialInput;

    [Header("refs - state")]
    public BeatLoop Loop;

    // could be an array
    Dictionary<int, HashSet<int>> notesSequence = new();
    Dictionary<int, int> noteMap = new();
    HashSet<int> currNotes = new();

    public IntEvent OnNoteOn;
    public IntEvent OnNoteOff;

    // -- lifecycle --
    void Start() {
        if (!string.IsNullOrWhiteSpace(InitialInput)) {
            var lineIndex = 0;
            foreach (var line in InitialInput.Split("\n")) {
                var notes = new HashSet<int>();
                notesSequence.Add(lineIndex, notes);
                foreach (var noteStr in line.Split(",")) {
                    if (int.TryParse(noteStr.Trim(), out var note)) {
                        notes.Add(note);
                    }
                }

                lineIndex++;
            }

            MaxSteps.Value = lineIndex;
        }

        Loop.CurrStep.Changed.Register(_ => {
            var i = Loop.CurrStep.Value % MaxSteps.Value;
            if (!notesSequence.TryGetValue(i, out var currNotes)) {
                currNotes = new HashSet<int>();
            }

            i = (i + 1) % MaxSteps.Value;

            if (!notesSequence.TryGetValue(i, out var nextNotes)) {
                notesSequence.Add(i, new());
            }

            var log = $"[sequence] beat {i}: notes raised: ";
            if (nextNotes != null) {
                currNotes?.SymmetricExceptWith(nextNotes);
            }

            foreach (var note in currNotes) {
                var evt = nextNotes.Contains(note) ? OnNoteOn : OnNoteOff;
                evt.Raise(note);
                log += $"{note} {(nextNotes.Contains(note) ? "on" : "off")},";
            }

            Debug.Log(log);

            currNotes = nextNotes;
        });
    }

    public bool AddNote(int i) {
        var currStep = Loop.CurrStep.Value;
        if (!notesSequence.TryGetValue(currStep, out currNotes)) {
            currNotes = new();
            notesSequence.Add(currStep, currNotes);
        }

        return currNotes.Add(i);
    }
}

}