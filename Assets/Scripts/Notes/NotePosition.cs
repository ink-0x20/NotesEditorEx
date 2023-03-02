using UnityEngine;

namespace NoteEditor.Notes
{
    public struct NotePosition
    {
        // ノーツサイズ、特殊ノーツ
        public int size, special;
        public int LPB, num, block;

        public NotePosition(int size, int special, int LPB, int num, int block)
        {
            // ノーツサイズ
            this.size = size;
            // 特殊ノーツ
            this.special = special;
            // 下位変換
            int lowerChangeLPB = LPB;
            int lowerChangeNum = num;
            while (lowerChangeLPB % 2 == 0 && lowerChangeNum % 2 == 0)
            {
                lowerChangeLPB /= 2;
                lowerChangeNum /= 2;
            }
            while (lowerChangeLPB % 3 == 0 && lowerChangeNum % 3 == 0)
            {
                lowerChangeLPB /= 3;
                lowerChangeNum /= 3;
            }
            this.LPB = lowerChangeLPB;
            this.num = lowerChangeNum;
            this.block = block;
        }

        public int ToSamples(int frequency, int BPM)
        {
            return Mathf.FloorToInt(num * (frequency * 60f / BPM / LPB));
        }

        public override string ToString()
        {
            return LPB + "-" + num + "-" + block;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is NotePosition))
            {
                return false;
            }

            NotePosition target = (NotePosition)obj;
            return (
                Mathf.Approximately((float)num / LPB, (float)target.num / target.LPB) &&
                block == target.block);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public static NotePosition None
        {
            get { return new NotePosition(1, 0, -1, -1, -1); }
        }

        public NotePosition Add(int LPB, int num, int block)
        {
            return new NotePosition(this.size, this.special, this.LPB + LPB, this.num + num, this.block + block);
        }
    }
}
