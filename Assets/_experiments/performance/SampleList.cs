using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityHawk;

namespace _experiments.performance {
[CreateAssetMenu(fileName = "SampleList", menuName = "mutplunders/samplelist", order = 0)]
public class SampleList: ScriptableObject {
    public List<Savestate> Samples;

    Dictionary<string, Savestate> _dictionary;


    public Savestate GetSampleByName(string sampleName) {
        if (_dictionary == null) {
            _dictionary = Samples.ToDictionary(
                s => s.name,
                s => s
            );
        }

        if (_dictionary.TryGetValue(sampleName, out var sample)) {
            return sample;
        }

        return null;
    }
}
}