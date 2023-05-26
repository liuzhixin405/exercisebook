// See https://aka.ms/new-console-template for more information

int i = 1000;
while(i>0){

var result = GetWinners(new List<User>{
    new User{ Name = "张三", UserType = UserType.Normal },
    new User{ Name = "李四", UserType = UserType.Vip },
    new User{ Name = "王五", UserType = UserType.Vip },
    new User{ Name = "赵六", UserType = UserType.Normal },
    new User{ Name = "孙七", UserType = UserType.Normal },
    new User{ Name = "周八", UserType = UserType.Vip },
    new User{ Name = "吴九", UserType = UserType.Normal },
    new User{ Name = "郑十", UserType = UserType.Normal },
    new User{ Name = "钱十一", UserType = UserType.Vip },
    new User{ Name = "孔十二", UserType = UserType.Normal },
    new User{ Name = "杨十三", UserType = UserType.Vip },
    new User{ Name = "朱十四", UserType = UserType.Vip },
    new User{ Name = "秦十五", UserType = UserType.Normal },
});


if(result.Count!=10){
System.Console.WriteLine($"第{10001-i}次抽奖,中奖人数：{result.Count}");

}
//Console.WriteLine($"中奖名单：{System.Text.Json.JsonSerializer.Serialize(result)}");
i--;

}



static List<User> GetWinners(List<User> users)
{
    Random random = new Random();
    List<User> winners = new List<User>();
    int lotteryCount = 10; // 抽奖人数为10

    // 创建中奖者名单数组
    List<string> winnerNames = new List<string>();

    // 根据参与者的类型确定中奖概率
    foreach (User user in users)
    {
        if (user.UserType == UserType.Vip)
        {
            // VIP 中奖概率为2
            for (int i = 0; i < 2; i++)
            {
                winnerNames.Add(user.Name);
            }
        }
        else
        {
            // 非 VIP 中奖概率为1
            winnerNames.Add(user.Name);
        }
    }

    // 进行10次抽奖
    for (int i = 0; i < lotteryCount; i++)
    {
        // 随机选择中奖者的索引
        int winnerIndex = random.Next(0, winnerNames.Count);
        string winnerName = winnerNames[winnerIndex];

        // 根据中奖者名字查找对应的 User 对象
        User winner = users.Find(user => user.Name == winnerName);

        winners.Add(winner);

        // 如果中奖者是 VIP，移除所有重复的名字
        if (winner.UserType == UserType.Vip)
        {
            winnerNames.RemoveAll(name => name == winnerName);
        }
        else
        {
            // 如果中奖者是非 VIP，只移除一个名字
            winnerNames.Remove(winnerName);
        }
    }

    return winners;
}


  /* /// <summary>
        /// 获取中奖用户
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        static List<User> GetWinners(List<User> users)
        {
            Random random = new Random();
            List<User> winners = new List<User>();
            #region old
            var numParticipants = users.Count;
            var number = numParticipants < 10 ? numParticipants : 10;
            for (int i = 0; i < number; i++)
            {

                // 复制参与抽奖的人员列表
                List<User> participantsCopy = new List<User>(users.Except(winners).ToList());

                // 计算每个人的中奖概率
                double[] probabilities = CalculateWinningProbabilities(participantsCopy);

                for (int j = 0; j < numParticipants; j++)
                {
                    // 生成一个随机数
                    double randomNumber = random.NextDouble();

                    // 根据随机数确定中奖者
                    double cumulativeProbability = 0;
                    int winnerIndex = -1;
                    for (int k = 0; k < participantsCopy.Count; k++)
                    {
                        cumulativeProbability += probabilities[k];

                        if (randomNumber <= cumulativeProbability)
                        {
                            winnerIndex = k;
                            break;
                        }
                    }

                    if (winnerIndex >= 0)
                    {
                        var winner = participantsCopy[winnerIndex];
                        winners.Add(winner);

                        // 移除中奖者，确保每个用户只能中奖一次
                        participantsCopy.RemoveAt(winnerIndex);
                        probabilities = CalculateWinningProbabilities(participantsCopy);
                        break;
                    }
                }
            }
            #endregion
            return winners;
        }

        /// <summary>
        /// 设置概率
        /// </summary>
        /// <param name="participants"></param>
        /// <returns></returns>
        static double[] CalculateWinningProbabilities(List<User> participants)
        {
            int numParticipants = participants.Count;
            int numVIPs = participants.Count(p => p.UserType== UserType.Vip);
            int numNonVIPs = numParticipants - numVIPs;

            double[] probabilities = new double[numParticipants];
            for (int i = 0; i < numParticipants; i++)
            {
                if (participants[i].UserType== UserType.Vip)
                    probabilities[i] = 2.0 / (numVIPs * 3);
                else
                    probabilities[i] = 1.0 / (numNonVIPs * 3);
            }
            return probabilities;
        } */