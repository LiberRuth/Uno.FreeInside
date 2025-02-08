namespace Uno.FreeInside.Data;

internal class RandomUA
{
    static Random rnd = new Random();
    private static string[] UserAgentList = new string[] {
          "Mozilla/5.0 (iPhone; CPU iPhone OS 17_6_1 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/17.5 Mobile/15E148 Safari/604.1",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 17_6_1 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) CriOS/128.0.6613.34 Mobile/15E148 Safari/604.1",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 17_6_1 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) FxiOS/129.0 Mobile/15E148 Safari/605.1.15",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 17_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/17.0 Mobile/15E148 Safari/604.1",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 17_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/16.4 Mobile/15E148 Safari/604.1",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 18_7 like Mac OS X; HP01124) AppleWebKit/533.2 (KHTML, like Gecko) CriOS/432.0.8702.51 Mobile/15E148 Safari/804.17",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 18_7; HP01124) AppleWebKit/533.2 (KHTML, like Gecko) CriOS/432.0.8702.51 Mobile/15E148 Safari/804.17",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 14_0 like Mac OS X) AppleWebKit/603.1.30 (KHTML, like Gecko) Version/12.0.0 Mobile/15A5370a Safari/602.1",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 14.6 (17F80) like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) FxiOS/100.0 Mobile/15E148 Safari/605.1.15",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 16_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) CriOS/104.0.5112.88 Mobile/15E148 Safari/604.1",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 16_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) CriOS/96.0.4664.53 Mobile/15E148 Safari/604.1",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 18_7 like Mac OS X; iPhone 15 Pro Max) AppleWebKit/533.2 (KHTML, like Gecko) CriOS/432.0.8702.51 Mobile/15E148 Safari/804.17",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 18_7; iPhone 15 Pro Max) AppleWebKit/533.2 (KHTML, like Gecko) CriOS/432.0.8702.51 Mobile/15E148 Safari/804.17",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 18_7 like Mac OS X; HP01124; 15 Pro Max) AppleWebKit/533.2 (KHTML, like Gecko) CriOS/432.0.8702.51 Mobile/15E148 Safari/804.17",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 18_7; iPhone 15 Pro Max) AppleWebKit/806.25 (KHTML, like Gecko) CriOS/432.0.8702.51 Mobile/15E148 Safari/804.17",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 18_7 like Mac OS X) AppleWebKit/807.1.15 (KHTML, like Gecko) CriOS/432.0.8702.51 Mobile/15E148 Safari/804.17",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 18_3 like Mac OS X) AppleWebKit/705.1.15 (KHTML, like Gecko) CriOS/222.0.6261.51 Mobile/15E148 Safari/704.1",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 15_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) CriOS/101.0.4951.58 Mobile/15E148 Safari/604.1",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 18_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/18.0 Mobile/15E148 Safari/604.1",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 18_7; iPhone 16 Ultra) AppleWebKit/533.2 (KHTML, like Gecko) CriOS/432.0.8702.51 Mobile/15E148 Safari/804.17",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 18_7 like Mac OS X; A3106) AppleWebKit/533.2 (KHTML, like Gecko) CriOS/432.0.8702.51 Mobile/15E148 Safari/804.17",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 18_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) GSA/320.0.639621854 Mobile/15E148 Safari/604.1",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 18_7; iPhone 16 Pro Max) AppleWebKit/533.2 (KHTML, like Gecko) CriOS/432.0.8702.51 Mobile/15E148 Safari/804.17",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 18_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) GSA/319.0.638705450 Mobile/15E148 Safari/604.1",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 17_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) CriOS/114.0.5735.99 Mobile/15E148 Safari/604.1",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 15_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) CriOS/102.0.5005.67 Mobile/15E148 Safari/604.1",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 15_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) CriOS/91.0.4472.80 Mobile/15E148 Safari/604.1",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 15_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) CriOS/1.0.7.0 Mobile/15E148 Safari/605.1",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 15_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) CriOS/87.0.4280.77 Mobile/15E148 Safari/604.1",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 15_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) CriOS/100.0.4896.77 Mobile/15E148 Safari/604.1",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 15_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) CriOS/92.0.4515.90 Mobile/15E148 Safari/604.1",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 14_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) CriOS/84.0.4147.71 Mobile/15E148 Safari/604.1",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 14_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) CriOS/85.0.4183.109 Mobile/15E148 Safari/604.1",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 14_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) CriOS/86.0.4240.93 Mobile/15E148 Safari/604.1",
            "Mozilla/7.0 (iPhone; CPU iPhone OS 18_7; iPhone 16 Ultra) AppleWebKit/533.2 (KHTML, like Gecko) CriOS/432.0.8702.51 Mobile/15E148 Safari/804.17",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 14_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) CriOS/87.0.4280.163 Mobile/15E148 Safari/604.1",
            "Mozilla/7.0 (iPhone; CPU iPhone OS 18_7; iPhone 15 Pro Max) AppleWebKit/533.2 (KHTML, like Gecko) CriOS/126.0.6478.35 Mobile/15E148 Safari/804.17",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 13_0 like Mac OS X) AppleWebKit/605 (KHTML, like Gecko) Mobile/15E148 Safari/605 QwantMobile/2.8",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 13_0 like Mac OS X) AppleWebKit/605 (KHTML, like Gecko) Mobile/15E148 Safari/605 QwantMobile/3.0",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 13_1 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.1 Mobile/15E148",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 13_1 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) GSA/190.1.416438978 Mobile/15E148 Safari/604.1",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 13_1 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) CriOS/92.0.4515.90 Mobile/15E148 Safari/604.1",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 16_1_1 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.5 Mobile/15E148 Snapchat/10.77.5.59 (like Safari/604.1)",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 17_5_1 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/17.5 Mobile/15E148 Safari/604.1"
        };
    public static string UserAgent()
    {
        return UserAgentList[rnd.Next(0, UserAgentList.Length)];
    }
}
