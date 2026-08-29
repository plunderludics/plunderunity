using System;
using System.IO;
using System.Text;
using BizHawk.Client.Common;
using BizHawk.Common;
using BizHawk.Common.PathExtensions;
using BizHawk.Emulation.Common;
using NaughtyAttributes;
using Plunderludics.Lib;
using UnityEngine;
using UnityHawk;

public class Test : MonoBehaviour {
	public Rom m_Rom;
	public Savestate m_Savestate;
	public Track m_Track;

    [Button("load sample")]
    void LoadSample() {
	    m_Track.LoadSample(m_Savestate, m_Rom);
    }

}