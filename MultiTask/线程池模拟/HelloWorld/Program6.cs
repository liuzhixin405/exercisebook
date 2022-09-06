using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XmlToJson
{
    class Program6
    {
        static void Five_Main(string[] args)
        {
            string xml = @"<?xml version=""1.0"" encoding=""GBK""?>
            <Transaction>
                <SystemHead>
                    <Language> zh_CN </Language>
                    <Encodeing></Encodeing>
                    <Version></Version>
                    <ServiceName></ServiceName>
                    <CifNo> 2000729524 </CifNo>
                    <UserID> 014 </UserID>
                    <SyMacFlag></SyMacFlag>
                    <MAC></MAC>
                    <SyPinFlag></SyPinFlag>
                    <PinSeed></PinSeed>
                    <LicenseId></LicenseId>
                    <Flag></Flag>
                    <Note></Note>
                </SystemHead>
                <TransHead>
                    <TransCode> b2e004003 </TransCode>
                    <BatchID> 20038556452006121800000002 </BatchID>
                    <JnlDate> 20030809(YYYYMMDD) </JnlDate>
                    <JnlTime> 144534 </JnlTime>
                </TransHead>
                <TransContent>
                        <ReqData>
                            <ClientPatchID> 200385564520061218000000010001 </ClientPatchID>
            <ClientBchID> 222 </ClientBchID>
            <ClientPchID> 3639174 </ClientPchID>
                        </ReqData>
                </TransContent>
            </Transaction>";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            string jsonText = JsonConvert.SerializeXmlNode(doc);
            Console.WriteLine(jsonText);

            string jsonStr = @"{""?xml"":{
                                ""@version"":""1.0"",
                                ""@encoding"":""GBK""
                            },
                            ""transaction"":{
                                ""balance"":{
                                    ""balanceRequest"":{
                                        ""balanceRequestHeader"":{
                                            ""language"":""zh-cn"",
                                            ""clientTime"":""2009-10-23T10:30:01"",
                                            ""logonPart"":{
                                                ""userID"":"""",
                                                ""userPassword"":"""",
                                                ""operatorID"":""""
                                            },
                                            ""batchID"":""2000049420200910231000001"",
                                            ""transPatches"":""1""
                                        },
                                        ""balanceRequestBody"":{
                                            ""balanceRequestRecord"":{
                                                ""clientPatchID"":""2000049420200910231000001001"",
                                                ""accountNo"":""083503120100304014119""
                                            }
                                        }
                                    }
                                }
                            }
                        }";
            XmlDocument doc1 = JsonConvert.DeserializeXmlNode(jsonStr);
            Console.WriteLine(doc1.OuterXml);
            Console.Read();

        }
    }
}
