using System.ComponentModel;

namespace LiveScore_ES.Backend.ReadModel
{
    public enum MatchState
    {
        [Description("准备")]
        ToBePlayed =0,
        [Description("进行中")]
        InProress = 1,
        [Description("完成")]
        Finished =2,
        [Description("暂停")]
        Suspended =3
    }
}
