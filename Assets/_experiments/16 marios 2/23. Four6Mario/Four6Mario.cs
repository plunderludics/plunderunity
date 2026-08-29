using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityHawk;

public class Four6Mario : MonoBehaviour {
    // should there be overlap?
    [SerializeField] Emulator m_Emulator;
    [SerializeField] List<Savestate> m_Samples;
    [SerializeField] Rom m_Rom;
    [SerializeField] RawImage m_Image;
    
    // -- props --
    float m_Time;
    int m_Index = -1;
    [SerializeField] bool m_Play;
    bool m_Sounding;
    float m_CurrStart = -1;
    float m_CurrEnd = -1;

    // Update is called once per frame
    void Update() {
        if (!m_Play) {
            return;
        }

        m_Time += Time.deltaTime;

        if (m_Time > m_CurrEnd) {
            // stop "sounding"
            m_Emulator.IsMuted = true;
            m_Image.gameObject.SetActive(false);
            
            // start next section
            m_Index++;
            var currSection = player1[m_Index];
            m_CurrStart = Random.Range(currSection.StartRange_Start, currSection.StartRange_End);
            var endStart = Mathf.Max(currSection.EndRange_Start, m_CurrStart);
            m_CurrEnd = Random.Range(endStart, currSection.EndRange_End);
            
            // set the emulator state
            m_Emulator.LoadState(m_Samples[currSection.Sound - 1]);
            
            Debug.Log($"playing sound {currSection.Sound} from {m_CurrStart} to {m_CurrEnd}, current time: {m_Time}");
            m_Sounding = false;
        }

        if (m_Time >= m_CurrStart && !m_Sounding) {
            Debug.Log($"starting at {m_Time}");
            
            // start "sounding"
            m_Image.gameObject.SetActive(true);
            m_Sounding = true;
            m_Emulator.IsMuted = false;
        }
    }

    struct Four6Instruction {
        public int Sound;
        public float StartRange_Start;
        public float StartRange_End;
        public float EndRange_Start;
        public float EndRange_End;

        public Four6Instruction(float startRangeStart, float startRangeEnd, float endRangeStart, float endRangeEnd, int sound) {
            Sound = sound;
            StartRange_Start = ConvertTime(startRangeStart);
            StartRange_End = ConvertTime(startRangeEnd);
            EndRange_Start = ConvertTime(endRangeStart);
            EndRange_End = ConvertTime(endRangeEnd);
        }

        private static float ConvertTime(float src) {
            var floor = Mathf.Floor(src);
            var frac = src - Mathf.Floor(src);

            return floor * 60 + frac * 100;
        }
    }
    
    /// player 1's part aka One7
    private List<Four6Instruction> player1 = new() {
        // player 1
        // page 1
        new(0.00f, 1.15f, 0.55f, 2.05f, 2),
        new(0.00f, 1.30f, 1.00f, 2.30f, 4),
        new(1.50f, 2.35f, 2.20f, 3.05f, 9),
        new(2.50f, 3.35f, 3.20f, 4.05f, 11),
        new(3.00f, 4.00f, 3.40f, 4.40f, 5),
        new(3.40f, 4.55f, 4.35f, 5.45f, 8),
        new(4.10f, 5.40f, 5.10f, 6.40f, 2),
        new(5.15f, 6.45f, 6.15f, 7.45f, 8),
        // page 2
        new(6.10f, 7.40f, 7.10f, 8.40f, 8),
        new(7.30f, 8.15f, 8.00f, 8.45f, 4),
        new(8.10f, 9.40f, 9.10f, 10.40f, 8),
        new(8.15f, 9.45f, 9.15f, 10.45f, 5),
        new(10.35f, 11.05f, 10.55f, 11.25f, 2),
        new(10.25f, 11.10f, 10.55f, 11.40f, 5),
        new(11.35f, 11.50f, 11.45f, 12.00f, 8),
        new(10.55f, 12.25f, 11.55f, 13.25f, 8),
        new(11.30f, 13.00f, 12.30f, 14.00f, 8),
        // page 3
        new(13.20f, 13.35f, 13.30f, 13.45f, 7),
        new(13.20f, 14.35f, 14.15f, 15.25f, 6),
        new(13.35f, 14.50f, 14.30f, 15.40f, 7),
        new(15.20f, 15.35f, 15.40f, 15.45f, 2),
        new(15.35f, 16.05f, 15.55f, 16.25f, 7),
        new(15.20f, 16.20f, 16.00f, 17.00f, 8),
        new(16.05f, 17.05f, 16.45f, 17.45f, 3),
        new(16.35f, 17.50f, 17.30f, 18.40f, 8),
        // page 4
        new(17.25f, 18.25f, 18.05f, 19.05f, 6),
        new(18.30f, 19.00f, 18.50f, 19.20f, 7),
        new(19.10f, 19.40f, 19.30f, 20.00f, 8),
        new(18.35f, 20.05f, 19.35f, 21.05f, 12),
        new(19.45f, 20.30f, 20.15f, 21.00f, 4),
        new(20.45f, 31.45f, 21.25f, 22.25f, 7),
        new(22.05f, 22.50f, 22.35f, 23.20f, 6),
        //page 5
        new(22.10f, 23.40f, 23.10f, 24.40f, 12),
        new(23.00f, 24.00f, 23.40f, 24.40f, 7),
        new(24.15f, 25.30f, 25.10f, 26.20f, 5),
        new(24.10f, 25.40f, 25.10f, 26.40f, 3),
        new(25.55f, 27.10f, 26.50f, 28.00f, 9),
        new(26.15f, 27.30f, 27.10f, 28.20f, 7),
        new(27.35f, 28.50f, 28.30f, 29.40f, 5),
        new(28.05f, 28.50f, 28.35f, 29.20f, 5),
        new(29.10f, 29.40f, 29.30f, 30.00f, 8),
        new(29.35f, 29.50f, 29.45f, 30.00f, 11),
    };
}
