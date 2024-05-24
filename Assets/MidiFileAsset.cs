using System.Linq;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using NaughtyAttributes;
using UnityEngine;

namespace Plunderludics.Lib {
public class MidiFileAsset : ScriptableObject {
    public MidiFile File;

    [ShowNativeProperty] int TrackCount {
        get => File.GetTrackChunks().Count();
    }

}
}