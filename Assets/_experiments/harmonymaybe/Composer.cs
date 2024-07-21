using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Plunderludics.Mut.HarmonyMaybe {

partial class Composer: MonoBehaviour {
    [Header("refs-atoms")]
    [SerializeField] IntEvent _NoteOn;
    [SerializeField] IntEvent _NoteOff;

    [Header("refs-masks")]
    [SerializeField] CustomMask _MaskCrash1;
    [SerializeField] CustomMask _MaskOgre1;
    [SerializeField] CustomMask _MaskGlover1;

    [SerializeField] CustomMask _MaskCrash2;
    [SerializeField] CustomMask _MaskOgre2;
    [SerializeField] CustomMask _MaskGlover2;

    readonly HashSet<int> _ActiveNotes = new();

    void Start() {
        CreateNotes();
        _NoteOn.Register(i => {
            if (_NoteBehaviours.TryGetValue(i, out var noteBehaviour)) {
                noteBehaviour.OnNoteStart();
            }
            _ActiveNotes.Add(i);
        });

        _NoteOff.Register(i => {
            if (_NoteBehaviours.TryGetValue(i, out var noteBehaviour)) {
                noteBehaviour.OnNoteEnd();
            }
            _ActiveNotes.Remove(i);
        });
    }

    void Update() {
        var delta = Time.deltaTime;

        foreach (var note in _ActiveNotes) {
            if (_NoteBehaviours.TryGetValue(note, out var noteBehaviour)) {
                noteBehaviour.OnNoteHold(delta);
            }
        }
    }

    class NoteBehaviour {
        public delegate void NoteStart();
        public delegate void NoteHold(float delta);
        public delegate void NoteEnd();

        public NoteStart OnNoteStart;
        public NoteEnd OnNoteEnd;
        public NoteHold OnNoteHold;
    }
}
}