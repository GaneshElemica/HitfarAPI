using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Xml;
using PSEncryption_Decryption;
using System.Diagnostics;

namespace HitfarAPI.Controllers
{
    public class ShippingApiController : ApiController
    {
        String shipDirectoryName = System.Configuration.ConfigurationManager.AppSettings.Get("shipDirectoryName").ToString();
        String ShippingKey = string.Empty;
        String DeliveryNo = string.Empty;
        String strOperation = string.Empty;
        String ERPType = string.Empty;
        String PrinterName = string.Empty;
        String PlantId = string.Empty;
        XmlDocument Resxml = new XmlDocument();
        XmlDocument xmldoc = new XmlDocument();
        String strResponse = string.Empty;
        public string Response(string msg)
        {
            string strResponse = "";
            try
            {
                strResponse = "<Result>" + System.Environment.NewLine;
                strResponse = strResponse + "<Status>ERROR</Status>" + System.Environment.NewLine;
                strResponse = strResponse + "<Message>" + System.Environment.NewLine;
                strResponse = strResponse + msg + System.Environment.NewLine;
                strResponse = strResponse + "</Message>" + System.Environment.NewLine;
                strResponse = strResponse + "</Result>" + System.Environment.NewLine;
            }
            catch (Exception ex)
            {
                strResponse = strResponse + ex.Message + "</Message>" + System.Environment.NewLine;
                strResponse = strResponse + "</Result>" + System.Environment.NewLine;
            }
            return strResponse;
        }
        private HttpResponseMessage HResponsexml(string Input)
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent(Input.ToString(), Encoding.UTF8, "text/xml")
            };
        }

        [HttpPost]
        public HttpResponseMessage ProcessShipment(HttpRequestMessage request)
        {
            try
            {
                if (request.Content.ReadAsStreamAsync().Result.Length > 0)
                {
                    xmldoc.Load(request.Content.ReadAsStreamAsync().Result);
                    try
                    {
                        ShippingKey = xmldoc.GetElementsByTagName("SHIPPINGKEY")[0].InnerText;
                    }
                    catch (Exception)
                    {
                        return HResponsexml(Response("SHIPPING KEY REQUIRED"));
                    }
                    try
                    {
                        DeliveryNo = xmldoc.GetElementsByTagName("DELIVERYNO")[0].InnerText;
                    }
                    catch (Exception)
                    {
                        return HResponsexml(Response("DELIVERY# REQUIRED"));
                    }
                    try
                    {
                        strOperation = xmldoc.GetElementsByTagName("OPERATION")[0].InnerText;
                    }
                    catch (Exception)
                    {
                        return HResponsexml(Response("OPERATION TYPE REQUIRED"));
                    }
                    try
                    {
                        ERPType = xmldoc.GetElementsByTagName("ERP")[0].InnerText;
                    }
                    catch (Exception)
                    {
                        return HResponsexml(Response("ERP NAME REQUIRED"));
                    }
                    try
                    {
                        PrinterName = xmldoc.GetElementsByTagName("PRINTER")[0].InnerText;
                    }
                    catch (Exception)
                    {
                        return HResponsexml(Response("PRINTER NAME REQUIRED"));
                    }
                    try
                    {
                        PlantId= xmldoc.GetElementsByTagName("Plant")[0].InnerText;
                    }
                    catch(Exception)
                    {
                        
                    }
                    try
                    {
                        string fname = "\\Request_" + ERPType + "_" + strOperation + "_" + DeliveryNo + "_" + DateTime.Now.ToString("MMddyyyyhhmmssFFFF") + ".xml";
                        using (System.IO.StreamWriter outfile = new System.IO.StreamWriter("C:\\Processweaver\\BackUp" + Convert.ToString(fname)))
                        {
                            outfile.Write(xmldoc.InnerXml);
                        }
                    }
                    catch (Exception)
                    {
                    }
                    #region MainInfo
                    if (ERPType.ToString().ToUpper() == "SAPB1" || ERPType.ToString().ToUpper() == "SAP BUSINESS ONE")
                    {
                        try
                        {
                            PSEncryption_Decryption.Encryption_Decryption objEncryption_Decryption = new PSEncryption_Decryption.Encryption_Decryption();
                            if (DateTime.Now.Year.ToString() == objEncryption_Decryption.Decrypt(ShippingKey))
                            {
                                try
                                {
                                    if (strOperation == "SHIP")
                                    {
                                        clsProcessShipment shipobj = new clsProcessShipment();
                                        Resxml = shipobj.DoShipment(ERPType, DeliveryNo);
                                    }
                                    else if (strOperation.ToUpper() == "VOID")
                                    {
                                        clsProcessShipment voidpobj = new clsProcessShipment();
                                        Resxml = voidpobj.VoidShipment(ERPType, DeliveryNo);
                                    }
                                    return HResponsexml(Resxml.InnerXml);
                                }
                                catch (Exception ex)
                                {
                                    var st = new StackTrace(ex, true);
                                    var frame = st.GetFrame(0);
                                    var line = frame.GetFileLineNumber();
                                    return HResponsexml(Response("ERROR in Processing the Shipment :: ERP: " + ERPType.ToString().ToUpper() + " Error :: " + frame.ToString()));
                                }
                            }
                            else
                            {
                                return HResponsexml(Response("Shipping key has been expired, Please contact ProcessWeaver support for further assistance"));
                            }
                        }
                        catch (Exception Ex)
                        {
                            return HResponsexml(Response(Ex.Message.ToString()));
                        }
                    }
                    else
                    {
                        return HResponsexml(Response("ERP TYPE NOT FOUND"));
                    }
                    #endregion
                }
                else
                {
                    return HResponsexml(Response("SHIP API.." + DateTime.Now.ToString()));
                }
            }
            catch(Exception Ex)
            {
                return HResponsexml(Response("XML Loading Failed in ERP Engine :: Error " + Ex.Message));
            }
        }
    }
}
