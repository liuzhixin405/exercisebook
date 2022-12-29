namespace Cat.Seckill.Base.RabbitMq
{
    public static class RabbitConstant
    {
        public const string SECKILL_EXCHANGE = "seckill.exchange";
        public const string SECKILL_QUEUE = "seckill.queue";

        public const string TEST_EXCHANGE = "test.exchange";
        public const string TEST_QUEUE = "test.queue";
        public const string EMAIL_EXCHANGE = "email.exchange";
        public const string SMS_EXCHANGE = "sms.exchange";
        public const string EMAIL_QUEUE = "email.queue";
        public const string SMS_QUEUE = "sms.queue";
        public const string DELAY_EXCHANGE = "delay.exchange";
        public const string DELAY_QUEUE = "delay.queue";
        public const string DELAY_ROUTING_KEY = "delay.routing.key";
        public const string DEAD_LETTER_EXCHANGE = "dead.letter.exchange";
        public const string DEAD_LETTER_QUEUE = "dead.letter.queue";
        public const string DEAD_LETTER_ROUTING_KEY = "dead.letter.routing.key";
    }
}