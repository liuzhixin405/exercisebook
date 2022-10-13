using System.ComponentModel;

namespace LiveScore_ES.Backend.ReadModel
{
    /// <summary>
    /// 比赛队伍
    /// </summary>
    public enum TeamId
    {
        [Description("主场")]
        Home=1,
        [Description("客场")]
        Visitors=2
    }
}
