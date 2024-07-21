using System.Collections.Generic;

namespace Plunderludics.Mut.HarmonyMaybe {

partial class Composer {
    Dictionary<int, NoteBehaviour> _NoteBehaviours;

    void CreateNotes() {
        _NoteBehaviours = new() {
            {
                48, new NoteBehaviour() {
                    OnNoteStart = () => {
                        _MaskGlover1.AddRandomRect();
                    },
                    OnNoteHold = delta => { },
                    OnNoteEnd = () => {
                        _MaskGlover1.Clear();
                     }
                }
            },
            {
                49, new NoteBehaviour() {
                    OnNoteStart = () => {
                        _MaskOgre1.AddRandomRect();
                    },
                    OnNoteHold = delta => { },
                    OnNoteEnd = () => {
                        _MaskOgre1.Clear();
                     }
                }
            },
            {
                50, new NoteBehaviour() {
                    OnNoteStart = () => {
                        _MaskGlover1.AddRandomRect();
                    },
                    OnNoteHold = delta => { },
                    OnNoteEnd = () => {
                        _MaskGlover1.Clear();
                     }
                }
            },
            {
                51, new NoteBehaviour() {
                    OnNoteStart = () => {
                        _MaskOgre1.AddRandomRect();
                    },
                    OnNoteHold = delta => { },
                    OnNoteEnd = () => {
                        _MaskOgre1.Clear();
                     }
                }
            },
            {
                52, new NoteBehaviour() {
                    OnNoteStart = () => {
                        _MaskGlover1.AddRandomRect();
                    },
                    OnNoteHold = delta => { },
                    OnNoteEnd = () => {
                        _MaskGlover1.Clear();
                     }
                }
            },
            {
                53, new NoteBehaviour() {
                    OnNoteStart = () => {
                        _MaskGlover1.AddRandomRect();
                    },
                    OnNoteHold = delta => { },
                    OnNoteEnd = () => {
                        _MaskGlover1.Clear();
                     }
                }
            },
            {
                54, new NoteBehaviour() {
                    OnNoteStart = () => {
                        _MaskOgre1.AddRandomRect();
                    },
                    OnNoteHold = delta => { },
                    OnNoteEnd = () => {
                        _MaskOgre1.Clear();
                     }
                }
            },
            {
                55, new NoteBehaviour() {
                    OnNoteStart = () => {
                        _MaskGlover1.AddRandomRect();
                    },
                    OnNoteHold = delta => { },
                    OnNoteEnd = () => {
                        _MaskGlover1.Clear();
                     }
                }
            },
            {
                56, new NoteBehaviour() {
                    OnNoteStart = () => {
                        _MaskOgre1.AddRandomRect();
                    },
                    OnNoteHold = delta => { },
                    OnNoteEnd = () => {
                        _MaskOgre1.Clear();
                     }
                }
            },
            {
                57, new NoteBehaviour() {
                    OnNoteStart = () => {
                        _MaskGlover1.AddRandomRect();
                    },
                    OnNoteHold = delta => { },
                    OnNoteEnd = () => {
                        _MaskGlover1.Clear();
                     }
                }
            },
            {
                58, new NoteBehaviour() {
                    OnNoteStart = () => {
                        _MaskOgre1.AddRandomRect();
                    },
                    OnNoteHold = delta => { },
                    OnNoteEnd = () => {
                        _MaskOgre1.Clear();
                     }
                }
            },
            {
                59, new NoteBehaviour() {
                    OnNoteStart = () => {
                        _MaskGlover1.AddRandomRect();
                    },
                    OnNoteHold = delta => { },
                    OnNoteEnd = () => {
                        _MaskGlover1.Clear();
                     }
                }
            },
        };
    }

}
}