using System.Collections.Generic;

namespace NoteEditor.DTO
{
    public class MusicDTO
    {
        // **************************************************
        // 譜面情報
        // **************************************************
        [System.Serializable]
        public class EditData
        {
            public string name;
            public int maxLane;
            public float BPM;
            public int offset;
            public int level;
            public List<Note> notes;
        }

        [System.Serializable]
        public class Note
        {
            public int size;
            public int special;
            public double LPB;
            public double num;
            public int lane;
            public int type;
            public List<Note> notes;
        }
    }
}
