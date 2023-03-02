namespace NoteEditor.Notes
{
    public enum NoteTypes 
    {
        // 改良前のノーツ(1桁番台)
        BeforeSingle = 1,
        BeforeLong = 2,

        // 通常ノーツ(2桁番台)
        Single = 11,

        // ロングノーツ(3桁番台)
        StraightLineLong = 101,
        LeftCurveLong = 102,
        RightCurveLong = 103,

        // フリックノーツ(4桁番台)
        TopFlick = 1001,
        RightFlick = 1002,
        BottomFlick = 1003,
        LeftFlick = 1004,
        TopRightFlick = 1005,
        BottomRightFlick = 1006,
        BottomLeftFlick = 1007,
        TopLeftFlick = 1008
    }
}
